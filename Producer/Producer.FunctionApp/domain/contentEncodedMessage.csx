public class ContentEncodedMessage
{
	public string ContentId { get; private set; }

	public string StorageUri { get; private set; }

	public ContentEncodedMessage (string contentId, Uri storageUri)
	{
		ContentId = contentId;

		var uriBuilder = new UriBuilder (storageUri)
		{
			Scheme = Uri.UriSchemeHttps,
			Port = -1 // default port for scheme
		};

		StorageUri = uriBuilder.Uri.AbsoluteUri;
	}
}
