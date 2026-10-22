// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes;

namespace SharpTags;

/// <summary>
/// Synchronous iterator for a <see cref="ForEachNode{TModel,TIterated}" />.
/// </summary>
/// <typeparam name="TModel">The type which provides the iterator.</typeparam>
/// <typeparam name="TIterated">The iterated type.</typeparam>
public delegate IEnumerable<TIterated> ModelEnumerator<in TModel, out TIterated>(TModel model);
