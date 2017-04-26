#if DEBUG

using System;
using System.Collections.Generic;

namespace Producer.Domain
{
	public static class TempData
	{

		public static List<AvContent> PublicAvContent = new List<AvContent> {
			new AvContent {
				ProducerId = "admin",
				PublishedTo = UserRoles.General,
				PublishedAt = DateTimeOffset.Now,
				ContentType = AvContentTypes.Audio,
				Name = "Super Awesome Song.mp3",
				DisplayName = "Super Awesome Song",
				Description = "A super duper awesome song uploaded by me",
				StorageUrl = @"http://APP-SERVICE.streaming.mediaservices.windows.net/[your media services container (or maybe blob) id]/[your media item name].ism/manifest(format=m3u8-aapl)"
			}
		};
	}
}

#endif