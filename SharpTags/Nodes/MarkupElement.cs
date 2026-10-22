// SPDX-License-Identifier: LGPL-3.0-or-later

namespace SharpTags.Nodes;

internal static class MarkupElement
{
	public static readonly ReadOnlyMemory<byte> LessThan;
	public static readonly ReadOnlyMemory<byte> GreaterThan;
	public static readonly ReadOnlyMemory<byte> Slash;
	public static readonly ReadOnlyMemory<byte> SelfClose;

	static MarkupElement()
	{
		ReadOnlyMemory<byte> chars = "< />=\""u8.ToArray();
		LessThan = chars[..1];
		GreaterThan = chars[3..4];
		Slash = chars[2..3];
		SelfClose = chars[2..4];
	}
}
