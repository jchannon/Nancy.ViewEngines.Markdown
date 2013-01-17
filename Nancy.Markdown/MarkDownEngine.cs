using System;
using Nancy.ViewEngines;
using System.Collections.Generic;
using Nancy.Responses;
using System.IO;
using MarkdownSharp;
namespace Nancy.Markdown
{
	public class MarkDownEngine : IViewEngine
	{


		public void Initialize (ViewEngineStartupContext viewEngineStartupContext)
		{

		}

		public Response RenderView (ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
		{
			HtmlResponse response = new HtmlResponse();
	
			response.Contents = stream =>
			{
				string markDown = File.ReadAllText(viewLocationResult.Name);
				MarkdownSharp.Markdown parser = new MarkdownSharp.Markdown();
				var HTML = parser.Transform(markDown);
				var writer = new StreamWriter(stream);
				writer.Write(HTML);
			};

			return response;
		}

		public IEnumerable<string> Extensions {
			get
			{
				return new [] {".md"};
			}
		}


	}
}

