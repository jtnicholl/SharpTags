// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.Nodes.Html.Attributes;

partial class SandboxAttribute
{
	[Pure]
	public static AttributeResult Evaluate(Sandbox value)
	{
		return value switch
		{
			Sandbox.AllowDownloads => new(MiscAttributeValues.AllowDownloads),
			Sandbox.AllowForms => new(MiscAttributeValues.AllowForms),
			Sandbox.AllowModals => new(MiscAttributeValues.AllowModals),
			Sandbox.AllowOrientationLock => new(MiscAttributeValues.AllowOrientationLock),
			Sandbox.AllowPointerLock => new(MiscAttributeValues.AllowPointerLock),
			Sandbox.AllowPopups => new(MiscAttributeValues.AllowPopups),
			Sandbox.AllowPopupsToEscapeSandbox => new(MiscAttributeValues.AllowPopupsToEscapeSandbox),
			Sandbox.AllowPresentation => new(MiscAttributeValues.AllowPresentation),
			Sandbox.AllowSameOrigin => new(MiscAttributeValues.AllowSameOrigin),
			Sandbox.AllowScripts => new(MiscAttributeValues.AllowScripts),
			Sandbox.AllowTopNavigation => new(MiscAttributeValues.AllowTopNavigation),
			Sandbox.AllowTopNavigationByUserActivation =>new(
				MiscAttributeValues
					.AllowTopNavigationByUserActivation
			),
			Sandbox.AllowTopNavigationToCustomProtocols => new(
				MiscAttributeValues
					.AllowTopNavigationToCustomProtocols
			),
			_ => AttributeResult.NotPresent,
		};
	}
}
