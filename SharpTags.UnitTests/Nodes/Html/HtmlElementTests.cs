// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using SharpTags.Nodes.Html.Elements;
using SharpTags.UnitTests.TestModels;

namespace SharpTags.UnitTests.Nodes.Html;

[TestFixture]
internal sealed class HtmlElementTests
{
	private static CancellationToken Cancel => TestContext.CurrentContext.CancellationToken;

	private static IEnumerable<TestCaseData> NoModelRenderTestCases() =>
	[
		new(
			new Div<object?>(new ContentSpan<object?>(), new ContentSpan<object?>()),
			"<div><span></span><span></span></div>"
		),
		new(
			new Div<object?>(),
			"<div></div>"
		),
		new(
			new Break<object?>(),
			"<br/>"
		),
		new(
			new Div<object?>
			{
				Id = "id",
			},
			"<div id=\"id\"></div>"
		),
		new(
			new Div<object?>
			{
				TabIndex = 1,
			},
			"<div tabindex=\"1\"></div>"
		),
		new(
			new Div<object?>
			{
				AccessKey = 'C',
			},
			"<div accesskey=\"C\"></div>"
		),
		new(
			new Div<object?>
			{
				AutoFocus = true,
			},
			"<div autofocus></div>"
		),
		new(
			new Div<object?>
			{
				AutoFocus = false,
			},
			"<div></div>"
		),
		new(
			new Div<object?>
			{
				Draggable = true,
			},
			"<div draggable=\"true\"></div>"
		),
		new(
			new Div<object?>
			{
				Draggable = false,
			},
			"<div draggable=\"false\"></div>"
		),
		new(
			new Inserted<object?>
			{
				DateTime = new DateOnly(2026, 1, 1),
			},
			"<ins datetime=\"2026-01-01\"></ins>"
		),
		new(
			new Inserted<object?>
			{
				DateTime = new DateTime(2026, 1, 1),
			},
			"<ins datetime=\"2026-01-01T00:00\"></ins>"
		),
	];

	[Test]
	public void RequiresModel_NodeWithModelAttribute_IsTrue()
	{
		var node = new Div<Elephant>
		{
			Id = (Func<Elephant, string>)(el => el.Name),
		};

		Assert.That(node.RequiresModel, Is.True);
	}

	[Test]
	public void RequiresModel_NodeWithModelTextChild_IsTrue()
	{
		var node = new Div<Elephant>((Func<Elephant, string>)(el => el.Name));

		Assert.That(node.RequiresModel, Is.True);
	}

	[Test]
	public void RequiresModel_NodeWithAttribute_IsFalse()
	{
		var node = new Div<object?>
		{
			Id = "id",
		};

		Assert.That(node.RequiresModel, Is.False);
	}

	[Test]
	public void RequiresModel_NodeWithTextChild_IsFalse()
	{
		var node = new Div<object?>("Text");

		Assert.That(node.RequiresModel, Is.False);
	}

	[TestCaseSource(nameof(NoModelRenderTestCases))]
	public async Task RenderStream(HtmlElement<object?> node, string expected)
	{
		await using MemoryStream memStream = new();

		await node.Render(memStream, null, Cancel);
		var result = Encoding.UTF8.GetString(memStream.ToArray());

		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCaseSource(nameof(NoModelRenderTestCases))]
	public async Task RenderPipeWriter(HtmlElement<object?> node, string expected)
	{
		await using MemoryStream memStream = new();

		await node.Render(PipeWriter.Create(memStream), null, Cancel);
		var result = Encoding.UTF8.GetString(memStream.ToArray());

		Assert.That(result, Is.EqualTo(expected));
	}

	[TestCaseSource(nameof(NoModelRenderTestCases))]
	public void RenderBufferWriter(HtmlElement<object?> node, string expected)
	{
		ArrayBufferWriter<byte> buffer = new();

		node.Render(buffer, null);
		var result = Encoding.UTF8.GetString(buffer.WrittenSpan);

		Assert.That(result, Is.EqualTo(expected));
	}
}
