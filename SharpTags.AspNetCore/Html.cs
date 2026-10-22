// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.AspNetCore;

internal static class Html
{
	public static readonly PlainText DocType = PlainText.UnsafeRaw("<!DOCTYPE html>"u8);
	public static readonly PlainText Newline = PlainText.UnsafeRaw("\n"u8);
}
