using System;
using Nancy;

namespace Nancy.Markdown.Demo
{
	public class HomeModule : NancyModule
	{
		public HomeModule ()
		{
			Get["/"] = _ => View["Home"];			   
		}
	}
}

