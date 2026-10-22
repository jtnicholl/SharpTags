using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

internal sealed partial class CharacterSetAttribute
{
	[Pure]
	public static AttributeResult Evaluate(CharacterSet value)
	{
		return value switch
		{
			CharacterSet.Utf8 => new(MiscAttributeValues.Utf8),
			_ => AttributeResult.NotPresent,
		};
	}
}
