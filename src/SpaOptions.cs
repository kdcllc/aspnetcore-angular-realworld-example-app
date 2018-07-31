namespace Conduit
{
    public class SpaOptions
    {
        public SpaOptions()
        {
            DistPath = "/dist";
            DefaultPage = "/index.html";
            DevServerScript = "start:hosted";
        }

        public string MapPath { get; set; }
        public string SourcePath { get; set; }
        public string DistPath { get; set; }
        public string DefaultPage { get; set; }
        public string DevServerScript { get; set; }
    }
}
