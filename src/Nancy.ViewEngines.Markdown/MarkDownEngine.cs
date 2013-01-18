using System;
using Nancy.ViewEngines;
using System.Collections.Generic;
using Nancy.Responses;
using System.IO;
using MarkdownSharp;
namespace Nancy.ViewEngines.Markdown
{
    public class MarkDownEngine : IViewEngine
    {
        private readonly IRootPathProvider rootPathProvider;

        public IEnumerable<string> Extensions
        {
            get { return new[] { "md" }; }
        }

        public MarkDownEngine(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            var response = new HtmlResponse();

            string HTML = renderContext.ViewCache.GetOrAdd(viewLocationResult, result =>
                                                                     {
                                                                         string markDown = File.ReadAllText(rootPathProvider.GetRootPath() + viewLocationResult.Location + "\\" + viewLocationResult.Name + ".md");
                                                                         var parser = new MarkdownSharp.Markdown();
                                                                         return parser.Transform(markDown);
                                                                     });
            response.Contents = stream =>
            {
                var writer = new StreamWriter(stream);
                writer.Write(HTML);
                writer.Flush();
            };
            return response;
        }




    }
}

