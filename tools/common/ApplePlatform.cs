//
// ApplePlatform.cs
//
// Copyright 2020 Microsoft Corp. All Rights Reserved.

namespace Xamarin.Utils {
	public enum ApplePlatform {
		None,
		MacOSX,
		iOS,
		WatchOS,
		TVOS,
		MacCatalyst,
	}

	public static class ApplePlatformExtensions {
		public static string AsString (this ApplePlatform @this)
		{
			switch (@this) {
			case ApplePlatform.iOS:
				return "iOS";
			case ApplePlatform.MacOSX:
				return "macOS";
			case ApplePlatform.WatchOS:
				return "watchOS";
			case ApplePlatform.TVOS:
				return "tvOS";
			case ApplePlatform.MacCatalyst:
				return "MacCatalyst";
			case ApplePlatform.None:
				return "None";
			default:
				return "Unknown";
			}
		}

		public static string ToFramework (this ApplePlatform @this)
		{
			var netVersion = "net6.0";
			switch (@this) {
			case ApplePlatform.iOS:
				return netVersion + "-ios";
			case ApplePlatform.MacOSX:
				return netVersion + "-macos";
			case ApplePlatform.WatchOS:
				return netVersion + "-watchos";
			case ApplePlatform.TVOS:
				return netVersion + "-tvos";
			case ApplePlatform.MacCatalyst:
				return netVersion + "-maccatalyst";
			default:
				return "Unknown";
			}
		}
	}
}
