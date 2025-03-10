// 
// CGColorSpace.cs: Implements geometry classes
//
// Authors: Mono Team
//     
// Copyright 2009 Novell, Inc
// Copyright 2011-2014,2016 Xamarin Inc
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
using System.ComponentModel;
using System.Runtime.InteropServices;

using CoreFoundation;
using ObjCRuntime;
using Foundation;

#if !NET
using NativeHandle = System.IntPtr;
#endif

namespace CoreGraphics {

	// untyped enum -> CGColorSpace.h
	public enum CGColorRenderingIntent {
		Default,
		AbsoluteColorimetric,
		RelativeColorimetric,
		Perceptual,
		Saturation
	};
	
	// untyped enum -> CGColorSpace.h
	public enum CGColorSpaceModel {
		Unknown = -1,
		Monochrome,
		RGB,
		CMYK,
		Lab,
		DeviceN,
		Indexed,
		Pattern,
		Xyz,
	}

#if NET
	[SupportedOSPlatform ("ios")]
	[SupportedOSPlatform ("maccatalyst")]
	[SupportedOSPlatform ("macos")]
	[SupportedOSPlatform ("tvos")]
#endif
	public class CGColorSpace : NativeObject
	{
#if !COREBUILD
#if !XAMCORE_3_0
#if !NET
		[Obsolete ("Use a real 'null' value instead of this managed wrapper over a null native instance.")]
#else
		[Obsolete ("Use a real 'null' value instead of this managed wrapper over a null native instance.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		public readonly static CGColorSpace Null = CreateNull ();
#endif

#if !NET
		public CGColorSpace (NativeHandle handle)
			: base (handle, false)
		{
		}
#endif

#if !XAMCORE_3_0
		static CGColorSpace CreateNull ()
		{
			var throwOnInitFailure = Class.ThrowOnInitFailure;
			Class.ThrowOnInitFailure = false;
			try {
				return new CGColorSpace (IntPtr.Zero, true);
			} finally {
				Class.ThrowOnInitFailure = throwOnInitFailure;
			}
		}
#endif

		static IntPtr Create (CFPropertyList propertyList)
		{
			if (propertyList is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (propertyList));
			return CGColorSpaceCreateWithPropertyList (propertyList.GetCheckedHandle ());
		}

		public CGColorSpace (CFPropertyList propertyList)
			: base (Create (propertyList), true)
		{
		}

		[Preserve (Conditional=true)]
		internal CGColorSpace (NativeHandle handle, bool owns)
			: base (handle, owns)
		{
		}

		internal static CGColorSpace? FromHandle (IntPtr handle, bool owns)
		{
			if (handle == IntPtr.Zero)
				return null;
			return new CGColorSpace (handle, owns);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGColorSpaceRelease (/* CGColorSpaceRef */ IntPtr space);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceRetain (/* CGColorSpaceRef */ IntPtr space);

		protected internal override void Retain ()
		{
			CGColorSpaceRetain (GetCheckedHandle ());
		}

		protected internal override void Release ()
		{
			CGColorSpaceRelease (GetCheckedHandle ());
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateDeviceGray ();

		public static CGColorSpace CreateDeviceGray ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceGray (), true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateDeviceRGB ();

		public static CGColorSpace CreateDeviceRGB ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceRGB (), true);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static IntPtr CGColorSpaceCreateDeviceCMYK ();

		public static /* CGColorSpaceRef */ CGColorSpace CreateDeviceCmyk ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceCMYK (), true);
		}

#if !XAMCORE_3_0
#if !NET
		[Obsolete ("This method has been renamed 'CreateDeviceCmyk'.")]
#else
		[Obsolete ("This method has been renamed 'CreateDeviceCmyk'.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
		[EditorBrowsable (EditorBrowsableState.Never)]
		public static /* CGColorSpaceRef */ CGColorSpace CreateDeviceCMYK ()
		{
			return new CGColorSpace (CGColorSpaceCreateDeviceCMYK (), true);
		}
#endif
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateCalibratedGray (/* const CGFloat[3] */ nfloat [] whitepoint, /* const CGFloat[3] */ nfloat []? blackpoint, /* CGFloat */ nfloat gamma);

		public static CGColorSpace? CreateCalibratedGray (nfloat [] whitepoint, nfloat []? blackpoint, nfloat gamma)
		{
			if (whitepoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (whitepoint));
			if (whitepoint.Length != 3)
				throw new ArgumentException ("Must have exactly 3 values", nameof (whitepoint));
			if (blackpoint is not null && blackpoint.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (blackpoint));
			
			var ptr = CGColorSpaceCreateCalibratedGray (whitepoint, blackpoint, gamma);
			return FromHandle (ptr, true);
		}
		
		// 3, 3, 3, 9
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateCalibratedRGB (/* const CGFloat[3] */ nfloat [] whitePoint, /* const CGFloat[3] */ nfloat []? blackPoint, /* const CGFloat[3] */ nfloat []? gamma, /* const CGFloat[9] */ nfloat []? matrix);

		public static CGColorSpace? CreateCalibratedRGB (nfloat [] whitepoint, nfloat []? blackpoint, nfloat []? gamma, nfloat []? matrix)
		{
			if (whitepoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (whitepoint));
			if (whitepoint.Length != 3)
				throw new ArgumentException ("Must have exactly 3 values", nameof (whitepoint));
			if (blackpoint is not null && blackpoint.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (blackpoint));
			if (gamma is not null && gamma.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (gamma));
			if (matrix is not null && matrix.Length != 9)
				throw new ArgumentException ("Must be null or have exactly 9 values", nameof (matrix));
			
			var ptr = CGColorSpaceCreateCalibratedRGB (whitepoint, blackpoint, gamma, matrix);
			return FromHandle (ptr, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef __nullable */ IntPtr CGColorSpaceCreateLab (nfloat [] whitepoint, nfloat []? blackpoint, nfloat []? range);

		// Available since the beginning of time
		public static CGColorSpace? CreateLab (nfloat [] whitepoint, nfloat []? blackpoint, nfloat []? range)
		{
			if (whitepoint is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (whitepoint));
			if (whitepoint.Length != 3)
				throw new ArgumentException ("Must have exactly 3 values", nameof (whitepoint));
			if (blackpoint is not null && blackpoint.Length != 3)
				throw new ArgumentException ("Must be null or have exactly 3 values", nameof (blackpoint));
			if (range is not null && range.Length != 4)
				throw new ArgumentException ("Must be null or have exactly 4 values", nameof (range));

			var ptr = CGColorSpaceCreateLab (whitepoint, blackpoint, range);
			return FromHandle (ptr, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateIndexed (/* CGColorSpaceRef */ IntPtr baseSpace,
			/* size_t */ nint lastIndex, /* const unsigned char* */ byte[] colorTable);

		public static CGColorSpace? CreateIndexed (CGColorSpace baseSpace, int lastIndex, byte[] colorTable)
		{
			var ptr = CGColorSpaceCreateIndexed (baseSpace.GetHandle (), lastIndex, colorTable);
			return FromHandle (ptr, true);
		}

			
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreatePattern (/* CGColorSpaceRef */ IntPtr baseSpace);

		public static CGColorSpace? CreatePattern (CGColorSpace baseSpace)
		{
			var ptr = CGColorSpaceCreatePattern (baseSpace.GetHandle ());
			return FromHandle (ptr, true);
		}
		
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateWithName (/* CFStringRef */ IntPtr name);

		public static CGColorSpace? CreateWithName (string name)
		{
			if (name is null)
				ObjCRuntime.ThrowHelper.ThrowArgumentNullException (nameof (name));
			using (var ns = new NSString (name)) {
				var cs = CGColorSpaceCreateWithName (ns.Handle);
				return FromHandle (cs, true);
			}
		}

		static CGColorSpace? Create (IntPtr handle)
		{
			if (handle == IntPtr.Zero)
				return null;
			var r = CGColorSpaceCreateWithName (handle);
			return FromHandle (r, true);
		}

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
#endif
		public static CGColorSpace? CreateGenericGray ()
		{
			return Create (CGColorSpaceNames.GenericGray.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
#endif
		public static CGColorSpace? CreateGenericRgb ()
		{
			return Create (CGColorSpaceNames.GenericRgb.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (8,0)]
#endif
		public static CGColorSpace? CreateGenericCmyk ()
		{
			return Create (CGColorSpaceNames.GenericCmyk.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
#endif
		public static CGColorSpace? CreateGenericRgbLinear ()
		{
			return Create (CGColorSpaceNames.GenericRgbLinear.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (8,0)]
#endif
		public static CGColorSpace? CreateAdobeRgb1988 ()
		{
			return Create (CGColorSpaceNames.AdobeRgb1998.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (8,0)]
#endif
		public static CGColorSpace? CreateSrgb ()
		{
			return Create (CGColorSpaceNames.Srgb.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios8.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (8,0)]
#endif
		public static CGColorSpace? CreateGenericGrayGamma2_2 ()
		{
			return Create (CGColorSpaceNames.GenericGrayGamma2_2.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		public static CGColorSpace? CreateGenericXyz ()
		{
			return Create (CGColorSpaceNames.GenericXyz.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		public static CGColorSpace? CreateAcesCGLinear ()
		{
			return Create (CGColorSpaceNames.AcesCGLinear.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		public static CGColorSpace? CreateItuR_709 ()
		{
			return Create (CGColorSpaceNames.ItuR_709.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		public static CGColorSpace? CreateItuR_2020 ()
		{
			return Create (CGColorSpaceNames.ItuR_2020.Handle);
		}

#if NET
		[SupportedOSPlatform ("ios9.0")]
		[SupportedOSPlatform ("macos10.11")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (9,0)]
		[Mac (10,11)]
#endif
		public static CGColorSpace? CreateRommRgb ()
		{
			return Create (CGColorSpaceNames.RommRgb.Handle);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceGetBaseColorSpace (/* CGColorSpaceRef */ IntPtr space);

		public CGColorSpace? GetBaseColorSpace ()
		{
			var h = CGColorSpaceGetBaseColorSpace (Handle);
			return FromHandle (h, false);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static CGColorSpaceModel CGColorSpaceGetModel (/* CGColorSpaceRef */ IntPtr space);

		public CGColorSpaceModel Model {
			get {
				return CGColorSpaceGetModel (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGColorSpaceGetNumberOfComponents (/* CGColorSpaceRef */ IntPtr space);
		
		public nint Components {
			get {
				return CGColorSpaceGetNumberOfComponents (Handle);
			}
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* size_t */ nint CGColorSpaceGetColorTableCount (/* CGColorSpaceRef */ IntPtr space);

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static void CGColorSpaceGetColorTable (/* CGColorSpaceRef */ IntPtr space, /* uint8_t* */ byte[] table);
		
		public byte[] GetColorTable ()
		{
			nint n = CGColorSpaceGetColorTableCount (Handle);
			if (n == 0)
				return Array.Empty<byte> ();
			
			byte[] table = new byte [n * GetBaseColorSpace ()!.Components];
			CGColorSpaceGetColorTable (Handle, table);
			return table;
		}
			
#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos10.13")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13 use 'CreateIDCCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos11.0 use 'CreateIDCCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 use 'CreateIDCCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreateIDCCData' instead.")]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateWithICCProfile (/* CFDataRef */ IntPtr data);

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateWithICCData (/* CFTypeRef cg_nullable */ IntPtr data);

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos10.13")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13 use 'CreateIDCCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos11.0 use 'CreateIDCCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 use 'CreateIDCCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'CreateIDCCData' instead.")]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'CreateIDCCData' instead.")]
#endif
#if NET
		public static CGColorSpace? CreateIccProfile (NSData? data)
#else
		public static CGColorSpace? CreateICCProfile (NSData? data)
#endif
		{
			IntPtr ptr = CGColorSpaceCreateWithICCProfile (data.GetHandle ());
			return FromHandle (ptr, true);
		}

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("tvos")]
#else
		[iOS (10, 0)]
		[Mac (10, 12)]
#endif
		public static CGColorSpace? CreateIccData (NSData data)
		{
			return CreateIccData (data.GetHandle ());
		}

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
#endif
		public static CGColorSpace? CreateIccData (CGDataProvider provider)
		{
			return CreateIccData (provider.GetHandle ());
		}

		static CGColorSpace? CreateIccData (IntPtr handle)
		{
			var ptr = CGColorSpaceCreateWithICCData (handle);
			return FromHandle (ptr, true);
		}

		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CGColorSpaceRef */ IntPtr CGColorSpaceCreateICCBased (/* size_t */ nint nComponents,
			/* const CGFloat* __nullable */ nfloat[]? range, 
			/* CGDataProviderRef __nullable */ IntPtr profile,
			/* CGColorSpaceRef __nullable */ IntPtr alternate);

#if NET
		public static CGColorSpace? CreateIccProfile (nfloat[]? range, CGDataProvider profile, CGColorSpace alternate)
#else
		public static CGColorSpace? CreateICCProfile (nfloat[]? range, CGDataProvider profile, CGColorSpace alternate)
#endif
		{
			nint nComponents = range is null ? 0 : range.Length / 2;
			var ptr = CGColorSpaceCreateICCBased (nComponents, range, profile.GetHandle (), alternate.GetHandle ());
			return FromHandle (ptr, true);
		}

#if NET
		[SupportedOSPlatform ("ios")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos10.13")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13 use 'GetICCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos11.0 use 'GetICCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 use 'GetICCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'GetICCData' instead." )]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		extern static /* CFDataRef */ IntPtr CGColorSpaceCopyICCProfile (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios7.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
		[SupportedOSPlatform ("tvos")]
		[UnsupportedOSPlatform ("macos10.13")]
		[UnsupportedOSPlatform ("tvos11.0")]
		[UnsupportedOSPlatform ("ios11.0")]
#if MONOMAC
		[Obsolete ("Starting with macos10.13 use 'GetICCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos11.0 use 'GetICCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios11.0 use 'GetICCData' instead.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[iOS (7,0)] // note: pre-release docs/headers says iOS6 and later, available on OSX since 10.5
		[Deprecated (PlatformName.MacOSX, 10, 13, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.iOS, 11, 0, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.TvOS, 11, 0, message: "Use 'GetICCData' instead." )]
		[Deprecated (PlatformName.WatchOS, 4, 0, message: "Use 'GetICCData' instead." )]
#endif
#if NET
		public NSData? GetIccProfile ()
#else
		public NSData? GetICCProfile ()
#endif
		{
			IntPtr ptr = CGColorSpaceCopyICCProfile (Handle);
			return Runtime.GetNSObject<NSData> (ptr, true);
		}

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern /* CFDataRef* */ IntPtr CGColorSpaceCopyICCData (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
#endif
		public NSData? GetIccData ()
		{
			IntPtr ptr = CGColorSpaceCopyICCData (Handle);
			return Runtime.GetNSObject<NSData> (ptr, true);
		}

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[iOS (10,0)]
		[TV (10,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern unsafe /* CFStringRef* */ IntPtr CGColorSpaceCopyName (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[SupportedOSPlatform ("macos")]
#else
		[iOS (10,0)]
		[TV (10,0)]
#endif
		public string? Name {
			get {
				return CFString.FromHandle (CGColorSpaceCopyName (Handle), true);
			}
		}

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CGColorSpaceIsWideGamutRGB (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[Watch (3,0)]
		[TV (10,0)]
#endif
		public bool IsWideGamutRgb {
			get {
				return CGColorSpaceIsWideGamutRGB (Handle);
			}
		}

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[TV (10,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CGColorSpaceSupportsOutput (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[TV (10,0)]
#endif
		public bool SupportsOutput {
			get {
				return CGColorSpaceSupportsOutput (Handle);
			}
		}

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[TV (10,0)]
		[Watch (5,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCopyPropertyList (IntPtr space);

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[TV (10,0)]
		[Watch (5,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateWithPropertyList (IntPtr plist);

#if NET
		[SupportedOSPlatform ("ios10.0")]
		[SupportedOSPlatform ("macos10.12")]
		[SupportedOSPlatform ("tvos10.0")]
		[SupportedOSPlatform ("maccatalyst")]
#else
		[iOS (10,0)]
		[Mac (10,12)]
		[TV (10,0)]
		[Watch (5,0)]
#endif
		public CFPropertyList? ToPropertyList ()
		{
			var x = CGColorSpaceCopyPropertyList (Handle);
			if (x == IntPtr.Zero)
				return null;
			return new CFPropertyList (x, owns: true);
		}

#if NET
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15.4")]
		[UnsupportedOSPlatform ("tvos13.4")]
		[UnsupportedOSPlatform ("ios13.4")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15.4.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.4.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.4.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Mac (10,15)]
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
		[Deprecated (PlatformName.MacOSX, 10,15,4)]
		[Deprecated (PlatformName.iOS, 13,4)]
		[Deprecated (PlatformName.TvOS, 13,4)]
		[Deprecated (PlatformName.WatchOS, 6,2)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CGColorSpaceIsHDR (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("macos10.15")]
		[SupportedOSPlatform ("ios13.0")]
		[SupportedOSPlatform ("tvos13.0")]
		[SupportedOSPlatform ("maccatalyst")]
		[UnsupportedOSPlatform ("macos10.15.4")]
		[UnsupportedOSPlatform ("tvos13.4")]
		[UnsupportedOSPlatform ("ios13.4")]
#if MONOMAC
		[Obsolete ("Starting with macos10.15.4.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif TVOS
		[Obsolete ("Starting with tvos13.4.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#elif IOS
		[Obsolete ("Starting with ios13.4.", DiagnosticId = "BI1234", UrlFormat = "https://github.com/xamarin/xamarin-macios/wiki/Obsolete")]
#endif
#else
		[Mac (10,15)]
		[iOS (13,0)]
		[TV (13,0)]
		[Watch (6,0)]
		[Deprecated (PlatformName.MacOSX, 10,15,4)]
		[Deprecated (PlatformName.iOS, 13,4)]
		[Deprecated (PlatformName.TvOS, 13,4)]
		[Deprecated (PlatformName.WatchOS, 6,2)]
#endif
		public bool IsHdr {
			get {
				return CGColorSpaceIsHDR (Handle);
			}
		}

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,0)]
		[TV (14,0)]
		[Watch (7,0)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CGColorSpaceUsesExtendedRange (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios14.0")]
		[SupportedOSPlatform ("tvos14.0")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,0)]
		[TV (14,0)]
		[Watch (7,0)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		public bool UsesExtendedRange {
			get {
				return CGColorSpaceUsesExtendedRange (Handle);
			}
		}

#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Watch (7,1)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		[return: MarshalAs (UnmanagedType.I1)]
		static extern bool CGColorSpaceUsesITUR_2100TF (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Watch (7,1)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		public bool UsesItur2100TF => CGColorSpaceUsesITUR_2100TF (Handle);

#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Watch (7,1)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateLinearized (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Watch (7,1)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		public CGColorSpace? CreateLinearized () => Runtime.GetINativeObject<CGColorSpace> (CGColorSpaceCreateLinearized (Handle), owns: true);

#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Watch (7,1)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateExtended (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Watch (7,1)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		public CGColorSpace? CreateExtended () => Runtime.GetINativeObject<CGColorSpace> (CGColorSpaceCreateExtended (Handle), owns: true);

#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Watch (7,1)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern IntPtr CGColorSpaceCreateExtendedLinearized (/* CGColorSpaceRef */ IntPtr space);

#if NET
		[SupportedOSPlatform ("ios14.1")]
		[SupportedOSPlatform ("tvos14.2")]
		[SupportedOSPlatform ("macos11.0")]
		[SupportedOSPlatform ("maccatalyst14.0")]
#else
		[iOS (14,1)]
		[TV (14,2)]
		[Watch (7,1)]
		[Mac (11,0)]
		[MacCatalyst (14,0)]
#endif
		public CGColorSpace? CreateExtendedLinearized () => Runtime.GetINativeObject<CGColorSpace> (CGColorSpaceCreateExtendedLinearized (Handle), owns: true);

#if NET
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Mac (12,0)]
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
		[Watch (8,0)]
#endif
		[return: MarshalAs (UnmanagedType.I1)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern bool CGColorSpaceIsHLGBased (/* CGColorSpace */ IntPtr space);

#if NET
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Mac (12,0)]
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
		[Watch (8,0)]
#endif
		public bool IsHlgBased => CGColorSpaceIsHLGBased (Handle); 

#if NET
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Mac (12,0)]
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
		[Watch (8,0)]
#endif
		[return: MarshalAs (UnmanagedType.I1)]
		[DllImport (Constants.CoreGraphicsLibrary)]
		static extern bool CGColorSpaceIsPQBased (/* CGColorSpace */ IntPtr space);

#if NET
		[SupportedOSPlatform ("macos12.0")]
		[SupportedOSPlatform ("ios15.0")]
		[SupportedOSPlatform ("tvos15.0")]
		[SupportedOSPlatform ("maccatalyst15.0")]
#else
		[Mac (12,0)]
		[iOS (15,0)]
		[TV (15,0)]
		[MacCatalyst (15,0)]
		[Watch (8,0)]
#endif
		public bool IsPQBased => CGColorSpaceIsPQBased (Handle);


#endif // !COREBUILD
	}
}
