namespace Nancy.ViewEngines.Markdown.Demo
{
	using System;
	using Nancy;

	public class HomeModule : NancyModule
	{
		public HomeModule ()
		{
			Get["/"] = _ => View["Home"];			   
		}
	}
}

