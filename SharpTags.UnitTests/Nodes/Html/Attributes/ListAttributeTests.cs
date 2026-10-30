// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes.Html.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.UnitTests.Nodes.Html.Attributes;

[TestFixture]
internal sealed class ListAttributeTests
{
	private static IEnumerable<TestCaseData> EvaluateTestCases() =>
	[
		new(
			new ListAttributeValue<IEnumerable<string>>(new PlainText("D")),
			new AttributeResult("D"u8.ToArray())
		),
		new(
			new ListAttributeValue<IEnumerable<string>>(_ => new PlainText("E")),
			new AttributeResult("E"u8.ToArray())
		),
		new(
			new ListAttributeValue<IEnumerable<string>>(_ => (PlainText?)"F"),
			new AttributeResult("F"u8.ToArray())
		),
		new(
			new ListAttributeValue<IEnumerable<string>>(ie => ie.Select(s => new PlainText(s))),
			new AttributeResult("A B C"u8.ToArray())
		),
		new(
			new ListAttributeValue<IEnumerable<string>>(_ => (IEnumerable<PlainText>?)null),
			AttributeResult.NotPresent
		),
		new(
			new ListAttributeValue<IEnumerable<string>>(_ => "AB C"),
			new AttributeResult("AB C"u8.ToArray())
		),
		new(
			new ListAttributeValue<IEnumerable<string>>(_ => (string?)null),
			AttributeResult.NotPresent
		),
		new(
			new ListAttributeValue<IEnumerable<string>>(ie => ie),
			new AttributeResult("A B C"u8.ToArray())
		),
		new(
			new ListAttributeValue<IEnumerable<string>>(_ => (IEnumerable<string>?)null),
			AttributeResult.NotPresent
		),
	];

	[TestCaseSource(nameof(EvaluateTestCases))]
	public void Evaluate(ListAttributeValue<IEnumerable<string>> value, AttributeResult expected)
	{
		var attribute = new ListAttribute<IEnumerable<string>>("list"u8.ToArray(), value, " "u8);

		var actual = attribute.Evaluate(["A", "B", "C"]);

		Assert.That(actual, Is.EqualTo(expected));
	}
}
