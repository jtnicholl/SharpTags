// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;

namespace SharpTags.Nodes.Html.Attributes;

internal interface ITernaryAttribute
{
	static abstract ReadOnlyMemory<byte> TrueValue { get; }
	static abstract ReadOnlyMemory<byte> FalseValue { get; }

	[Pure]
	static abstract AttributeResult Evaluate(bool? value);
}
