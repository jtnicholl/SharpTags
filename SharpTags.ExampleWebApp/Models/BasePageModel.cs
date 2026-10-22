using System.Security.Claims;

namespace SharpTags.ExampleWebApp.Models;

internal class BasePageModel
{
	/// <summary>
	/// In a real application, you'd probably populate this from a <see cref="ClaimsPrincipal" />.
	/// </summary>
	public required string Username { get; init; }
}
