// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes.Html.Elements;

namespace SharpTags.Nodes.Html.Attributes.Values;

internal static class InputAttributes
{
	public static readonly ReadOnlyMemory<byte> Button;
	public static readonly ReadOnlyMemory<byte> Checkbox;
	public static readonly ReadOnlyMemory<byte> Color;
	public static readonly ReadOnlyMemory<byte> PlainDate;
	public static readonly ReadOnlyMemory<byte> PlainDateTime;
	public static readonly ReadOnlyMemory<byte> Email;
	public static readonly ReadOnlyMemory<byte> File;
	public static readonly ReadOnlyMemory<byte> Hidden;
	public static readonly ReadOnlyMemory<byte> Image;
	public static readonly ReadOnlyMemory<byte> PlainMonth;
	public static readonly ReadOnlyMemory<byte> Number;
	public static readonly ReadOnlyMemory<byte> Password;
	public static readonly ReadOnlyMemory<byte> Radio;
	public static readonly ReadOnlyMemory<byte> Range;
	public static readonly ReadOnlyMemory<byte> Reset;
	public static readonly ReadOnlyMemory<byte> Search;
	public static readonly ReadOnlyMemory<byte> Submit;
	public static readonly ReadOnlyMemory<byte> Telephone;
	public static readonly ReadOnlyMemory<byte> PlainTime;
	public static readonly ReadOnlyMemory<byte> Url;
	public static readonly ReadOnlyMemory<byte> PlainWeek;

	public static readonly ReadOnlyMemory<byte> User;
	public static readonly ReadOnlyMemory<byte> Environment;

	public static readonly ReadOnlyMemory<byte> Get;
	public static readonly ReadOnlyMemory<byte> Post;
	public static readonly ReadOnlyMemory<byte> Dialog = ElementNames.Dialog;

	public static readonly ReadOnlyMemory<byte> FormUrlEncoded;
	public static readonly ReadOnlyMemory<byte> FormData;
	public static readonly ReadOnlyMemory<byte> PlainText;

	static InputAttributes()
	{
		ReadOnlyMemory<byte> values = (
			"buttoncheckboxcolordatetime-localemailfilehiddenimagemonthnumber"u8 +
			"passwordradiorangeresetsearchsubmitelurlweekuserenvironmentgetpost"u8 +
			"application/x-www-form-urlencodedmultipart/form-datatext/plain"u8
		).ToArray();

		Button = values.FindSlice("button"u8);
		Checkbox = values.FindSlice("checkbox"u8);
		Color = values.FindSlice("color"u8);
		PlainDate = values.FindSlice("date"u8);
		PlainDateTime = values.FindSlice("datetime-local"u8);
		Email = values.FindSlice("email"u8);
		File = values.FindSlice("file"u8);
		Hidden = values.FindSlice("hidden"u8);
		Image = values.FindSlice("image"u8);
		PlainMonth = values.FindSlice("month"u8);
		Number = values.FindSlice("number"u8);
		Password = values.FindSlice("password"u8);
		Radio = values.FindSlice("radio"u8);
		Range = values.FindSlice("range"u8);
		Reset = values.FindSlice("reset"u8);
		Search = values.FindSlice("search"u8);
		Submit = values.FindSlice("submit"u8);
		Telephone = values.FindSlice("tel"u8);
		PlainTime = values.FindSlice("time"u8);
		Url = values.FindSlice("url"u8);
		PlainWeek = values.FindSlice("week"u8);

		User = values.FindSlice("user"u8);
		Environment = values.FindSlice("environment"u8);

		Get = values.FindSlice("get"u8);
		Post = values.FindSlice("post"u8);

		FormUrlEncoded = values.FindSlice("application/x-www-form-urlencoded"u8);
		FormData = values.FindSlice("multipart/form-data"u8);
		PlainText = values.FindSlice("text/plain"u8);
	}
}
