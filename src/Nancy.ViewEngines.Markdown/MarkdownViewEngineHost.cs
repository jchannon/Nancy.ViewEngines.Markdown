using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nancy.ViewEngines.SuperSimpleViewEngine;

namespace Nancy.ViewEngines.Markdown
{
    public class MarkdownViewEngineHost : IViewEngineHost
    {
        private readonly IViewEngineHost viewEngineHost;
        private readonly IRenderContext renderContext;
        private static readonly IEnumerable<string> ValidExtensions = new[] { "md", "markdown" };

        public MarkdownViewEngineHost(IViewEngineHost viewEngineHost, IRenderContext renderContext)
        {
            this.viewEngineHost = viewEngineHost;
            this.renderContext = renderContext;
            this.Context = this.renderContext.Context;
        }

        public object Context { get; private set; }

        public string HtmlEncode(string input)
        {
            return this.viewEngineHost.HtmlEncode(input);
        }

        public string GetTemplate(string templateName, object model)
        {
            var viewLocationResult = this.renderContext.LocateView(templateName, model);

            if (viewLocationResult == null)
            {
                return "[ERR!]";
            }

            var masterpartialContent = viewLocationResult.Contents.Invoke().ReadToEnd();

            if (!ValidExtensions.Any(x => x.Equals(viewLocationResult.Extension, StringComparison.OrdinalIgnoreCase)))
            {
                return masterpartialContent;
            }

            string html;

            if (viewLocationResult.Name == "master")
            {
                string header = masterpartialContent.Substring(masterpartialContent.IndexOf("<!DOCTYPE html>", StringComparison.Ordinal),
                                                                masterpartialContent.IndexOf("<body>", StringComparison.Ordinal) + 6);

                string toConvert =
                    masterpartialContent.Substring(
                        masterpartialContent.IndexOf("<body>", StringComparison.Ordinal) + 6,
                        (masterpartialContent.IndexOf("</body>", StringComparison.Ordinal) - 7) -
                        (masterpartialContent.IndexOf("<body>", StringComparison.Ordinal)));

                string footer = masterpartialContent.Substring(masterpartialContent.IndexOf("</body>", StringComparison.Ordinal));

                var parser = new MarkdownSharp.Markdown();
                html = parser.Transform(toConvert);

                var regex = new Regex("<p>(@[^<]*)</p>");
                var serverHtml = regex.Replace(html, "$1");

                return header + serverHtml + footer;
            }
            else
            {
                var parser = new MarkdownSharp.Markdown();
                html = parser.Transform(masterpartialContent);
            }

            return html;
        }

        public string GetUriString(string name, params string[] parameters)
        {
            return this.viewEngineHost.GetUriString(name, parameters);
        }

        public string ExpandPath(string path)
        {
            return this.viewEngineHost.ExpandPath(path);
        }

        public string AntiForgeryToken()
        {
            return this.viewEngineHost.AntiForgeryToken();
        }
    }
}