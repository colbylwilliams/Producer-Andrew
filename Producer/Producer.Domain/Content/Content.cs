using System;

using NomadCode.Azure;

#if __MOBILE__
using Newtonsoft.Json;
#endif

namespace Producer.Domain
{
	public class Content : AzureEntity
	{
		public string Name { get; set; }

		public string ProducerId { get; set; }

		public string Description { get; set; }

		public UserRoles PublishedTo { get; set; } = UserRoles.Producer;

		public DateTimeOffset? PublishedAt { get; set; }

#if __MOBILE__

		[JsonIgnore]
		public bool Published => PublishedAt.HasValue && !string.IsNullOrEmpty (ProducerId);

#endif
	}
}
