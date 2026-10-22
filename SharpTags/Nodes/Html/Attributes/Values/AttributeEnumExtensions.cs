// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes.Html.Elements;

namespace SharpTags.Nodes.Html.Attributes.Values;

/// <summary>
/// Extensions for attribute enums.
/// </summary>
public static class AttributeEnumExtensions
{
	extension(Sandbox)
	{
		/// <summary>
		/// A value for <see cref="IFrame{TModel}.Sandbox" /> which provides all permissions.
		/// </summary>
		public static IEnumerable<Sandbox> AllPermissions => [];
	}
}
