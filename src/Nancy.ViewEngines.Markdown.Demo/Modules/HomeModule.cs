using System;
using Nancy;

namespace Nancy.ViewEngines.Markdown.Demo
{
	public class HomeModule : NancyModule
	{
		public HomeModule ()
		{
			Get["/"] = _ => View["Home"];			   
		}
	}
}

