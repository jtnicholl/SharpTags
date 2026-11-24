// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Buffers;
using System.Text;

namespace SharpTags.SourceGenerator;

internal sealed class SourceGenerator : IAsyncDisposable
{
	private static readonly ReadOnlyMemory<byte> fileHeader = (
		"// SPDX-License-Identifier: LGPL-3.0-or-later\n"u8 +
		"\n"u8 +
		"#nullable enable\n"u8 +
		"#pragma warning disable STAGS_001\n\n"u8
	).ToArray();
	private static readonly ReadOnlyMemory<byte> usingText = "using "u8.ToArray();
	private static readonly ReadOnlyMemory<byte> namespaceText = "namespace "u8.ToArray();

	private static readonly ReadOnlyMemory<byte> seeAlsoPrefix = "/// <seealso "u8.ToArray();
	private static readonly ReadOnlyMemory<byte> seeAlsoCref = "cref=\""u8.ToArray();
	private static readonly ReadOnlyMemory<byte> seeAlsoHref = "href=\""u8.ToArray();
	private static readonly ReadOnlyMemory<byte> seeAlsoSuffix = "\" />"u8.ToArray();
	private static readonly ReadOnlyMemory<byte> summaryPrefix = "/// <summary>"u8.ToArray();
	private static readonly ReadOnlyMemory<byte> summarySuffix = "</summary>"u8.ToArray();

	private static readonly ReadOnlyMemory<byte> publicText = "public "u8.ToArray();
	private static readonly ReadOnlyMemory<byte> staticText = "static "u8.ToArray();
	private static readonly ReadOnlyMemory<byte> readonlyText = "readonly "u8.ToArray();
	private static readonly ReadOnlyMemory<byte> propertySuffix = " { get; set; }"u8.ToArray();

	private readonly Stream fileStream;
	private readonly ReadOnlyMemory<byte> indentText;
	private readonly bool disposeStream;

	private int indentLevel;
	private bool lineEmpty = true;

	private sealed class BlockScope : IAsyncDisposable
	{
		private readonly SourceGenerator generator;
		private readonly bool semicolon;

		public BlockScope(SourceGenerator generator, bool semicolon)
		{
			this.generator = generator;
			this.semicolon = semicolon;
		}

		public async ValueTask DisposeAsync()
		{
			await this.generator.EndBlock(this.semicolon);
		}
	}

	public SourceGenerator(
		Stream fileStream,
		ReadOnlyMemory<byte> indentText,
		bool disposeStream = false
	)
	{
		this.fileStream = fileStream;
		this.indentText = indentText;
		this.disposeStream = disposeStream;
	}

	public async Task WriteHeader()
	{
		await this.fileStream.WriteAsync(fileHeader);
		this.lineEmpty = true;
	}

	public async Task WriteUsing(ReadOnlyMemory<byte> @namespace)
	{
		await this.fileStream.WriteAsync(usingText);
		await this.fileStream.WriteAsync(@namespace);
		this.fileStream.WriteByte((byte)';');
		await NewLine();
	}

	public async Task WriteUsings(params IEnumerable<ReadOnlyMemory<byte>> namespaces)
	{
		foreach (var ns in namespaces)
		{
			await WriteUsing(ns);
		}

		await NewLine();
	}

	public async Task DeclareNamespace(ReadOnlyMemory<byte> @namespace)
	{
		await this.fileStream.WriteAsync(namespaceText);
		await this.fileStream.WriteAsync(@namespace);
		this.fileStream.WriteByte((byte)';');
		await NewLine();
		await NewLine();
	}

	public async Task WriteSummary(ReadOnlyMemory<byte> summary)
	{
		await Write(summaryPrefix);
		await NewLine();
		await Write(summaryPrefix[..4]);
		await this.fileStream.WriteAsync(summary);
		await NewLine();
		await Write(summaryPrefix[..4]);
		await this.fileStream.WriteAsync(summarySuffix);
		await NewLine();
	}

	public async Task WriteSummary(ReadOnlySequence<byte> summary)
	{
		await Write(summaryPrefix);
		foreach (var fragment in summary)
		{
			await this.fileStream.WriteAsync(fragment);
		}
		await this.fileStream.WriteAsync(summarySuffix);
		await NewLine();
	}

	public async Task WriteSummary(params IEnumerable<ReadOnlyMemory<byte>> summary)
	{
		await Write(summaryPrefix);
		await NewLine();
		await Write(summaryPrefix[..4]);
		foreach (var fragment in summary)
		{
			await this.fileStream.WriteAsync(fragment);
		}
		await NewLine();
		await Write(summaryPrefix[..4]);
		await this.fileStream.WriteAsync(summarySuffix);
		await NewLine();
	}

	public Task WriteSummary(string summary) => WriteSummary(Encoding.UTF8.GetBytes(summary));

	public Task WriteProperty(
		ReadOnlyMemory<byte> type,
		string name,
		bool mutable = true
	)
	{
		return WriteProperty(type, Encoding.UTF8.GetBytes(name), mutable);
	}

	public async Task WriteProperty(
		ReadOnlyMemory<byte> type,
		ReadOnlyMemory<byte> name,
		bool mutable = true
	)
	{
		await EnsureIndent();
		await this.fileStream.WriteAsync(publicText);
		await this.fileStream.WriteAsync(type);
		this.fileStream.WriteByte((byte)' ');
		await this.fileStream.WriteAsync(name);
		if (mutable)
		{
			await this.fileStream.WriteAsync(propertySuffix);
		}
		else
		{
			await this.fileStream.WriteAsync(propertySuffix[..8]);
			this.fileStream.WriteByte((byte)'}');
		}
		await NewLine();
	}

	public Task WriteReadonlyStaticField(string type, ReadOnlyMemory<byte> name)
	{
		return WriteReadonlyStaticField(Encoding.UTF8.GetBytes(type), name);
	}

	public Task WriteReadonlyStaticField(string type, string name)
	{
		return WriteReadonlyStaticField(Encoding.UTF8.GetBytes(type), Encoding.UTF8.GetBytes(name));
	}

	public async Task WriteReadonlyStaticField(ReadOnlyMemory<byte> type, ReadOnlyMemory<byte> name)
	{
		await EnsureIndent();
		await this.fileStream.WriteAsync(publicText);
		await this.fileStream.WriteAsync(staticText);
		await this.fileStream.WriteAsync(readonlyText);
		await this.fileStream.WriteAsync(type);
		this.fileStream.WriteByte((byte)' ');
		await this.fileStream.WriteAsync(name);
		this.fileStream.WriteByte((byte)';');
		await NewLine();
	}

	private async Task EnsureIndent()
	{
		if (this.lineEmpty)
		{
			for (int i = 0; i < this.indentLevel; i++)
			{
				await this.fileStream.WriteAsync(this.indentText);
			}
		}
	}

	public async Task NewLine()
	{
		this.fileStream.WriteByte((byte)'\n');
		this.lineEmpty = true;
	}

	public async Task<IAsyncDisposable> Block(bool semicolon = false)
	{
		await StartBlock();
		return new BlockScope(this, semicolon);
	}

	private async Task StartBlock()
	{
		await NewLine();
		await EnsureIndent();
		this.fileStream.WriteByte((byte)'{');
		this.indentLevel++;
		await NewLine();
	}

	private async Task EndBlock(bool semicolon)
	{
		this.indentLevel--;
		if (!this.lineEmpty)
		{
			await NewLine();
		}
		await EnsureIndent();
		this.fileStream.WriteByte((byte)'}');
		if (semicolon)
		{
			this.fileStream.WriteByte((byte)';');
		}
		this.fileStream.WriteByte((byte)'\n');
	}

	public async Task SeeAlsoCode(ReadOnlyMemory<byte> codeItem)
	{
		await this.fileStream.WriteAsync(seeAlsoPrefix);
		await this.fileStream.WriteAsync(seeAlsoCref);
		await this.fileStream.WriteAsync(codeItem);
		await this.fileStream.WriteAsync(seeAlsoSuffix);
		await NewLine();
	}

	public Task SeeAlsoCode(string codeItem) => SeeAlsoCode(Encoding.UTF8.GetBytes(codeItem));

	public async Task SeeAlsoUrl(params IEnumerable<ReadOnlyMemory<byte>> href)
	{
		await this.Write(seeAlsoPrefix);
		await this.fileStream.WriteAsync(seeAlsoHref);
		foreach (var part in href)
		{
			await this.fileStream.WriteAsync(part);
		}
		await this.fileStream.WriteAsync(seeAlsoSuffix);
		await NewLine();
	}

	public Task SeeAlsoUrl(string href) => SeeAlsoUrl(Encoding.UTF8.GetBytes(href));

	public async Task Write(ReadOnlyMemory<byte> utf8Text)
	{
		await EnsureIndent();
		await this.fileStream.WriteAsync(utf8Text);
		this.lineEmpty = false;
	}

	public Task Write(string utf16Text) => Write(Encoding.UTF8.GetBytes(utf16Text));

	public ValueTask DisposeAsync()
	{
		return this.disposeStream ? this.fileStream.DisposeAsync() : ValueTask.CompletedTask;
	}
}
