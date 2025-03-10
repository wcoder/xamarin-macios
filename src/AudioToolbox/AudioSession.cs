// 
// AudioSession.cs: AudioSession bindings
//
// Authors:
//    Miguel de Icaza (miguel@novell.com)
//    Marek Safar (marek.safar@gmail.com)
//     
// Copyright 2009 Novell, Inc
// Copyright 2011-2013 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#nullable enable

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CoreFoundation;
using ObjCRuntime;
using Foundation;
using System.Threading;

using OSStatus = System.Int32;

namespace AudioToolbox {
// This API has been deprecated in iOS 7 and everyone should be using AVAudioSession now
// also AudioSession has been removed from TVOS already and AVAudioSession has been around since iOS 3.0
#if !XAMCORE_3_0
#if !TVOS
#if !MONOMAC // AudioSession isn't an OS X API, but can't remove from compat
	public class AudioSessionException : Exception {
		static string Lookup (int k)
		{
			switch ((AudioSessionErrors)k){
			case AudioSessionErrors.NotInitialized:
				return "AudioSession.Initialize has not been called";
					
			case AudioSessionErrors.AlreadyInitialized:
				return "You called AudioSession.Initialize more than once";
			
			case AudioSessionErrors.InitializationError:
				return "There was an error during the AudioSession.initialization";
				
			case AudioSessionErrors.UnsupportedPropertyError:
				return "The audio session property is not supported";
				
			case AudioSessionErrors.BadPropertySizeError:
				return "The size of the audio property was not correct";
				
			case AudioSessionErrors.NotActiveError:
				return "Application Audio Session is not active";
				
			case AudioSessionErrors.NoHardwareError:
				return "The device has no Audio Input capability";
				
			case AudioSessionErrors.IncompatibleCategory:
				return "The specified AudioSession.Category can not be used with this audio operation";
				
			case AudioSessionErrors.NoCategorySet:
				return "This operation requries AudioSession.Category to be explicitly set";
				
			}
			return String.Format ("Unknown error code: {0}", k);
		}
		
		internal AudioSessionException (int k) : base (Lookup (k))
		{
			ErrorCode = (AudioSessionErrors) k;
		}

		public AudioSessionErrors ErrorCode { get; private set; }
	}

	public class AccessoryInfo
	{
		internal AccessoryInfo (int id, string? description)
		{
			ID = id;
			Description = description;
		}

		public int ID { get; private set; }
		public string? Description { get; private set; }
	}

	public class InputSourceInfo
	{
		public int ID { get; private set; }
		public string? Description { get; private set; }
	}
	
	public class AudioSessionPropertyEventArgs :EventArgs {
		public AudioSessionPropertyEventArgs (AudioSessionProperty prop, int size, IntPtr data)
		{
			this.Property = prop;
			this.Size = size;
			this.Data = data;
		}
		public AudioSessionProperty Property { get; set; }
		public int Size  { get; set; }
		public IntPtr Data { get; set; }
	}

	public class AudioSessionRouteChangeEventArgs : EventArgs {
		static IntPtr route_change_key, previous_route_key, current_route_key;

		static AudioSessionRouteChangeEventArgs ()
		{
			var lib = Libraries.AudioToolbox.Handle;
			route_change_key = Dlfcn.GetIntPtr (lib, "kAudioSession_RouteChangeKey_Reason");
			previous_route_key = Dlfcn.GetIntPtr (lib, "kAudioSession_AudioRouteChangeKey_PreviousRouteDescription");
			current_route_key = Dlfcn.GetIntPtr (lib, "kAudioSession_AudioRouteChangeKey_CurrentRouteDescription");
		}

		public NSDictionary Dictionary { get; private set; }
		
		public AudioSessionRouteChangeEventArgs (IntPtr dictHandle)
		{
			Dictionary = new NSDictionary (dictHandle);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
#endif
		public AudioSessionRouteChangeReason Reason {
			get {
				using (var num = new NSNumber (Dictionary.LowlevelObjectForKey (route_change_key))){
					return (AudioSessionRouteChangeReason) num.Int32Value;
				}
			}
		}

		NSArray? Extract (IntPtr key, NSString secondKey)
		{
			var dictH = Dictionary.LowlevelObjectForKey (key);
			if (dictH == IntPtr.Zero)
				return null;

//			Console.WriteLine ("Extracting from {2} {0} and getting {1}", new NSString (key), new NSDictionary (dictH).Description, Dictionary.Description);
			// Description dictionary, indexed by the second key, the result is an array
			using (var descDict = new NSDictionary (dictH)){
				var sdict = descDict.LowlevelObjectForKey (secondKey.Handle);
				if (sdict == IntPtr.Zero)
					return null;

				return new NSArray (sdict);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
#endif
		public AudioSessionInputRouteKind PreviousInputRoute {
			get {
				using (var array = Extract (previous_route_key, AudioSession.AudioRouteKey_Inputs!))
					return AudioSession.GetInputRoute (array);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
#endif
		public AudioSessionOutputRouteKind []? PreviousOutputRoutes {
			get {
				using (var array = Extract (previous_route_key, AudioSession.AudioRouteKey_Outputs!))
					return AudioSession.GetOutputRoutes (array);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
#endif
		public AudioSessionInputRouteKind CurrentInputRoute {
			get {
				using (var array = Extract (current_route_key, AudioSession.AudioRouteKey_Inputs!))
					return AudioSession.GetInputRoute (array);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios7.0")]
#if IOS
		[Obsolete ("Starting with ios7.0.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 7, 0)]
#endif
		public AudioSessionOutputRouteKind []? CurrentOutputRoutes {
			get {
				using (var array = Extract (current_route_key, AudioSession.AudioRouteKey_Outputs!))
					return AudioSession.GetOutputRoutes (array);
			}
		}
	}

#if NET
	[UnsupportedOSPlatform ("ios7.0")]
#if IOS
	[Obsolete ("Starting with ios7.0 use 'AVAudioSession' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
	[Deprecated (PlatformName.iOS, 7, 0, message : "Use 'AVAudioSession' instead.")]
#endif
	public static class AudioSession {
		static bool initialized;
		public static event EventHandler? Interrupted;
		public static event EventHandler? Resumed;

		internal static NSString? AudioRouteKey_Type;
		internal static NSString? AudioRouteKey_Inputs;
		internal static NSString? AudioRouteKey_Outputs;
		
		static NSString? InputRoute_LineIn;
		static NSString? InputRoute_BuiltInMic;
		static NSString? InputRoute_HeadsetMic;
		static NSString? InputRoute_BluetoothHFP;
		static NSString? InputRoute_USBAudio;
		
		static NSString? OutputRoute_LineOut;
		static NSString? OutputRoute_Headphones;
		static NSString? OutputRoute_BluetoothHFP;
		static NSString? OutputRoute_BluetoothA2DP;
		static NSString? OutputRoute_BuiltInReceiver;
		static NSString? OutputRoute_BuiltInSpeaker;
		static NSString? OutputRoute_USBAudio;
		static NSString? OutputRoute_HDMI;
		static NSString? OutputRoute_AirPlay;
		static NSString? InputSourceKey_ID;
		static NSString? InputSourceKey_Description;
		static NSString? OutputDestinationKey_ID;
		static NSString? OutputDestinationKey_Description;
		
		[DllImport (Constants.AudioToolboxLibrary)]
#if NET
		unsafe extern static OSStatus AudioSessionInitialize(IntPtr cfRunLoop, IntPtr cfstr_runMode, delegate* unmanaged<IntPtr, uint, void> listener, IntPtr userData);
#else
		extern static OSStatus AudioSessionInitialize(IntPtr cfRunLoop, IntPtr cfstr_runMode, InterruptionListener listener, IntPtr userData);
#endif

		public static void Initialize ()
		{
			Initialize (null, null);
		}

		public static void Initialize (CFRunLoop? runLoop, string? runMode)
		{
			CFString? s = runMode is null ? null : new CFString (runMode);
#if NET
			int k;
			unsafe {
				k = AudioSessionInitialize (runLoop.GetHandle (), s.GetHandle (), &Interruption, IntPtr.Zero);
			}
#else
			int k = AudioSessionInitialize (runLoop.GetHandle (), s.GetHandle (), Interruption, IntPtr.Zero);
#endif
			if (k != 0 && k != (int)AudioSessionErrors.AlreadyInitialized)
				throw new AudioSessionException (k);
			
			if (initialized)
				return;

			IntPtr lib = Libraries.AudioToolbox.Handle;
			
			AudioRouteKey_Inputs = Dlfcn.GetStringConstant (lib, "kAudioSession_AudioRouteKey_Inputs");
			AudioRouteKey_Outputs = Dlfcn.GetStringConstant (lib, "kAudioSession_AudioRouteKey_Outputs");
			AudioRouteKey_Type = Dlfcn.GetStringConstant (lib, "kAudioSession_AudioRouteKey_Type");

			InputRoute_LineIn = Dlfcn.GetStringConstant (lib, "kAudioSessionInputRoute_LineIn");
			InputRoute_BuiltInMic = Dlfcn.GetStringConstant (lib, "kAudioSessionInputRoute_BuiltInMic");
			InputRoute_HeadsetMic = Dlfcn.GetStringConstant (lib, "kAudioSessionInputRoute_HeadsetMic");
			InputRoute_BluetoothHFP = Dlfcn.GetStringConstant (lib, "kAudioSessionInputRoute_BluetoothHFP");
			InputRoute_USBAudio = Dlfcn.GetStringConstant (lib, "kAudioSessionInputRoute_USBAudio");
			
			OutputRoute_LineOut = Dlfcn.GetStringConstant (lib, "kAudioSessionOutputRoute_LineOut");
			OutputRoute_Headphones = Dlfcn.GetStringConstant (lib, "kAudioSessionOutputRoute_Headphones");
			OutputRoute_BluetoothHFP = Dlfcn.GetStringConstant (lib, "kAudioSessionOutputRoute_BluetoothHFP");
			OutputRoute_BluetoothA2DP = Dlfcn.GetStringConstant (lib, "kAudioSessionOutputRoute_BluetoothA2DP");
			OutputRoute_BuiltInReceiver = Dlfcn.GetStringConstant (lib, "kAudioSessionOutputRoute_BuiltInReceiver");
			OutputRoute_BuiltInSpeaker = Dlfcn.GetStringConstant (lib, "kAudioSessionOutputRoute_BuiltInSpeaker");
			OutputRoute_USBAudio = Dlfcn.GetStringConstant (lib, "kAudioSessionOutputRoute_USBAudio");
			OutputRoute_HDMI = Dlfcn.GetStringConstant (lib, "kAudioSessionOutputRoute_HDMI");
			OutputRoute_AirPlay = Dlfcn.GetStringConstant (lib, "kAudioSessionOutputRoute_AirPlay");

			InputSourceKey_ID = Dlfcn.GetStringConstant (lib, "kAudioSession_InputSourceKey_ID");
			InputSourceKey_Description = Dlfcn.GetStringConstant (lib, "kAudioSession_InputSourceKey_Description");

			OutputDestinationKey_ID = Dlfcn.GetStringConstant (lib, "kAudioSession_OutputDestinationKey_ID");
			OutputDestinationKey_Description = Dlfcn.GetStringConstant (lib, "kAudioSession_OutputDestinationKey_Description");
			
			initialized = true;
		}

#if !NET
		delegate void InterruptionListener (IntPtr userData, uint state);
#endif

#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (InterruptionListener))]
#endif
		static void Interruption (IntPtr userData, uint state)
		{
			EventHandler h;

			h = (state == 1) ? Interrupted! : Resumed!;
			if (h is not null)
				h (null, EventArgs.Empty);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioSessionSetActive ([MarshalAs (UnmanagedType.I1)] bool active);

		public static void SetActive (bool active)
		{
			int k = AudioSessionSetActive (active);
			if (k != 0)
				throw new AudioSessionException (k);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static AudioSessionErrors AudioSessionSetActiveWithFlags ([MarshalAs (UnmanagedType.I1)] bool active, AudioSessionActiveFlags inFlags);

		public static AudioSessionErrors SetActive (bool active, AudioSessionActiveFlags flags)
		{
			return AudioSessionSetActiveWithFlags (active, flags);
		}

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioSessionGetProperty(AudioSessionProperty id, ref int size, IntPtr data);

		[DllImport (Constants.AudioToolboxLibrary)]
		extern static OSStatus AudioSessionSetProperty (AudioSessionProperty id, int size, IntPtr data);

		[DllImport (Constants.AudioToolboxLibrary)]
		// deprecated in iOS7 but not exposed / used anywhere
		extern static OSStatus AudioSessionGetPropertySize (AudioSessionProperty id, out int size);

		static double GetDouble (AudioSessionProperty property)
		{
			unsafe {
				double val = 0;
				int size = 8;
				int k = AudioSessionGetProperty (property, ref size, (IntPtr) (&val));
				if (k != 0)
					throw new AudioSessionException (k);

				return val;
			}
		}

		static float GetFloat (AudioSessionProperty property)
		{
			unsafe {
				float val = 0;
				int size = 4;
				int k = AudioSessionGetProperty (property, ref size, (IntPtr) (&val));
				if (k != 0)
					throw new AudioSessionException (k);
				return val;
			}
		}

		static int GetInt (AudioSessionProperty property)
		{
			unsafe {
				int val = 0;
				int size = 4;
				int k = AudioSessionGetProperty (property, ref size, (IntPtr) (&val));
				if (k != 0)
					throw new AudioSessionException (k);
				
				return val;
			}
		}

		static IntPtr GetIntPtr (AudioSessionProperty property)
		{
			unsafe {
				IntPtr val;
				int size = IntPtr.Size;
				int k = AudioSessionGetProperty (property, ref size, (IntPtr) (&val));
				if (k != 0)
					throw new AudioSessionException (k);
				
				return val;
			}
		}
		
		static void SetDouble (AudioSessionProperty property, double val)
		{
			unsafe {
				int k = AudioSessionSetProperty (property, 8, (IntPtr) (&val));
				if (k != 0)
					throw new AudioSessionException (k);
			}
		}

		static void SetInt (AudioSessionProperty property, int val)
		{
			unsafe {
				int k = AudioSessionSetProperty (property, sizeof (int), (IntPtr) (&val));
				if (k != 0)
					throw new AudioSessionException (k);
			}
		}

		static void SetFloat (AudioSessionProperty property, float val)
		{
			unsafe {
				int k = AudioSessionSetProperty (property, 4, (IntPtr) (&val));
				if (k != 0)
					throw new AudioSessionException (k);
			}
		}

		static public double PreferredHardwareSampleRate {
			get {
				return GetDouble (AudioSessionProperty.PreferredHardwareSampleRate);
			}
			set {
				SetDouble (AudioSessionProperty.PreferredHardwareSampleRate, value);
			}
		}

		static public float PreferredHardwareIOBufferDuration {
			get {
				return GetFloat (AudioSessionProperty.PreferredHardwareIOBufferDuration);
			}
			set {
				SetFloat (AudioSessionProperty.PreferredHardwareIOBufferDuration, value);
			}
		}

		static public AudioSessionCategory Category {
			get {
				return (AudioSessionCategory) GetInt (AudioSessionProperty.AudioCategory);
			}
			set {
				SetInt (AudioSessionProperty.AudioCategory, (int) value);
			}
		}

		public static AudioSessionInterruptionType InterruptionType {
			get {
				return (AudioSessionInterruptionType) GetInt (AudioSessionProperty.InterruptionType);
			}
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("ios5.0")]
#if IOS
		[Obsolete ("Starting with ios5.0 use 'InputRoute' or 'OutputRoute' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 5, 0, message : "Use 'InputRoute' or 'OutputRoute' instead.")]
#endif
		static public string? AudioRoute {
			get {
				return CFString.FromHandle (GetIntPtr (AudioSessionProperty.AudioRoute));
			}
		}

		static public AccessoryInfo[] InputSources {
			get {
				return ExtractAccessoryInfo (GetIntPtr (AudioSessionProperty.InputSources), InputSourceKey_ID!, InputSourceKey_Description!);
			}
		}

		static public AccessoryInfo[] OutputDestinations {
			get {
				return ExtractAccessoryInfo (GetIntPtr (AudioSessionProperty.OutputDestinations), OutputDestinationKey_ID!, OutputDestinationKey_Description!);
			}
		}

		static AccessoryInfo[] ExtractAccessoryInfo (IntPtr ptr, NSString id, NSString description)
		{
			using (var array = new CFArray (ptr, false)) {
				var res = new AccessoryInfo [array.Count];
				for (int i = 0; i < res.Length; ++i) {
					var dict = array.GetValue (i);
					var n = new NSNumber (CFDictionary.GetValue (dict, id.Handle));
					var desc = CFString.FromHandle (CFDictionary.GetValue (dict, description.Handle));

					res [i] = new AccessoryInfo ((int) n, desc);
					id.Dispose ();
				}
				return res;
			}
		}

		/* Could not test what sort of unique CFNumberRef value it's

		static public int InputSource {
			get {
				return GetInt (AudioSessionProperty.InputSource);
			}
			set {
				SetInt (AudioSessionProperty.InputSource, value);
			}
		}

		static public int OutputDestination {
			get {
				return GetInt (AudioSessionProperty.OutputDestination);
			}
			set {
				SetInt (AudioSessionProperty.OutputDestination, value);
			}
		}

		*/

		static internal AudioSessionInputRouteKind GetInputRoute (NSArray? arr)
		{
			if (arr is null || arr.Count == 0)
				return AudioSessionInputRouteKind.None;
			
			var dict = new NSDictionary (arr.ValueAt (0));
			
			if (dict is null || dict.Count == 0)
				return AudioSessionInputRouteKind.None;
			
			var val = (NSString) dict [AudioRouteKey_Type];
			
			if (val is null)
				return AudioSessionInputRouteKind.None;
			
			if (val == InputRoute_LineIn) {
				return AudioSessionInputRouteKind.LineIn;
			} else if (val == InputRoute_BuiltInMic) {
				return AudioSessionInputRouteKind.BuiltInMic;
			} else if (val == InputRoute_HeadsetMic) {
				return AudioSessionInputRouteKind.HeadsetMic;
			} else if (val == InputRoute_BluetoothHFP) {
				return AudioSessionInputRouteKind.BluetoothHFP;
			} else if (val == InputRoute_USBAudio) {
				return AudioSessionInputRouteKind.USBAudio;
			} else {
				return (AudioSessionInputRouteKind) (int) (IntPtr) val.Handle;
			}
		}

		static internal AudioSessionOutputRouteKind []? GetOutputRoutes (NSArray? arr)
		{
			if (arr is null || arr.Count == 0)
				return null;
			
			var result = new AudioSessionOutputRouteKind [arr.Count];
			for (uint i = 0; i < arr.Count; i++) {
				var dict = new NSDictionary ((IntPtr) arr.ValueAt (i));
				
				result [i] = AudioSessionOutputRouteKind.None;
				
				if (dict is null || dict.Count == 0)
					continue;
				
				var val = (NSString) dict [AudioRouteKey_Type];
				
				if (val is null)
					continue;
				
				if (val == OutputRoute_LineOut) {
					result [i] = AudioSessionOutputRouteKind.LineOut;
				} else if (val == OutputRoute_Headphones) {
					result [i] = AudioSessionOutputRouteKind.Headphones;
				} else if (val == OutputRoute_BluetoothHFP) {
					result [i] = AudioSessionOutputRouteKind.BluetoothHFP;
				} else if (val == OutputRoute_BluetoothA2DP) {
					result [i] = AudioSessionOutputRouteKind.BluetoothA2DP;
				} else if (val == OutputRoute_BuiltInReceiver) {
					result [i] = AudioSessionOutputRouteKind.BuiltInReceiver;
				} else if (val == OutputRoute_BuiltInSpeaker) {
					result [i] = AudioSessionOutputRouteKind.BuiltInSpeaker;
				} else if (val == OutputRoute_USBAudio) {
					result [i] = AudioSessionOutputRouteKind.USBAudio;
				} else if (val == OutputRoute_HDMI) {
					result [i] = AudioSessionOutputRouteKind.HDMI;
				} else if (val == OutputRoute_AirPlay) {
					result [i] = AudioSessionOutputRouteKind.AirPlay;
				} else
					result [i] = (AudioSessionOutputRouteKind) (int) (IntPtr) val.Handle;
			}
			return result;
		}

		static public AudioSessionInputRouteKind InputRoute {
			get {
				return GetInputRoute ((NSArray) AudioRouteDescription [AudioRouteKey_Inputs]);
			}
		}

		static public AudioSessionOutputRouteKind []? OutputRoutes {
			get {
				return GetOutputRoutes ((NSArray) AudioRouteDescription [AudioRouteKey_Outputs]);
			}
		}
		
		static NSDictionary AudioRouteDescription {
			get {
				NSDictionary dict = new NSDictionary (GetIntPtr (AudioSessionProperty.AudioRouteDescription));
				dict.DangerousRelease ();
				return dict;
			}
		}

		static public double CurrentHardwareSampleRate {
			get {
				return GetDouble (AudioSessionProperty.CurrentHardwareSampleRate);
			}
		}

		static public int CurrentHardwareInputNumberChannels {
			get {
				return GetInt (AudioSessionProperty.CurrentHardwareInputNumberChannels);
			}
		}

		static public int CurrentHardwareOutputNumberChannels {
			get {
				return GetInt (AudioSessionProperty.CurrentHardwareOutputNumberChannels);
			}
		}

		static public float CurrentHardwareOutputVolume {
			get {
				return GetFloat (AudioSessionProperty.CurrentHardwareOutputVolume);
			}
		}

		static public float CurrentHardwareInputLatency {
			get {
				return GetFloat (AudioSessionProperty.CurrentHardwareInputLatency);
			}
		}

		static public float CurrentHardwareOutputLatency {
			get {
				return GetFloat (AudioSessionProperty.CurrentHardwareOutputLatency);
			}
		}

		static public float CurrentHardwareIOBufferDuration {
			get {
				return GetFloat (AudioSessionProperty.CurrentHardwareIOBufferDuration);
			}
		}

		static public bool OtherAudioIsPlaying {
			get {
				return GetInt (AudioSessionProperty.OtherAudioIsPlaying) != 0;
			}
		}

		static public AudioSessionRoutingOverride RoutingOverride {
			set {
				SetInt (AudioSessionProperty.OverrideAudioRoute, (int) value);
			}
		}

		static public bool AudioInputAvailable {
			get {
				return GetInt (AudioSessionProperty.AudioInputAvailable) != 0;
			}
		}

		static public bool AudioShouldDuck {
			get {
				return GetInt (AudioSessionProperty.OtherMixableAudioShouldDuck) != 0;
			}
			set {
				SetInt (AudioSessionProperty.OtherMixableAudioShouldDuck, value ? 1 : 0);
			}
		}

		static public bool OverrideCategoryMixWithOthers {
			get {
				return GetInt (AudioSessionProperty.OverrideCategoryMixWithOthers) != 0;
			}
			set {
				SetInt (AudioSessionProperty.OverrideCategoryMixWithOthers, value ? 1 : 0);
			}
		}

		static public bool OverrideCategoryDefaultToSpeaker {
			get {
				return GetInt (AudioSessionProperty.OverrideCategoryDefaultToSpeaker) != 0;
			}
			set {
				SetInt (AudioSessionProperty.OverrideCategoryDefaultToSpeaker, value ? 1 : 0);
			}
		}

		static public bool OverrideCategoryEnableBluetoothInput {
			get {
				return GetInt (AudioSessionProperty.OverrideCategoryEnableBluetoothInput) != 0;
			}
			set {
				SetInt (AudioSessionProperty.OverrideCategoryEnableBluetoothInput, value ? 1 : 0);
			}
		}
		
		static public AudioSessionMode Mode {
			get {
				return (AudioSessionMode) GetInt (AudioSessionProperty.Mode);
			}
			set {
				SetInt (AudioSessionProperty.Mode, (int) value);
			}
		}

		static public bool InputGainAvailable {
			get {
				return GetInt (AudioSessionProperty.InputGainAvailable) != 0;
			}
		}

		static public float InputGainScalar {
			get {
				return GetFloat (AudioSessionProperty.InputGainScalar);
			}
			set {
				SetFloat (AudioSessionProperty.InputGainScalar, value);
			}
		}

#if !NET
		delegate void _PropertyListener (IntPtr userData, AudioSessionProperty prop, int size, IntPtr data);
#endif
		public delegate void PropertyListener (AudioSessionProperty prop, int size, IntPtr data);
		
#if NET
		[UnmanagedCallersOnly]
#else
		[MonoPInvokeCallback (typeof (_PropertyListener))]
#endif
		static void Listener (IntPtr userData, AudioSessionProperty prop, int size, IntPtr data)
		{
			ArrayList a = (ArrayList) listeners ![prop];
			if (a is null){
				// Should never happen
				return;
			}

			foreach (PropertyListener pl in a){
				pl (prop, size, data);
			}
		}

		[DllImport (Constants.AudioToolboxLibrary)]
#if NET
		unsafe extern static AudioSessionErrors AudioSessionAddPropertyListener(AudioSessionProperty id, delegate* unmanaged<IntPtr, AudioSessionProperty, int, IntPtr, void> inProc, IntPtr userData);
#else
		extern static AudioSessionErrors AudioSessionAddPropertyListener(AudioSessionProperty id, _PropertyListener inProc, IntPtr userData);
#endif

		static Hashtable? listeners;

		public static AudioSessionErrors AddListener (AudioSessionProperty property, PropertyListener listener)
		{
			if (listener is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (listener));

			if (listeners is null)
				listeners = new Hashtable ();

			ArrayList a = (ArrayList) listeners [property];
			if (a is null)
				listeners [property] = a = new ArrayList ();

			a.Add (listener);

			if (a.Count == 1) {
#if NET
				unsafe {
					return AudioSessionAddPropertyListener (property, &Listener, IntPtr.Zero);
				}
#else
				return AudioSessionAddPropertyListener (property, Listener, IntPtr.Zero);
#endif
			}

			return AudioSessionErrors.None;
		}

		public static void RemoveListener (AudioSessionProperty property, PropertyListener listener)
		{
			if (listener is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (listener));

			ArrayList a = (ArrayList) listeners ![property];
			if (a is null)
				return;
			a.Remove (listener);
			if (a.Count == 0)
				listeners [property] = null;
		}

               static Hashtable? strongListenerHash;

		static void AddListenerEvent (AudioSessionProperty property, object handler, PropertyListener listener)
		{
			if (strongListenerHash is null)
				Interlocked.CompareExchange (ref strongListenerHash, new Hashtable (), null);

			lock (strongListenerHash) {
				strongListenerHash [handler] = listener;
			}

			AddListener (property, listener);
		}

		static void RemoveListenerEvent (AudioSessionProperty property, object handler)
		{
			if (strongListenerHash is null)
				return;

			PropertyListener listener;
			lock (strongListenerHash) {
				listener = (PropertyListener) strongListenerHash [handler]; 
				if (listener is null)
					return;

				strongListenerHash.Remove (handler);
			}

			RemoveListener (AudioSessionProperty.CurrentHardwareOutputVolume, listener);
		}

		public static event EventHandler<AudioSessionRouteChangeEventArgs> AudioRouteChanged {
			add {
				AddListenerEvent (AudioSessionProperty.AudioRouteChange, value, 
					(prop, size, data) => value (null, new AudioSessionRouteChangeEventArgs (data)));
			}
			remove {
				RemoveListenerEvent (AudioSessionProperty.AudioRouteChange, value);
			}
		}

		public static event Action<float> CurrentHardwareOutputVolumeChanged {
			add {
				AddListenerEvent (AudioSessionProperty.CurrentHardwareOutputVolume, value, 
						  (prop, size, data) => value ((float) data));
			}
			remove {
				RemoveListenerEvent (AudioSessionProperty.CurrentHardwareOutputVolume, value);
			}
		}

 		public static event Action<bool> AudioInputBecameAvailable {
			add {
				AddListenerEvent (AudioSessionProperty.AudioInputAvailable, value, 
						  (prop, size, data) => value (data != IntPtr.Zero));
			}
			remove {
				RemoveListenerEvent (AudioSessionProperty.AudioInputAvailable, value);
			}
		}

		public static event Action<bool> ServerDied {
			add {
				AddListenerEvent (AudioSessionProperty.ServerDied, value, 
						  (prop, size, data) => value (data != IntPtr.Zero));
			}
			remove {
				RemoveListenerEvent (AudioSessionProperty.ServerDied, value);
			}
		}

		public static event Action<bool> InputGainBecameAvailable {
			add {
				AddListenerEvent (AudioSessionProperty.InputGainAvailable, value, 
						  (prop, size, data) => value (data != IntPtr.Zero));
			}
			remove {
				RemoveListenerEvent (AudioSessionProperty.InputGainAvailable, value);
			}
		}

		public static event Action<float> InputGainScalarChanged {
			add {
				AddListenerEvent (AudioSessionProperty.InputGainScalar, value, 
						  (prop, size, data) => value ((float) data));
			}
			remove {
				RemoveListenerEvent (AudioSessionProperty.InputGainScalar, value);
			}
		}

		public static event Action<AccessoryInfo[]> InputSourcesChanged {
			add {
				AddListenerEvent (AudioSessionProperty.InputSources, value, 
						  (prop, size, data) => value (ExtractAccessoryInfo (data, InputSourceKey_ID!, InputSourceKey_Description!)));
			}
			remove {
				RemoveListenerEvent (AudioSessionProperty.InputSources, value);
			}
		}

		public static event Action<AccessoryInfo[]> OutputDestinationsChanged {
			add {
				AddListenerEvent (AudioSessionProperty.OutputDestinations, value, 
						  (prop, size, data) => value (ExtractAccessoryInfo (data, OutputDestinationKey_ID!, OutputDestinationKey_Description!)));
			}
			remove {
				RemoveListenerEvent (AudioSessionProperty.OutputDestinations, value);
			}
		}
	}
#endif // !MONOMAC
#endif // !TVOS
#endif // !XAMCORE_3_0
}
