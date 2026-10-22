// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text;
using SharpTags.Built;
using SharpTags.Nodes.Html.Elements;

namespace SharpTags.UnitTests.LargeRenderTests;

[TestFixture]
internal sealed class BuiltMarkupRender
{
	private static CancellationToken Cancel => TestContext.CurrentContext.CancellationToken;

	[Test]
	public async Task Render_SimpleHtmlDoc_Succeeds()
	{
		var page = BuiltMarkup.Build(
			new Html<object?>(
				new Head<object?>(),
				new Body<object?>(
					new Header<object?>(new Heading1<object?>("Hello World!")),
					new Main<object?>(new Paragraph<object?>("This is the main element.")),
					new Footer<object?>(new ContentSpan<object?>("This is the footer."))
				)
			)
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, null, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "<html>" +
			"<head></head>" +
			"<body>" +
				"<header><h1>Hello World!</h1></header>" +
				"<main><p>This is the main element.</p></main>" +
				"<footer><span>This is the footer.</span></footer>" +
			"</body>" +
		"</html>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_EscapeCharacters_Escaped()
	{
		var page = BuiltMarkup.Build(
			new Html<object?>(
				new Head<object?>(),
				new Body<object?>(
					new Header<object?>(new Heading1<object?>("Hello \"World\"!")),
					new Main<object?>("This is &<<the>> main element."),
					new Footer<object?>("This is the footer.")
				)
			)
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, null, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "<html>" +
			"<head></head>" +
			"<body>" +
				"<header><h1>Hello &quot;World&quot;!</h1></header>" +
				"<main>This is &amp;&lt;&lt;the&gt;&gt; main element.</main>" +
				"<footer>This is the footer.</footer>" +
			"</body>" +
		"</html>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_RawText_Succeeds()
	{
		var page = BuiltMarkup.Build(
			new Html<object?>(
				new Head<object?>(
					new Script<object?>(
						PlainText.UnsafeRaw("let foo = 4;"),
						PlainText.UnsafeRaw("if (foo === 4 && true)"),
						PlainText.UnsafeRaw("{ alert(\"Hello World!\"); }")
					)
				),
				new Body<object?>(
					new Header<object?>(new Heading1<object?>("Hello World!")),
					new Main<object?>("This is the main element."),
					new Footer<object?>("This is the footer.")
				)
			)
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, null, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "<html>" +
			"<head><script>" +
				"let foo = 4;if (foo === 4 && true){ alert(\"Hello World!\"); }" +
			"</script></head>" +
			"<body>" +
				"<header><h1>Hello World!</h1></header>" +
				"<main>This is the main element.</main>" +
				"<footer>This is the footer.</footer>" +
			"</body>" +
		"</html>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_ConstantAttributes_Succeeds()
	{
		var page = BuiltMarkup.Build(
			new Html<object?>(
				new Head<object?>(),
				new Body<object?>(
					new Header<object?>(new Heading1<object?>("Hello World!"))
					{
						Id = "header",
					},
					new Main<object?>(new Paragraph<object?>("This is the main element."))
					{
						Id = "main",
					},
					new Footer<object?>(
						new ContentSpan<object?>("This is the footer.")
						{
							Id = "foot-span",
						}
					)
				)
			)
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, null, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "<html>" +
			"<head></head>" +
			"<body>" +
				"<header id=\"header\"><h1>Hello World!</h1></header>" +
				"<main id=\"main\"><p>This is the main element.</p></main>" +
				"<footer><span id=\"foot-span\">This is the footer.</span></footer>" +
			"</body>" +
		"</html>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_EscapeAttributes_Succeeds()
	{
		var page = BuiltMarkup.Build(
			new Html<object?>(
				new Head<object?>(),
				new Body<object?>(
					new Header<object?>(new Heading1<object?>("Hello World!"))
					{
						Id = "header",
					},
					new Main<object?>(new Paragraph<object?>("This is the main element."))
					{
						Id = "ma&\"in"u8,
					},
					new Footer<object?>(
						new ContentSpan<object?>("This is the footer.")
						{
							Id = "foot-span",
						}
					)
				)
			)
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, null, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "<html>" +
			"<head></head>" +
			"<body>" +
				"<header id=\"header\"><h1>Hello World!</h1></header>" +
				"<main id=\"ma&amp;&quot;in\"><p>This is the main element.</p></main>" +
				"<footer><span id=\"foot-span\">This is the footer.</span></footer>" +
			"</body>" +
		"</html>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_VoidElements_Succeeds()
	{
		var page = BuiltMarkup.Build(
			new Html<object?>(
				new Head<object?>(),
				new Body<object?>(
					new Header<object?>(new Heading1<object?>("Hello World!")),
					new Main<object?>(
						new Paragraph<object?>(
							"This is the main element.",
							new Break<object?>(),
							"There are multiple lines.",
							new Break<object?>(),
							"Isn't that neat?"
						)
					),
					new Footer<object?>(new ContentSpan<object?>("This is the footer."))
				)
			)
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, null, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "<html>" +
			"<head></head>" +
			"<body>" +
				"<header><h1>Hello World!</h1></header>" +
				"<main><p>" +
					"This is the main element.<br/>There are multiple lines.<br/>Isn't that neat?" +
				"</p></main>" +
				"<footer><span>This is the footer.</span></footer>" +
			"</body>" +
		"</html>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_VoidElementsWithAttributes_Succeeds()
	{
		var page = BuiltMarkup.Build(
			new Html<object?>(
				new Head<object?>(),
				new Body<object?>(
					new Header<object?>(new Heading1<object?>("Hello World!")),
					new Main<object?>(
						new Paragraph<object?>(
							"This is the main element."u8,
							new Break<object?>
							{
								Id = "break-1"
							},
							"There are multiple lines."u8,
							new Break<object?>
							{
								Class = "break-2",
							},
							"Isn't that neat?"
						)
					),
					new Footer<object?>(new ContentSpan<object?>("This is the footer."))
				)
			)
		);

		string result;
		await using (MemoryStream memStream = new())
		{
			await page.Render(memStream, null, Cancel);
			result = Encoding.UTF8.GetString(memStream.ToArray());
		}

		const string expected = "<html>" +
			"<head></head>" +
			"<body>" +
				"<header><h1>Hello World!</h1></header>" +
				"<main>" +
					"<p>" +
						"This is the main element." +
						"<br id=\"break-1\"/>" +
						"There are multiple lines." +
						"<br class=\"break-2\"/>" +
						"Isn't that neat?" +
					"</p>" +
				"</main>" +
				"<footer><span>This is the footer.</span></footer>" +
			"</body>" +
		"</html>";
		Assert.That(result, Is.EqualTo(expected));
	}
}
