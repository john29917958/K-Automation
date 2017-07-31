namespace Ncu.Oolab.Korat.KAutomation.Configs.App
{
    public enum Environments { Develop, Testing, Production }

    public class AppConfig
    {
        public string Name { get; set; }
        public Environments Environment { get; set; }
        public string Author { get; set; }
        public string Manager { get; set; }
        public string Contact { get; set; }
    }
}
