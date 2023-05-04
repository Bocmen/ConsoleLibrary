using ConsoleLibrary.Views;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Xamarin.Forms;

namespace ConsoleLibrary.Extensions
{
    public static class FormattedStringExtension
    {
        public const string ValueAttributeName = "value";
        public const string HrefAttributeName = "href";

        public const string ColorElementName = "color";
        public const string LinkElementName = "link";

        private static readonly Dictionary<string, Func<XElement, Span>> _generatorsSpan = new Dictionary<string, Func<XElement, Span>>()
        {
            {ColorElementName, ParseColorElem },
            {LinkElementName, ParseLink },
        };

        public static bool TryAddGenerator(string nameElement, Func<XElement, Span> fGet)
        {
            if (_generatorsSpan.ContainsKey(nameElement)) return false;
            _generatorsSpan.Add(nameElement, fGet);
            return true;
        }

        public static string ColorPattern(string text, Color color) => $"<{ColorElementName} {ValueAttributeName}='{color.ToHex()}'>{text}</{ColorElementName}>";
        public static string LinkPattern(string text, string url) => $"<{LinkElementName} {HrefAttributeName}='{url}'>{text}</{LinkElementName}>";
        public static void ParseText(this FormattedString formattedString, string text)
        {
            try
            {
                var xElement = XElement.Parse($"<div>{text}</div>");
                foreach (var node in xElement.Nodes())
                {
                    if (node.NodeType == System.Xml.XmlNodeType.Text)
                    {
                        var xText = (XText)node;
                        formattedString.Spans.Add(new Span() { Text = xText.Value });
                    }
                    else if (node.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        var nodeElem = (XElement)node;
                        if (_generatorsSpan.TryGetValue(nodeElem.Name.LocalName.ToLower(), out Func<XElement, Span> fGet))
                        {
                            var spanAdd = fGet.Invoke(nodeElem);
                            if (spanAdd != null) formattedString.Spans.Add(spanAdd);
                        }
                    }
                }
            }
            catch
            {
                formattedString.Spans.Add(new Span() { Text = text }); // FIX OR DELITE ???
            }
        }
        private static Span ParseColorElem(XElement element)
        {
            var colorAttribute = element.Attribute(ValueAttributeName);
            if (colorAttribute == null) return null;
            return new Span() { Text = element.Value, TextColor = Color.FromHex(colorAttribute.Value) };
        }
        private static HyperlinkSpan ParseLink(XElement element)
        {
            var colorAttribute = element.Attribute(HrefAttributeName);
            if (colorAttribute == null) return null;
            return new HyperlinkSpan() { Text = element.Value, Url = colorAttribute.Value };
        }

        public static FormattedString Create(string text)
        {
            FormattedString formattedString = new FormattedString();
            formattedString.ParseText(text);
            return formattedString;
        }
    }
}
