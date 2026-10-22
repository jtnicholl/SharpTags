// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes.Html.Attributes.Values;

internal static class GlobalAttributeValues
{
	public static readonly ReadOnlyMemory<byte> Words;
	public static readonly ReadOnlyMemory<byte> Characters;

	public static readonly ReadOnlyMemory<byte> PlainTextOnly;

	public static readonly ReadOnlyMemory<byte> LeftToRight;
	public static readonly ReadOnlyMemory<byte> RightToLeft;
	public static readonly ReadOnlyMemory<byte> Auto;
	public static readonly ReadOnlyMemory<byte> Hint;
	public static readonly ReadOnlyMemory<byte> Manual;

	public static readonly ReadOnlyMemory<byte> Enter;
	public static readonly ReadOnlyMemory<byte> Done;
	public static readonly ReadOnlyMemory<byte> Go;
	public static readonly ReadOnlyMemory<byte> Next;
	public static readonly ReadOnlyMemory<byte> Previous;
	public static readonly ReadOnlyMemory<byte> Search;
	public static readonly ReadOnlyMemory<byte> Send;
	public static readonly ReadOnlyMemory<byte> None;
	public static readonly ReadOnlyMemory<byte> Decimal;
	public static readonly ReadOnlyMemory<byte> Numeric;
	public static readonly ReadOnlyMemory<byte> Tel;
	public static readonly ReadOnlyMemory<byte> Email;
	public static readonly ReadOnlyMemory<byte> Url;

	public static readonly ReadOnlyMemory<byte> UntilFound;

	public static readonly ReadOnlyMemory<byte> On;
	public static readonly ReadOnlyMemory<byte> Off;
	public static readonly ReadOnlyMemory<byte> True;
	public static readonly ReadOnlyMemory<byte> False;
	public static readonly ReadOnlyMemory<byte> No;

	public static readonly ReadOnlyMemory<byte> Async;
	public static readonly ReadOnlyMemory<byte> Sync;
	public static readonly ReadOnlyMemory<byte> Low;
	public static readonly ReadOnlyMemory<byte> High;
	public static readonly ReadOnlyMemory<byte> Eager;
	public static readonly ReadOnlyMemory<byte> Lazy;

	public static readonly ReadOnlyMemory<byte> NoReferrer;
	public static readonly ReadOnlyMemory<byte> NoReferrerWhenDowngrade;
	public static readonly ReadOnlyMemory<byte> Origin;
	public static readonly ReadOnlyMemory<byte> OriginWhenCrossOrigin;
	public static readonly ReadOnlyMemory<byte> SameOrigin;
	public static readonly ReadOnlyMemory<byte> StrictOrigin;
	public static readonly ReadOnlyMemory<byte> StrictOriginWhenCrossOrigin;
	public static readonly ReadOnlyMemory<byte> UnsafeUrl;

	static GlobalAttributeValues()
	{
		ReadOnlyMemory<byte> values = (
			"rtltrautohintmanualwordscharactersofftruefalseplaintext-only"u8 +
			"entergodonextpreviousearchsendnonedecimalnumerictelemailurl"u8 +
			"until-foundasynclowhigheagerlazynoreferrerwhendowngrade"u8 +
			"sameoriginwhencrossoriginstrictoriginwhencrossoriginunsafeurl"u8
		).ToArray();

		Words = values.FindSlice("words"u8);
		Characters = values.FindSlice("characters"u8);

		PlainTextOnly = values.FindSlice("plaintext-only"u8);

		LeftToRight = values.FindSlice("ltr"u8);
		RightToLeft = values.FindSlice("rtl"u8);
		Auto = values.FindSlice("auto"u8);
		Hint = values.FindSlice("hint"u8);
		Manual = values.FindSlice("manual"u8);

		Enter = values.FindSlice("enter"u8);
		Done = values.FindSlice("done"u8);
		Go = values.FindSlice("go"u8);
		Next = values.FindSlice("next"u8);
		Previous = values.FindSlice("previous"u8);
		Search = values.FindSlice("search"u8);
		Send = values.FindSlice("send"u8);
		None = values.FindSlice("none"u8);
		Decimal = values.FindSlice("decimal"u8);
		Numeric = values.FindSlice("numeric"u8);
		Tel = values.FindSlice("tel"u8);
		Email = values.FindSlice("email"u8);
		Url = values.FindSlice("url"u8);

		UntilFound = values.FindSlice("until-found"u8);

		On = values.FindSlice("on"u8);
		Off = values.FindSlice("off"u8);
		True = values.FindSlice("true"u8);
		False = values.FindSlice("false"u8);
		No = values.FindSlice("no"u8);

		Async = values.FindSlice("async"u8);
		Sync = values.FindSlice("sync"u8);
		Low = values.FindSlice("low"u8);
		High = values.FindSlice("high"u8);
		Eager = values.FindSlice("eager"u8);
		Lazy = values.FindSlice("lazy"u8);

		NoReferrer = values.FindSlice("noreferrer"u8);
		NoReferrerWhenDowngrade = values.FindSlice("noreferrerwhendowngrade"u8);
		Origin = values.FindSlice("origin"u8);
		OriginWhenCrossOrigin = values.FindSlice("originwhencrossorigin"u8);
		SameOrigin = values.FindSlice("sameorigin"u8);
		StrictOrigin = values.FindSlice("strictorigin"u8);
		StrictOriginWhenCrossOrigin = values.FindSlice("strictoriginwhencrossorigin"u8);
		UnsafeUrl = values.FindSlice("unsafeurl"u8);
	}
}
