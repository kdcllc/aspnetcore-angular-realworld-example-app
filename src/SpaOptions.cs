namespace Conduit
{
    public class SpaOptions
    {
        public SpaOptions()
        {
            DistPath = "/dist";
            //ServerRenderBundlePath = "/dist-server/main.bundle.js";
            DefaultPage = "/index.html";
            DevServerScript = "start:hosted";
            //ServerRenderBuildScript = "build:ssr";
        }

        public string MapPath { get; set; }
        public string SourcePath { get; set; }
        public string DistPath { get; set; }
        public string ServerRenderBundlePath { get; set; }
        public string DefaultPage { get; set; }
        public string DevServerScript { get; set; }
        public string ServerRenderBuildScript { get; set; }
    }
}
