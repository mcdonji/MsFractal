using Fractal.Domain;

namespace Fractal.Web.Controllers
{
    public class Razorness
    {
        public Razorness(string json, string webPageId, FModel fModel)
        {
            IsJson = json == "True" || json == "true" || json == "T" || json == "t";
            WebPageId = webPageId;
            FModel = fModel;
        }
        public Razorness(string webPageId, FModel fModel)
        {
            IsJson = false;
            WebPageId = webPageId;
            FModel = fModel;
        }


        public string WebPageId { get; set; }
        public FModel FModel { get; set; }
        public bool IsJson { get; set; }
    }
}