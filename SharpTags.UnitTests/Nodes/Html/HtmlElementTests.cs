// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Text;
using SharpTags.Nodes.Html.Elements;
using SharpTags.UnitTests.TestModels;

namespace SharpTags.UnitTests.Nodes.Html;

[TestFixture]
internal sealed class HtmlElementTests
{
	private static CancellationToken Cancel => TestContext.CurrentContext.CancellationToken;

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

	[Test]
	public async Task Render_ElementWithChildren_IncludesChildren()
	{
		var node = new Div<object?>(new ContentSpan<object?>(), new ContentSpan<object?>());

		string result;
		await using (var stream = new MemoryStream())
		{
			await node.Render(stream, null, Cancel);
			result = Encoding.UTF8.GetString(stream.ToArray());
		}

		const string expected = "<div><span></span><span></span></div>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_NonVoidElementNoChildren_HasClosingTag()
	{
		var node = new Div<object?>();

		string result;
		await using (var stream = new MemoryStream())
		{
			await node.Render(stream, null, Cancel);
			result = Encoding.UTF8.GetString(stream.ToArray());
		}

		const string expected = "<div></div>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_VoidElement_IsSelfClosing()
	{
		var node = new Break<object?>();

		string result;
		await using (var stream = new MemoryStream())
		{
			await node.Render(stream, null, Cancel);
			result = Encoding.UTF8.GetString(stream.ToArray());
		}

		const string expected = "<br/>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_WithStringAttribute_IncludesAttribute()
	{
		var node = new Div<object?>
		{
			Id = "id",
		};

		string result;
		await using (var stream = new MemoryStream())
		{
			await node.Render(stream, null, Cancel);
			result = Encoding.UTF8.GetString(stream.ToArray());
		}

		const string expected = "<div id=\"id\"></div>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_WithIntegerAttribute_IncludesAttribute()
	{
		var node = new Div<object?>
		{
			TabIndex = 1,
		};

		string result;
		await using (var stream = new MemoryStream())
		{
			await node.Render(stream, null, Cancel);
			result = Encoding.UTF8.GetString(stream.ToArray());
		}

		const string expected = "<div tabindex=\"1\"></div>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_WithCharAttribute_IncludesAttribute()
	{
		var node = new Div<object?>
		{
			AccessKey = 'C',
		};

		string result;
		await using (var stream = new MemoryStream())
		{
			await node.Render(stream, null, Cancel);
			result = Encoding.UTF8.GetString(stream.ToArray());
		}

		const string expected = "<div accesskey=\"C\"></div>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_WithBooleanAttributeTrue_IncludesAttribute()
	{
		var node = new Div<object?>
		{
			AutoFocus = true,
		};

		string result;
		await using (var stream = new MemoryStream())
		{
			await node.Render(stream, null, Cancel);
			result = Encoding.UTF8.GetString(stream.ToArray());
		}

		const string expected = "<div autofocus></div>";
		Assert.That(result, Is.EqualTo(expected));
	}

	[Test]
	public async Task Render_WithBooleanAttributeFalse_ExcludesAttribute()
	{
		var node = new Div<object?>
		{
			AutoFocus = false,
		};

		string result;
		await using (var stream = new MemoryStream())
		{
			await node.Render(stream, null, Cancel);
			result = Encoding.UTF8.GetString(stream.ToArray());
		}

		const string expected = "<div></div>";
		Assert.That(result, Is.EqualTo(expected));
	}
}
