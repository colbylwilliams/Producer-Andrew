#if __MOBILE__
using System;
using Newtonsoft.Json;
#endif

namespace Producer.Domain
{
	public class AvContent : Content
	{
		public double Duration { get; set; }

		public string StorageUrl { get; set; }

		public string DisplayName { get; set; }

		public AvContentTypes ContentType { get; set; }


#if __MOBILE__

		[JsonIgnore]
		public bool HasStorageUrl => !string.IsNullOrWhiteSpace (StorageUrl);

		[JsonIgnore]
		public string DurationString => $"{Math.Floor (Duration / 60)}:{(Duration % 60).ToString ("00")}";

		[JsonIgnore]
		public bool Local => !string.IsNullOrWhiteSpace (LocalStorageUrl);

		[JsonIgnore]
		public string LocalStorageUrl {
			get { return SettingsStudio.Settings.StringForKey (Id); }
			set { SettingsStudio.Settings.SetSetting (Id, value ?? string.Empty); }
		}

		[JsonIgnore]
		public string LocalInboxPath {
			get { return SettingsStudio.Settings.StringForKey ($"inbox-{Id}"); }
			set { SettingsStudio.Settings.SetSetting ($"inbox-{Id}", value ?? string.Empty); }
		}

#endif
	}
}
