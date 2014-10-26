using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Fractal.Domain;

namespace Fractal.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WebDoor()
        {
            Request request= CreateRequest();
            DomainConceptInstance site = null;
            DomainConceptInstance hostName = FractalDb.SelectOne("HostName", FractalDb.Where("Name", request.HostName));
            if (hostName == null)
            {
                site = FractalDb.SelectOne("Site", FractalDb.Where("Name", "Emidium"));
            }
            else
            {
                site = hostName.FirstLeft("SiteToHostNames");
            }
            if (site != null)
            {
                Razorness razorness = RunSiteControllers(site, request) ?? FindPageController(site, request);
                if (razorness != null)
                {
                    if (razorness.IsJson)
                    {
                        return Json(razorness.FModel);
                    }
                    else
                    {
                        return View(razorness.WebPageId, razorness.FModel);
                    }
                }
            }
            return View();
        }

        private Razorness RunSiteControllers(DomainConceptInstance site, Request request)
        {
            List<DomainConceptInstance> siteControllers = site.GetRightDcis("SiteToSiteControllers");
            foreach (DomainConceptInstance siteController in siteControllers)
            {
                object fmodel = FF.Rf(siteController, request, siteController);
                if (fmodel != null)
                {
                    return new Razorness(siteController.FirstRight("SiteControllerToPage").Id, (FModel)fmodel); ;
                }
            }
            return null;
        }

        private Razorness FindPageController(DomainConceptInstance site, Request request)
        {
            List<DomainConceptInstance> pageControllers = site.GetRightDcis("SiteToPageControllers");
            string path = request.Path;
            DomainConceptInstance pageController = pageControllers.Find(pg => pg["Path"] == path);
            if (pageController != null)
            {
                object fmodel = FF.Rf(pageController, request, pageController);
                return new Razorness(pageController["JSON"], pageController.FirstRight("PageControllerToPage").Id, (FModel)fmodel);
            }
            return null;
        }

        private Request CreateRequest()
        {
            Request request = new Request();
            request.HostName = Request.Url.Host;
            request.Path = Request.Url.AbsolutePath;
            return request;
        }
    }
}
