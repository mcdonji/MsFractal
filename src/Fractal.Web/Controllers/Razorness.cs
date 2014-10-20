using Fractal.Domain;

namespace Fractal.Web.Controllers
{
    public class Razorness
    {
        public Razorness(string webPageId, FModel fModel)
        {
            WebPageId = webPageId;
            FModel = fModel;
        }

        public string WebPageId { get; set; }
        public FModel FModel { get; set; }
    }
}