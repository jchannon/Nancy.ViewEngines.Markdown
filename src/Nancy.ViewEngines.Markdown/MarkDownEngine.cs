namespace Nancy.ViewEngines.Markdown
{
    using Nancy.ViewEngines;
    using System.Collections.Generic;
    using Nancy.Responses;
    using System.IO;
    using MarkdownSharp;
    using Nancy.ViewEngines.SuperSimpleViewEngine;
    using System;
    using System.Text.RegularExpressions;

    public class MarkDownEngine : IViewEngine
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly SSVEWrapper ssveWrapper;

        public IEnumerable<string> Extensions
        {
            get { return new[] { "md" }; }
        }

        public MarkDownEngine(IRootPathProvider rootPathProvider, SSVEWrapper ssveWrapper)
        {
            this.rootPathProvider = rootPathProvider;
            this.ssveWrapper = ssveWrapper;
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            var response = new HtmlResponse();

            var html = renderContext.ViewCache.GetOrAdd(viewLocationResult, result =>
                                                                     {
                                                                         string markDown = File.ReadAllText(rootPathProvider.GetRootPath() + viewLocationResult.Location + Path.DirectorySeparatorChar + viewLocationResult.Name + ".md");
                                                                         var parser = new Markdown();
                                                                         return parser.Transform(markDown);
                                                                     });

            /*
            
            <p>		- matches the literal string "<p>"
            (		- creates a capture group, so that we can get the text back by backreferencing in our replacement string
            @		- matches the literal string "@"
            [^<]*	- matches any character other than the "<" character and does this any amount of times
            )		- ends the capture group
            </p>	- matches the literal string "</p>"
            
            */

            var rgx = new Regex("<p>(@[^<]*)</p>");
            var serverHtml = rgx.Replace(html, "$1");

            var renderHtml = this.ssveWrapper.Render(serverHtml, model, new NancyViewEngineHost(renderContext));

            response.Contents = stream =>
            {
                var writer = new StreamWriter(stream);
                writer.Write(renderHtml);
                writer.Flush();
            };

            return response;
        }
    }
}

