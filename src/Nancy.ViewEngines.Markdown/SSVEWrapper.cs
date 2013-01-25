namespace Nancy.ViewEngines.Markdown
{
    using Nancy.ViewEngines.SuperSimpleViewEngine;

    public class SSVEWrapper
    {
        private readonly SuperSimpleViewEngine engineWrapper;

        public SSVEWrapper(SuperSimpleViewEngine engineWrapper)
        {
            this.engineWrapper = engineWrapper;

         
        }

        public string Render(string template, dynamic model, IViewEngineHost host)
        {
            return engineWrapper.Render(template, model, host);
        }

    }
}
