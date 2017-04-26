#if !FUNCTION_EDIT
#load "..\domain\apsPayload.csx"
#load "..\domain\contentEncodedMessage.csx"

#r "Microsoft.Azure.NotificationHubs"
#r "Newtonsoft.Json"
#else
using System;
using System.Threading.Tasks;
#endif

using Microsoft.Azure.NotificationHubs;

using Newtonsoft.Json.Linq;

#if FUNCTION_EDIT
public static class AvContentTableFunction
{
#endif

	public static async Task Run (ContentEncodedMessage contentMessage, JObject record, IAsyncCollector<Notification> notification, TraceWriter log)
	{
		if (record != null)
		{
			try
			{
				if (string.IsNullOrEmpty (contentMessage.ContentId))
					throw new ArgumentException ("Must have value set for ContentId", nameof (contentMessage));


				if (string.IsNullOrEmpty (contentMessage.StorageUri))
					throw new ArgumentException ("Must have value set for StorageUri", nameof (contentMessage));



				record ["storageUrl"] = contentMessage.StorageUri;

				record ["publishedTo"] = "0";



				var contentName = (string)record ["displayName"];


				var payload = ApsPayload.Create ("New Music!", contentName, true).Serialize ();


				log.Info ($"Sending Notification payload: {payload}");


				await notification.AddAsync (new AppleNotification (payload));
			}
			catch (Exception ex)
			{
				log.Error (ex.Message);
				throw;
			}
		}
		else
		{
			var ex = new Exception ($"Unable to find record with Id {contentMessage.ContentId}");
			log.Error (ex.Message);
			throw ex;
		}
	}

#if FUNCTION_EDIT
}
#endif
