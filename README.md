# HTML as code

I don't like templates.
I prefer components.

This is an attempt to generate HTML by writing C# code and use functions as components.

## Usage

See the tests.

Some examples:

```csharp
var html = H("html",
    H("head", H("title", "Hello World!"),
    H("body",
        H("h1", "Hello World!"),
        H("p", "This is a paragraph."));
Console.WriteLine(html.Render());
```

This will create the following HTML (not indented):

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

Then you can call this function in your HTML generation code:

```csharp
var html = H("html",
    H("head", H("title", "Hello World!")),
    MakeBody());
```

