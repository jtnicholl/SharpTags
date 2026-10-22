// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using SharpTags.Built;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

[Experimental("STAGS_001")]
internal sealed class SourceDocumentAttribute<TModel> : HtmlAttribute<TModel>
{
	public SourceDocumentAttributeValue<TModel> Value { get; }

	protected override ReadOnlyMemory<byte> Name => AttributeNames.SourceDocument;

	internal override bool RequiresModel => this.Value.Value is Delegate;

	public SourceDocumentAttribute(SourceDocumentAttributeValue<TModel> value)
	{
		Value = value;
	}

	internal override AttributeResult Evaluate(TModel model)
	{
		throw new NotImplementedException();
	}

	internal override IEnumerable<BuiltMarkupSegment<TModel>> Build()
	{
		throw new NotImplementedException();
	}
}
