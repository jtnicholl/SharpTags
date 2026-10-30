// SPDX-License-Identifier: LGPL-3.0-or-later

using SharpTags.Nodes.Html.Attributes;
using SharpTags.Nodes.Html.Attributes.Values;

namespace SharpTags.UnitTests.Nodes.Html.Attributes;

[TestFixture]
internal sealed class PlainDateTimeAttributeTests
{
	private static IEnumerable<TestCaseData> EvaluateTestCases() =>
	[
		new(
			new PlainDateTimeAttributeValue<DateOnly>(new DateOnly(2026, 1, 1)),
			new AttributeResult("2026-01-01"u8.ToArray())
		),
		new(
			new PlainDateTimeAttributeValue<DateOnly>(new DateTime(2026, 1, 1)),
			new AttributeResult("2026-01-01T00:00"u8.ToArray())
		),
		new(
			new PlainDateTimeAttributeValue<DateOnly>(e => e),
			new AttributeResult("2004-08-30"u8.ToArray())
		),
		new(
			new PlainDateTimeAttributeValue<DateOnly>(e => e.ToDateTime(default)),
			new AttributeResult("2004-08-30T00:00"u8.ToArray())
		),
		new(
			new PlainDateTimeAttributeValue<DateOnly>(_ => (DateOnly?)null),
			AttributeResult.NotPresent
		),
		new(
			new PlainDateTimeAttributeValue<DateOnly>(_ => (DateTime?)null),
			AttributeResult.NotPresent
		),
		new(
			default(PlainDateTimeAttributeValue<DateOnly>),
			AttributeResult.NotPresent
		),
	];

	[TestCaseSource(nameof(EvaluateTestCases))]
	public void Evaluate(
		PlainDateTimeAttributeValue<DateOnly> value,
		AttributeResult expected
	)
	{
		PlainDateTimeAttribute<DateOnly> attribute = new(value);

		var actual = attribute.Evaluate(new(2004, 8, 30));

		Assert.That(actual, Is.EqualTo(expected));
	}
}
