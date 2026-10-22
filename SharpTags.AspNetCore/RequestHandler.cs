// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Diagnostics.Contracts;
using System.Net;
using Microsoft.AspNetCore.Http;
using SharpTags.Built;
using SharpTags.Nodes.Html.Elements;

namespace SharpTags.AspNetCore;

/// <summary>
/// Serves HTML pages as HTTP responses.
/// </summary>
/// <typeparam name="TModel">The type of model used to vary the contents of the page.</typeparam>
public sealed class RequestHandler<TModel>
{
	/// <summary>
	/// The HTTP status code used for successful responses.
	/// </summary>
	public HttpStatusCode StatusCode { get; }

	/// <summary>
	/// The value used for the <c>Content-Type</c> header in successful responses.
	/// </summary>
	public string ContentType { get; }

	private readonly BuiltMarkup<TModel> page;

	/// <summary>
	/// Construct a new <see cref="RequestHandler{TModel}" />.
	/// </summary>
	/// <param name="page">The <see cref="Html{TModel}" /> to use for the page.</param>
	/// <param name="statusCode">
	/// The response status code to use. Defaults to <see cref="HttpStatusCode.OK" />.
	/// </param>
	/// <param name="contentType">
	/// The value to use for the <c>Content-Type</c> header. Defaults to
	/// <c>text/html; charset=utf-8</c>.
	/// </param>
	/// <exception cref="ArgumentException">If <paramref name="contentType" /> is empty.</exception>
	public RequestHandler(
		Html<TModel> page,
		HttpStatusCode statusCode = HttpStatusCode.OK,
		string contentType = "text/html; charset=utf-8"
	)
	{
		ArgumentNullException.ThrowIfNull(page);
		ArgumentNullException.ThrowIfNullOrEmpty(contentType);
		this.StatusCode = statusCode;
		this.ContentType = contentType;
		this.page = BuiltMarkup.Build(Html.DocType, page, Html.Newline);
	}

	/// <summary>
	/// Render the page using the <paramref name="model" /> and serve it as a response.
	/// </summary>
	/// <param name="context">The <see cref="HttpContext" /> for the response.</param>
	/// <param name="model">The model used to render the page.</param>
	public async Task ServePage(HttpContext context, TModel model)
	{
		context.RequestAborted.ThrowIfCancellationRequested();

		using var outputText = new PooledList<ReadOnlyMemory<byte>>(this.page.segments.Length * 2);

		var length = this.page.Evaluate(model, outputText);

		context.Response.StatusCode = (int)StatusCode;
		context.Response.ContentType = ContentType;
		context.Response.ContentLength = length;

		foreach (var text in outputText)
		{
			await context.Response.BodyWriter
				.WriteAsync(text, context.RequestAborted)
				.ConfigureAwait(false);
		}
	}

	/// <summary>
	/// Render the page using the <paramref name="model" />, returning an
	/// implementation of <see cref="IResult" /> which can be used to serve it
	/// as a response.
	/// </summary>
	/// <param name="model">The model used to render the page.</param>
	/// <returns>A <see cref="RenderedPage" /> which can be used to serve a response.</returns>
	[Pure]
	public RenderedPage Page(TModel model)
	{
		var outputText = new PooledList<ReadOnlyMemory<byte>>(this.page.segments.Length * 2);
		try
		{
			var length = this.page.Evaluate(model, outputText);
			return new(outputText, StatusCode, ContentType, length);
		}
		catch
		{
			outputText.Dispose();
			throw;
		}
	}
}
