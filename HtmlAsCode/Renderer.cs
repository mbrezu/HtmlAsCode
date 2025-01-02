using System.Text;
using System.Web;
using FancyPen;

namespace HtmlAsCode;

public static class Renderer
{
    public interface IRoot { }

    public record Attribute(string Name, string Value) : IRoot;

    public interface INode : IRoot { }

    public record Element(
        string Name,
        IEnumerable<Attribute> Attributes,
        IEnumerable<INode> Children
    ) : INode;

    public record Text(string Content) : INode;

    public record RawText(string Content) : INode;

    private static readonly HashSet<string> voidElementNames = new(
        [
            "area",
            "base",
            "br",
            "col",
            "embed",
            "hr",
            "img",
            "input",
            "link",
            "meta",
            "param",
            "source",
            "track",
            "wbr",
        ],
        StringComparer.OrdinalIgnoreCase
    );

    public static string RenderPretty(
        this Element root,
        bool includeDoctype = true,
        int maxColumn = 100,
        int indentBy = 4
    )
    {
        Document renderAttribute(Attribute attr)
        {
            return Document.Concat(" ", attr.Name, "=\"", HttpUtility.HtmlEncode(attr.Value), "\"");
        }
        Document renderAttributes(IEnumerable<Attribute> attributes)
        {
            var list = attributes.ToList();
            return Document.Format(attributes.Select(renderAttribute).ToArray());
        }
        Document renderNode(INode node)
        {
            return node switch
            {
                Element childElement => renderImpl(childElement),
                Text text => (Document)text.Content,
                RawText rawText => (Document)rawText.Content,
                _ => throw new InvalidOperationException(
                    $"Unexpected node type: {node.GetType().Name}"
                ),
            };
        }
        Document renderImpl(Element element)
        {
            var tag = Document.Concat(
                $"<{element.Name}",
                renderAttributes(element.Attributes),
                ">"
            );
            if (voidElementNames.Contains(element.Name))
            {
                return tag;
            }
            else
            {
                var children = element.Children.ToList();
                if (children.Count == 0)
                {
                    return Document.Format(tag, $"</{element.Name}>");
                }
                else
                {
                    return Document.Format(
                        tag,
                        Document.Indent(
                            indentBy,
                            Document.Format(children.Select(renderNode).ToArray())
                        ),
                        $"</{element.Name}>"
                    );
                }
            }
        }
        var doc = includeDoctype
            ? Document.ConcatLines("<!DOCTYPE html>", renderImpl(root))
            : renderImpl(root);
        var renderer = new FancyPen.Renderer(maxColumn);
        return renderer.Render(doc);
    }

    public static string Render(this Element root, bool includeDoctype = true)
    {
        var sb = new StringBuilder();
        void renderImpl(Element element)
        {
            sb.Append($"<{element.Name}");
            foreach (var attr in element.Attributes)
            {
                sb.Append($" {attr.Name}=\"{HttpUtility.HtmlEncode(attr.Value)}\"");
            }
            bool isVoid = voidElementNames.Contains(element.Name);
            if (!isVoid)
            {
                sb.Append('>');
                foreach (var child in element.Children)
                {
                    switch (child)
                    {
                        case Element childElement:
                            renderImpl(childElement);
                            break;
                        case Text text:
                            sb.Append(HttpUtility.HtmlEncode(text.Content));
                            break;
                        case RawText rawText:
                            sb.Append(rawText.Content);
                            break;
                        default:
                            throw new InvalidOperationException(
                                $"Unexpected node type: {child.GetType().Name}"
                            );
                    }
                }
                sb.Append($"</{element.Name}>");
            }
            else
            {
                sb.Append('>');
            }
        }
        if (includeDoctype)
        {
            sb.Append("<!DOCTYPE html>");
        }
        renderImpl(root);
        return sb.ToString();
    }

    public static Element H(string name, params object[] members)
    {
        var attributes = new List<Attribute>();
        var children = new List<INode>();
        foreach (var member in members)
        {
            switch (member)
            {
                case Attribute attr:
                    attributes.Add(attr);
                    break;
                case IEnumerable<Attribute> attrs:
                    attributes.AddRange(attrs);
                    break;
                case Element element:
                    children.Add(element);
                    break;
                case IEnumerable<INode> elements:
                    children.AddRange(elements);
                    break;
                case string text:
                    children.Add(new Text(text));
                    break;
                case RawText rawText:
                    children.Add(rawText);
                    break;
                case Text text2:
                    children.Add(text2);
                    break;
                default:
                    throw new ArgumentException("Invalid member type");
            }
        }
        return new Element(name, attributes, children);
    }

    public static IEnumerable<Attribute> A(params string[] namesAndValues)
    {
        for (int i = 0; i < namesAndValues.Length; i += 2)
        {
            yield return new Attribute(namesAndValues[i], namesAndValues[i + 1]);
        }
    }

    public static IEnumerable<Element> F(params Element[] members) => members;

    public static RawText R(string rawText) => new(rawText);
}
