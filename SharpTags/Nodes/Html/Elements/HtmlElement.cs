// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Collections;
using SharpTags.Built;
using SharpTags.Nodes.Html.Attributes;

namespace SharpTags.Nodes.Html.Elements;

/// <summary>
/// Base class for all HTML elements.
/// </summary>
/// <typeparam name="TModel">
/// The type of model, which can be used to vary rendered output.
/// </typeparam>
partial class HtmlElement<TModel> : MarkupElement<TModel>
{
	/// <summary>
	/// If this element does not allow children.
	/// </summary>
	/// <seealso href="https://developer.mozilla.org/en-US/docs/Glossary/Void_element" />
	public bool IsVoid => SelfClosing;

	internal override bool RequiresModel
	{
		get
		{
			return HtmlAttributes.Any(a => a.RequiresModel) || base.RequiresModel;
		}
	}

	private protected abstract IEnumerable<HtmlAttribute<TModel>> HtmlAttributes { get; }

	private protected override IEnumerable<ITextRender<TModel>> Attributes => HtmlAttributes;

	private protected HtmlElement(
		ReadOnlyMemory<byte> name,
		bool isVoid,
		IList<MarkupNode<TModel>> children
	) : base(name, isVoid, children) { }

	private protected override IEnumerable<BuiltMarkupSegment<TModel>> BuildImplementation(
		Dictionary<object, IEnumerable> cache,
		HashSet<object> recursionCheck,
		bool capContext
	)
	{
		yield return MarkupElement.LessThan;
		yield return this.name;
		foreach (var attribute in HtmlAttributes)
		{
			foreach (var result in attribute.Build())
			{
				yield return result;
			}
		}

		if (IsVoid)
		{
			yield return MarkupElement.SelfClose;
		}
		else
		{
			yield return MarkupElement.GreaterThan;

			foreach (var child in this.Children)
			{
				foreach (var result in child.Build(cache, recursionCheck, capContext))
				{
					yield return result;
				}
			}

			yield return MarkupElement.LessThan;
			yield return MarkupElement.Slash;
			yield return this.name;
			yield return MarkupElement.GreaterThan;
		}
	}
}
