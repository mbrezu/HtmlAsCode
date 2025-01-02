using System.Text;
using System.Web;

namespace HtmlAsCode;

public static class Renderer
{
public interface IRoot { }

    public record Attribute(string Name, string Value) : IRoot;

    public interface INode : IRoot { }
    public record Element(
        string Name,
        IEnumerable<Attribute> Attributes,
        IEnumerable<INode> Children) : INode;

    public record Text(string Content) : INode;
    public record RawText(string Content) : INode;

    private static readonly HashSet<string> voidElementNames = new([
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
    ], StringComparer.OrdinalIgnoreCase);

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
                            throw new InvalidOperationException($"Unexpected node type: {child.GetType().Name}");
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
