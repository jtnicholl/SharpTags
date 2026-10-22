// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class LinkAsAttribute
{
	[Pure]
	public static AttributeResult Evaluate(LinkAs value)
	{
		return value switch
		{
			LinkAs.AudioWorklet => new(MiscAttributeValues.AudioWorklet),
			LinkAs.Fetch => new(MiscAttributeValues.Fetch),
			LinkAs.Font => new(MiscAttributeValues.Font),
			LinkAs.Image => new(MiscAttributeValues.Image),
			LinkAs.Json => new(MiscAttributeValues.Json),
			LinkAs.PaintWorklet => new(MiscAttributeValues.PaintWorklet),
			LinkAs.Script => new(MiscAttributeValues.Script),
			LinkAs.ServiceWorker => new(MiscAttributeValues.ServiceWorker),
			LinkAs.SharedWorker => new(MiscAttributeValues.SharedWorker),
			LinkAs.Style => new(MiscAttributeValues.Style),
			LinkAs.Text => new(MiscAttributeValues.Text),
			LinkAs.Track => new(MiscAttributeValues.Track),
			LinkAs.Worker => new(MiscAttributeValues.Worker),
			_ => AttributeResult.NotPresent,
		};
	}
}
