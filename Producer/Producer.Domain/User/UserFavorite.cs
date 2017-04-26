using NomadCode.Azure;

namespace Producer.Domain
{
	public class UserFavorite : AzureEntity
	{
		public int Index { get; set; }

		public string OwnerId { get; set; }

		public string ContentId { get; set; }
	}
}
