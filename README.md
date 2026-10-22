# SharpTags

SharpTags is an HTML templating tool inspired by
[ScalaTags](https://com-lihaoyi.github.io/scalatags/), but designed for .NET 11.

It is still a work-in-progress and does not have a stable API, nor is everything
fully implemented and tested, so use in production projects is not recommended
at this stage. That’s not to say you shouldn’t use it: please try it out and
share your feedback!

## What Is SharpTags?

The idea of SharpTags is to construct HTML documents using .NET objects. This
makes it immediately intuitive, because if you can already write C# code, you
can use SharpTags. There’s no templating language you need to learn.

In ScalaTags (the inspiration), everything is immutable, and you construct all
your HTML nodes every time you want to render a page. While SharpTags still lets
you do this, values can also be functions. This avoids memory allocations and
reduces the workload put on the garbage collector.

### Basic Example

```csharp
Div<Elephant> div = new()
{
	Class = "elephant", // Assign a literal value...
	Id = (Func<Elephant, string>)(el => el.Name), // ...or a delegate function!
	Children = // Child nodes can be passed into the constructor instead, if you prefer.
	{
		new Image<Elephant>
		{
			Source = "/images/elephant_icon.svg",
		},
		"Hello from SharpTags!", // You can add both elements and plain text.
	},
};

await using var stdout = Console.OpenStandardOutput();
await div.Render(stdout, new Elephant { Name = "Brinkley" });
```

This outputs the following (formatted for readability):

```html
<div class="elephant" id="Brinkley">
	<img src="/images/elephant_icon.svg"/>
	Hello from SharpTags!
</div>
```

### Built Nodes

Another change from ScalaTags is that SharpTags’ nodes are mutable. If you don’t
need or want them to remain mutable after you create them, they can be turned
into optimized, immutable versions.

```csharp
var builtDiv = div.Build();
```

Building HTML nodes searches through your node tree, rendering any static text
that doesn’t depend on the delegate functions to a single UTF-8 buffer. This is
the recommended way to render HTML in most scenarios.

When you use built nodes, they already know what functions they have and prepare
the text before they begin writing. Mutable nodes, on the other hand, write to
the output as they iterate through their contents. This means that your delegate
functions might be invoked on the wrong `SynchronizationContext` or
`TaskScheduler`, and if one throws an exception the written text will suddenly
stop partway through. Built nodes avoid both of these problems.

## Why Use SharpTags?

SharpTags provides some advantages over Blazor, Razor Views, and Razor Pages:
- Fully CLS compliant.
- Makes no use of reflection, thus supports trimming and Native AOT.
- Completely decoupled from ASP.NET Core: easily use it for non-web tasks such as email.

## Why Not Use SharpTags?

As mentioned above, SharpTags is in an early state, and its API is not stable.
Additionally, it requires .NET 11, which as of writing has not released as
stable.

No NuGet packages are provided at the moment. You could build them yourself, or
use SharpTags as a [Git submodule](https://git-scm.com/book/en/v2/Git-Tools-Submodules).

SharpTags makes heavy use of a source code generator for all the different HTML
elements and their many attributes. The source generator is a console
application, and it currently requires running manually.

## Repo Structure

### SharpTags

Core SharpTags library.

### SharpTags.AspNetCore

Makes using SharpTags in an ASP.NET Core application easier.

### SharpTags.ExampleWebApp

An example of an ASP.NET Core application using SharpTags to serve pages.

### SharpTags.SourceGenerator

The source code generator used to generate all the different HTML elements with their attributes.

### SharpTags.UnitTests

NUnit tests for the core library.

## License

All SharpTags code is available under the Lesser GNU General Public License,
version 3 or later. The terms of the GNU General Public License version 3 may be
found in [the license file](LICENSE), and the additional permissions granted by
version 3 of the Lesser GNU General Public License may be found in
[the lesser license file](LICENSE.LESSER).

The source code and assets of the example web application, **but no other
files**, are also available under the Creative Commons Zero license, version 1.0.
The terms of this license may be found in
[that application’s license file](SharpTags.ExampleWebApp/LICENSE).
