// SPDX-License-Identifier: LGPL-3.0-or-later

using System.Net;
using Microsoft.AspNetCore.Http;

namespace SharpTags.AspNetCore;

/// <summary>
/// A rendered HTML page ready to be served as an HTTP response.
/// </summary>
public sealed class RenderedPage :
	IResult,
	IContentTypeHttpResult,
	IStatusCodeHttpResult,
	IDisposable
{
	/// <summary>
	/// The HTTP status code used for the response.
	/// </summary>
	public HttpStatusCode StatusCode { get; }

	int? IStatusCodeHttpResult.StatusCode => (int)StatusCode;

	/// <summary>
	/// The value used for the <c>Content-Type</c> response header.
	/// </summary>
	public string ContentType { get; }

	/// <summary>
	/// The length in bytes of the response body.
	/// </summary>
	public long ContentLength { get; }

	private readonly PooledList<ReadOnlyMemory<byte>> outputText;

	internal RenderedPage(
		PooledList<ReadOnlyMemory<byte>> outputText,
		HttpStatusCode statusCode,
		string contentType,
		long contentLength
	)
	{
		this.outputText = outputText;
		this.StatusCode = statusCode;
		this.ContentType = contentType;
		this.ContentLength = contentLength;
	}

	/// <inheritdoc />
	public Task ExecuteAsync(HttpContext httpContext)
	{
		ArgumentNullException.ThrowIfNull(httpContext);
		ObjectDisposedException.ThrowIf(this.outputText.Disposed, this);
		return ExecuteAsyncImplementation(httpContext);
	}

	private async Task ExecuteAsyncImplementation(HttpContext httpContext)
	{
		httpContext.Response.StatusCode = (int)StatusCode;
		httpContext.Response.ContentType = ContentType;
		httpContext.Response.ContentLength = ContentLength;
		if (httpContext.Response.Headers.CacheControl.All(string.IsNullOrEmpty))
		{
			httpContext.Response.Headers.CacheControl = "private, no-store";
		}

		foreach (var text in this.outputText)
		{
			await httpContext.Response.BodyWriter
				.WriteAsync(text, httpContext.RequestAborted)
				.ConfigureAwait(false);
		}
	}

	/// <inheritdoc />
	public void Dispose()
	{
		this.outputText.Dispose();
	}
}
