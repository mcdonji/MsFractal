namespace Fractal.Domain
{
    public class Request
    {   
        public string Scheme { get; set; }
        public string HostName { get; set; }
        public string Port { get; set; }
        public string Path { get; set; }
        public string Post { get; set; }
        public string Ssl { get; set; }
        public string Session { get; set; }
        public string Ip { get; set; }
    }
}