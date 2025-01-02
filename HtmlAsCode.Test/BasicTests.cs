namespace HtmlAsCode.Test;

using FluentAssertions;
using static HtmlAsCode.Renderer;

public class BasicTests
{
    [Fact]
    public void SimplestHtmlDocument()
    {
        var html = H("html");
        html.Render().Should().Be("<!DOCTYPE html><html></html>");
    }

    [Fact]
    public void WithHeadAndBody()
    {
        var html = H("html", H("head"), H("body"));
        html.Render().Should().Be("<!DOCTYPE html><html><head></head><body></body></html>");
    }

    [Fact]
    public void WithAttributes()
    {
        var html = H(
            "html",
            A("lang", "en"),
            H("head", H("title", "Test Title"), H("script", A("src", "script.js"))),
            H("body")
        );
        html.Render()
            .Should()
            .Be(
                "<!DOCTYPE html><html lang=\"en\"><head><title>Test Title</title><script src=\"script.js\"></script></head><body></body></html>"
            );
    }

    [Fact]
    public void WithText()
    {
        var html = H("html", H("body", "Hello, World!"));
        html.Render().Should().Be("<!DOCTYPE html><html><body>Hello, World!</body></html>");
    }

    [Fact]
    public void WithRawHtml()
    {
        var html = H("html", R("<body>Hello, World!</body>"));
        html.Render().Should().Be("<!DOCTYPE html><html><body>Hello, World!</body></html>");
    }

    [Fact]
    public void WithVoidElement()
    {
        var html = H("input");
        html.Render(false).Should().Be("<input>");
    }

    [Fact]
    public void WithCollection()
    {
        var html = H(
            "div",
            new INode[] { new Text("Hello, ") },
            new INode[] { new Text("World!") }
        );
        html.Render(false).Should().Be("<div>Hello, World!</div>");
    }

    [Fact]
    public void WithExplicitText()
    {
        var html = H("div", new Text("Hello, World!"));
        html.Render(false).Should().Be("<div>Hello, World!</div>");
    }

    [Fact]
    public void WithExplicitAttribute()
    {
        var html = H("div", new Attribute("class", "test"));
        html.Render(false).Should().Be("<div class=\"test\"></div>");
    }

    [Fact]
    public void WithFragment()
    {
        var html = H("div", F(H("span", "Hello, World"), H("span", "Another World")));
        html.Render(false)
            .Should()
            .Be("<div><span>Hello, World</span><span>Another World</span></div>");
    }

    [Fact]
    public void RenderPrettifiedHtml()
    {
        var html = H(
            "html",
            A("lang", "en"),
            H("head", H("title", "Title"), H("script", A("src", "script.js"))),
            H(
                "body",
                H("h1", "Title"),
                H("p", "This is a paragraph."),
                H("p", "Another paragraph."),
                R("<p>Raw paragraph</p>"),
                H("input")
            )
        );
        var expected = """
<!DOCTYPE html>
<html lang="en">
    <head>
        <title>Title</title>
        <script src="script.js">
        </script>
    </head>
    <body>
        <h1>Title</h1>
        <p>This is a paragraph.</p>
        <p>Another paragraph.</p>
        <p>Raw paragraph</p>
        <input>
    </body>
</html>
""".Trim().ReplaceLineEndings();
        html.RenderPretty(maxColumn: 40).Trim().ReplaceLineEndings().Should().Be(expected);
    }
}
