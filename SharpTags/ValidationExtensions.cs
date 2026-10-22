using System.Runtime.CompilerServices;

namespace SharpTags;

internal static class ValidationExtensions
{
	extension(ArgumentNullException)
	{
		public static void ThrowIfContainsNull<T>(
			IEnumerable<T> enumerable,
			[CallerArgumentExpression(nameof(enumerable))] string? argument = null
		) where T : class
		{
			if (enumerable.Contains(null))
			{
				throw new ArgumentNullException(argument);
			}
		}

		public static void ThrowIfContainsNull<T>(
			ReadOnlySpan<T> span,
			[CallerArgumentExpression(nameof(span))] string? argument = null
		) where T : class
		{
			if (span.Contains(null))
			{
				throw new ArgumentNullException(argument);
			}
		}
	}
}
