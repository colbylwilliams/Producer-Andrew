using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using Producer.AppService.Models;
using Producer.Domain;

namespace Producer.AppService
{
	// #if !DEBUG
	// [Authorize]
	// #endif
    [MobileAppController]
    public class GetStorageTokenController : ApiController
    {

		const string containerName = "uploads-avcontent";

		const string connString = "MS_AzureStorageAccountConnectionString";


		ProducerContext context;


		public string ConnectionString { get; }

		public CloudBlobClient BlobClient { get; }

		public CloudStorageAccount StorageAccount { get; }


		public GetStorageTokenController()
		{
			context = new ProducerContext ();

			ConnectionString = ConfigurationManager.ConnectionStrings[connString].ConnectionString;
			StorageAccount = CloudStorageAccount.Parse (ConnectionString);
			BlobClient = StorageAccount.CreateCloudBlobClient ();
		}


		[HttpGet] // GET api/GetStorageToken
		public async Task<StorageToken> GetAsync ()
		{
			AvContent avContent = null;

			var contentId = getContentIdParamater (Request);

			using (DbContextTransaction transaction = context.Database.BeginTransaction ())
			{
				try
				{
					avContent = await context.AvContents.FindAsync (contentId);
				}
				catch (Exception)
				{
					transaction.Rollback ();

					avContent = null;
				}
			}


			if (avContent == null)
			{
				throw new HttpResponseException (Request.CreateBadRequestResponse ("No item in database matching the contentId paramater {0}", contentId));
			}


			// Errors creating the storage container result in a 500 Internal Server Error
			var container = BlobClient.GetContainerReference (containerName);

			await container.CreateIfNotExistsAsync ();


			// TODO: Check if there's already a blob with a name matching the Id


			var sasUri = getBlobSasUri (container, avContent.Name);

			return new StorageToken
			{
				ContentId = avContent.Id,
				SasUri = sasUri
			};
		}


		static string getBlobSasUri (CloudBlobContainer container, string blobName, string policyName = null)
		{
			string sasBlobToken;

			// Get a reference to a blob within the container.
			// Note that the blob may not exist yet, but a SAS can still be created for it.
			CloudBlockBlob blob = container.GetBlockBlobReference (blobName);

			if (policyName == null)
			{
				// Create a new access policy and define its constraints.
				// Note that the SharedAccessBlobPolicy class is used both to define the parameters of an ad-hoc SAS, and
				// to construct a shared access policy that is saved to the container's shared access policies.
				SharedAccessBlobPolicy adHocSAS = new SharedAccessBlobPolicy ()
				{
					// When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
					// Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
					SharedAccessExpiryTime = DateTime.UtcNow.AddHours (24),
					Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
				};

				// Generate the shared access signature on the blob, setting the constraints directly on the signature.
				sasBlobToken = blob.GetSharedAccessSignature (adHocSAS);
			}
			else
			{
				// Generate the shared access signature on the blob. In this case, all of the constraints for the
				// shared access signature are specified on the container's stored access policy.
				sasBlobToken = blob.GetSharedAccessSignature (null, policyName);
			}

			// Return the URI string for the container, including the SAS token.
			return blob.Uri + sasBlobToken;
		}


		static string getContentIdParamater(HttpRequestMessage request)
		{
			var contentId = request.GetQueryNameValuePairs ().FirstOrDefault (kv => kv.Key == StorageToken.ContentIdParam).Value;

			if (string.IsNullOrEmpty(contentId))
			{
				throw new HttpResponseException (request.CreateBadRequestResponse ("The paramater {0} is required", StorageToken.ContentIdParam));
			}

			return contentId;
		}
	}
}
