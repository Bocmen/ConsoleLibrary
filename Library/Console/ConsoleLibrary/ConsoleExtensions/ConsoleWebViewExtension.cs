using ConsoleLibrary.Interfaces;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ConsoleLibrary.ConsoleExtensions
{
    public static class ConsoleWebViewExtension
    {
        public static Task DrawWebView(this IConsole console, WebViewSource source) => console.AddUIElement(CreateWebView(source));
        public static View CreateWebView(WebViewSource source)
        {
            WebView view = new WebView()
            {
                Source = source
            };
            return ConsoleDecorationExtension.CreateViewOnFullScreenMode((mode) => view);
        }

        public static Task DrawWebViewDot(this IConsole console, string content) => console.AddUIElement(CreateWebViewDot(content));
        public static View CreateWebViewDot(string content)
        {
            var htmlSource = new HtmlWebViewSource()
            {
                Html = $"<!DOCTYPE><html><head><script src=\"https://d3js.org/d3.v5.min.js\"></script><script src=\"https://unpkg.com/@hpcc-js/wasm@0.3.11/dist/index.min.js\"></script><script src=\"https://unpkg.com/d3-graphviz@3.0.5/build/d3-graphviz.js\"></script> <style type=\"text/css\"> body, html, div, svg {{ height: 100%; width: 100%; margin: 0 auto; padding: 0; }} </style> </head> <body> <div id=\"graph\" style=\"text-align: center;\"></div><script>graphviz = d3.select(\"#graph\").graphviz().renderDot(`{content}`);</script></body></html>"
            };
            return CreateWebView(htmlSource);
        }
    }
}
