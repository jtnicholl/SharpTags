// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes.Html.Elements;

namespace SharpTags.Nodes.Html.Attributes.Values;

internal static class MiscAttributeValues
{
	public static readonly ReadOnlyMemory<byte> Render;
	public static readonly ReadOnlyMemory<byte> Any;
	public static readonly ReadOnlyMemory<byte> CloseRequest;
	public static readonly ReadOnlyMemory<byte> NoDownload;
	public static readonly ReadOnlyMemory<byte> NoFullscreen;
	public static readonly ReadOnlyMemory<byte> NoRemotePlayback;

	public static readonly ReadOnlyMemory<byte> Anonymous;
	public static readonly ReadOnlyMemory<byte> UseCredentials;

	// meta
	public static readonly ReadOnlyMemory<byte> ContentType;
	public static readonly ReadOnlyMemory<byte> ContentSecurityPolicy;
	public static readonly ReadOnlyMemory<byte> DefaultStyle;
	public static readonly ReadOnlyMemory<byte> Refresh;
	public static readonly ReadOnlyMemory<byte> Utf8;

	// link
	public static readonly ReadOnlyMemory<byte> AudioWorklet;
	public static readonly ReadOnlyMemory<byte> Fetch;
	public static readonly ReadOnlyMemory<byte> Font;
	public static readonly ReadOnlyMemory<byte> Image;
	public static readonly ReadOnlyMemory<byte> Json;
	public static readonly ReadOnlyMemory<byte> PaintWorklet;
	public static readonly ReadOnlyMemory<byte> Script = ElementNames.Script;
	public static readonly ReadOnlyMemory<byte> ServiceWorker;
	public static readonly ReadOnlyMemory<byte> SharedWorker;
	public static readonly ReadOnlyMemory<byte> Style = ElementNames.Style;
	public static readonly ReadOnlyMemory<byte> Text;
	public static readonly ReadOnlyMemory<byte> Track;
	public static readonly ReadOnlyMemory<byte> Worker;

	// ordered list style
	public static readonly ReadOnlyMemory<byte> LowercaseLetters;
	public static readonly ReadOnlyMemory<byte> UppercaseLetters;
	public static readonly ReadOnlyMemory<byte> LowercaseRomanNumerals;
	public static readonly ReadOnlyMemory<byte> UppercaseRomanNumerals;

	public static readonly ReadOnlyMemory<byte> Hide;
	public static readonly ReadOnlyMemory<byte> Show;
	public static readonly ReadOnlyMemory<byte> Toggle;

	public static readonly ReadOnlyMemory<byte> Metadata;
	public static readonly ReadOnlyMemory<byte> ImportMap;
	public static readonly ReadOnlyMemory<byte> Module;
	public static readonly ReadOnlyMemory<byte> Self;
	public static readonly ReadOnlyMemory<byte> Blank;
	public static readonly ReadOnlyMemory<byte> Parent;
	public static readonly ReadOnlyMemory<byte> Top;
	public static readonly ReadOnlyMemory<byte> UnreferencedTop;

	// iframe sandbox
	public static readonly ReadOnlyMemory<byte> AllowDownloads;
	public static readonly ReadOnlyMemory<byte> AllowForms;
	public static readonly ReadOnlyMemory<byte> AllowModals;
	public static readonly ReadOnlyMemory<byte> AllowOrientationLock;
	public static readonly ReadOnlyMemory<byte> AllowPointerLock;
	public static readonly ReadOnlyMemory<byte> AllowPopups;
	public static readonly ReadOnlyMemory<byte> AllowPopupsToEscapeSandbox;
	public static readonly ReadOnlyMemory<byte> AllowPresentation;
	public static readonly ReadOnlyMemory<byte> AllowSameOrigin;
	public static readonly ReadOnlyMemory<byte> AllowScripts;
	public static readonly ReadOnlyMemory<byte> AllowTopNavigation;
	public static readonly ReadOnlyMemory<byte> AllowTopNavigationByUserActivation;
	public static readonly ReadOnlyMemory<byte> AllowTopNavigationToCustomProtocols;

	public static readonly ReadOnlyMemory<byte> Open;
	public static readonly ReadOnlyMemory<byte> Closed;

	// area shapes
	public static readonly ReadOnlyMemory<byte> Rectangle;
	public static readonly ReadOnlyMemory<byte> Circle;
	public static readonly ReadOnlyMemory<byte> Polygon;
	public static readonly ReadOnlyMemory<byte> Default;

	// header scopes
	public static readonly ReadOnlyMemory<byte> Row;
	public static readonly ReadOnlyMemory<byte> Column;
	public static readonly ReadOnlyMemory<byte> RowGroup;
	public static readonly ReadOnlyMemory<byte> ColumnGroup;

	// track kinds
	public static readonly ReadOnlyMemory<byte> Captions;
	public static readonly ReadOnlyMemory<byte> Descriptions;
	public static readonly ReadOnlyMemory<byte> Chapters;

	public static readonly ReadOnlyMemory<byte> Hard;

	static MiscAttributeValues()
	{
		ReadOnlyMemory<byte> values = ("renderanycloserequestnodownload"u8 +
			"nofullscreennoremoteplaybackanonymoususecredentials"u8 +
			"contenttypecontentsecuritypolicydefaultstylerefreshutf-8"u8 +
			"audioworkletfetchfontimagejsonpaintworkletserviceworkertexttrack"u8 +
			"sharedworkerlowercaselettersuppercaseletterslowercaseromannumerals"u8 +
			"uppercaseromannumeralshideshowtogglemetadataimportmapmodule"u8 +
			"_self_blank_parent_top_unreferencedTop"u8 +
			"allow-downloadsallow-formsallow-modalsallow-orientation-lock"u8 +
			"allow-pointer-lockallow-popups-to-escape-sandboxallow-presentation"u8 +
			"allow-same-originallow-scriptsallow-top-navigation-by-user-activation"u8 +
			"allow-top-navigation-to-custom-protocols"u8 +
			"openclosedrectanglecirclepolygonrowgroupcolgroup"u8 +
			"captionsdescriptionschaptershard"u8
		).ToArray();

		Render = values.FindSlice("render"u8);
		Any = values.FindSlice("any"u8);
		CloseRequest = values.FindSlice("closerequest"u8);
		NoDownload = values.FindSlice("nodownload"u8);
		NoFullscreen = values.FindSlice("nofullscreen"u8);
		NoRemotePlayback = values.FindSlice("noremoteplayback"u8);

		Anonymous = values.FindSlice("anonymous"u8);
		UseCredentials = values.FindSlice("usecredentials"u8);

		ContentType = values.FindSlice("contenttype"u8);
		ContentSecurityPolicy = values.FindSlice("contentsecuritypolicy"u8);
		DefaultStyle = values.FindSlice("defaultstyle"u8);
		Refresh = values.FindSlice("refresh"u8);
		Utf8 = values.FindSlice("utf-8"u8);

		AudioWorklet = values.FindSlice("audioworklet"u8);
		Fetch = values.FindSlice("fetch"u8);
		Font = values.FindSlice("font"u8);
		Image = values.FindSlice("image"u8);
		Json = values.FindSlice("json"u8);
		PaintWorklet = values.FindSlice("paintworklet"u8);
		ServiceWorker = values.FindSlice("serviceworker"u8);
		SharedWorker = values.FindSlice("sharedworker"u8);
		Text = values.FindSlice("text"u8);
		Track = values.FindSlice("track"u8);
		Worker = values.FindSlice("worker"u8);

		LowercaseLetters = values.FindSlice("lowercaseletters"u8);
		UppercaseLetters = values.FindSlice("uppercaseletters"u8);
		LowercaseRomanNumerals = values.FindSlice("lowercaseromannumerals"u8);
		UppercaseRomanNumerals = values.FindSlice("uppercaseromannumerals"u8);

		Hide = values.FindSlice("hide"u8);
		Show = values.FindSlice("show"u8);
		Toggle = values.FindSlice("toggle"u8);

		Metadata = values.FindSlice("metadata"u8);
		ImportMap = values.FindSlice("importmap"u8);
		Module = values.FindSlice("module"u8);
		Self = values.FindSlice("_self"u8);
		Blank = values.FindSlice("_blank"u8);
		Parent = values.FindSlice("_parent"u8);
		Top = values.FindSlice("_top"u8);
		UnreferencedTop = values.FindSlice("_unreferencedTop"u8);

		AllowDownloads = values.FindSlice("allow-downloads"u8);
		AllowForms = values.FindSlice("allow-forms"u8);
		AllowModals = values.FindSlice("allow-modals"u8);
		AllowOrientationLock = values.FindSlice("allow-orientation-lock"u8);
		AllowPointerLock = values.FindSlice("allow-pointer-lock"u8);
		AllowPopups = values.FindSlice("allow-popups"u8);
		AllowPopupsToEscapeSandbox = values.FindSlice("allow-popups-to-escape-sandbox"u8);
		AllowPresentation = values.FindSlice("allow-presentation"u8);
		AllowSameOrigin = values.FindSlice("allow-same-origin"u8);
		AllowScripts = values.FindSlice("allow-scripts"u8);
		AllowTopNavigation = values.FindSlice("allow-top-navigation"u8);
		AllowTopNavigationByUserActivation =
			values.FindSlice("allow-top-navigation-by-user-activation"u8);
		AllowTopNavigationToCustomProtocols =
			values.FindSlice("allow-top-navigation-to-custom-protocols"u8);

		Open = values.FindSlice("open"u8);
		Closed = values.FindSlice("closed"u8);

		Rectangle = values.FindSlice("rectangle"u8);
		Circle = values.FindSlice("circle"u8);
		Polygon = values.FindSlice("polygon"u8);
		Default = values.FindSlice("default"u8);

		Row = values.FindSlice("row"u8);
		Column = values.FindSlice("col"u8);
		RowGroup = values.FindSlice("rowgroup"u8);
		ColumnGroup = values.FindSlice("colgroup"u8);

		Captions = values.FindSlice("captions"u8);
		Descriptions = values.FindSlice("descriptions"u8);
		Chapters = values.FindSlice("chapters"u8);

		Hard = values.FindSlice("hard"u8);
	}
}
