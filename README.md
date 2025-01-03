# HTML as code

A HTML generation library based on the belief that components written in a general purpose language are better than templates.

## Usage

See the tests.

Some examples:

```csharp
var html = H("html",
    H("head", H("title", "Hello World!"),
    H("body",
        H("h1", "Hello World!"),
        H("p", "This is a paragraph."));
Console.WriteLine(html.RenderPretty(maxColumn: 40));
```

This will create the following HTML (notice the `maxColumn` parameter which is set to a rather low value to trigger this particular formatting - a larger `maxColumn` value will result in output that has longer, fewer lines):

```html
<!DOCTYPE html>
<html>
    <head>
        <title>Hello World!</title>
    </head>
    <body>
        <h1>Hello World!</h1>
        <p>This is a paragraph.</p>
    </body>
</html>
```

How to use components?
Just write a function that returns the `body` element:

```csharp
Element MakeBody() => H("body", H("h1", "Hello World!"), H("p", "This is a paragraph."));
```

(notice the return type of `MakeBody`, this is the `Element` provided by the this library)

Then you can call this function in your HTML generation code:

```csharp
var html = H("html",
    H("head", H("title", "Hello World!")),
    MakeBody());
```

This is where the "as code" part starts to matter.
We can parametrize the function acting as components.

```csharp
Element MakeBody(string title, string content) => H("body", H("h1", title), H("p", content));
```

Using this component gives us:

```csharp
var html = H("html",
    H("head", H("title", "Hello World!")),
    MakeBody("Hello World!", "This is a paragraph."));
```

You may have noticed that text nodes are specified as strings.
These strings are escaped by the library.

To insert verbatim HTML, use the `R` function:

```csharp
var html = H("html",
    H("head", H("title", "Hello World!")),
    H("body", H("h1", "Hello World!"), R("<p>This is a paragraph.</p>")));
```

If you need to flatten lists of elements, use the `F` function:

```csharp
var html = H("html",
    H("head", H("title", "Hello World!")),
    H("body",
        H("h1", "Hello World!"),
        F(H("p", "This is a paragraph."), H("p", "Another paragraph."))));
```

... or you could just use a component that returns a `IEnumerable<Element>` instead of `Element`.

`F` is just a helper function to quickly create a list of elements.
