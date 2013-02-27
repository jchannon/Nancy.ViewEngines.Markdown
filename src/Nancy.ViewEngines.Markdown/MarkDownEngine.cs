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
        private readonly SuperSimpleViewEngine engineWrapper;
        private static readonly Regex ParagraphSubstitution = new Regex("<p>(@[^<]*)</p>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /*
            
            <p>		- matches the literal string "<p>"
            (		- creates a capture group, so that we can get the text back by backreferencing in our replacement string
            @		- matches the literal string "@"
            [^<]*	- matches any character other than the "<" character and does this any amount of times
            )		- ends the capture group
            </p>	- matches the literal string "</p>"
            
            */

        public IEnumerable<string> Extensions
        {
            get { return new[] { "md" }; }
        }

        public MarkDownEngine(SuperSimpleViewEngine engineWrapper)
        {
            this.engineWrapper = engineWrapper;
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            var response = new HtmlResponse();

            var html = renderContext.ViewCache.GetOrAdd(viewLocationResult, result =>
                                                                                {
                                                                                    string markDown =
                                                                                        viewLocationResult.Contents()
                                                                                                          .ReadToEnd();


                                                                                    var parser = new Markdown();
                                                                                    return parser.Transform(markDown);
                                                                                });


            var serverHtml = ParagraphSubstitution.Replace(html, "$1");

            var renderHtml = this.engineWrapper.Render(serverHtml, model, new MarkdownViewEngineHost(new NancyViewEngineHost(renderContext), renderContext));

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

