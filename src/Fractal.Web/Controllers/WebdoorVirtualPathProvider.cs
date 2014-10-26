using System;
using System.Collections;
using System.Web.Caching;
using System.Web.Hosting;
using Fractal.Domain;

namespace Fractal.Web.Controllers
{
    public class WebdoorVirtualPathProvider : VirtualPathProvider
    {
        public override bool FileExists(string webPagePath)
        {
            DomainConceptInstance page = null;
            if (webPagePath.Contains("/Views/Home/") && webPagePath.Contains(".aspx"))
            {
                string dciId = webPagePath.Substring("/Views/Home/".Length).Replace(".aspx", "");
                page = FractalDb.Dci(dci => dci.Id == dciId);
            }
            if (page == null)
            {
                return base.FileExists(webPagePath);
            }
            else
            {
                return true;
            }
        }

//        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
//        {
//            return null;
//        }
//
//        public override bool DirectoryExists(string virtualDir)
//        {
//            return true;
//        }
//
//        public override VirtualDirectory GetDirectory(string virtualDir)
//        {
//            return new WebDoorVirtualDirectoryProvider(virtualDir);
//        }

        public override VirtualFile GetFile(string webPagePath)
        {
            DomainConceptInstance page = null;
            if (webPagePath.Contains("/Views/Home/") && webPagePath.Contains(".aspx"))
            {
                string dciId = webPagePath.Substring("/Views/Home/".Length).Replace(".aspx", "");
                page = FractalDb.Dci(dci => dci.Id == dciId);
            }
            if (page == null)
            {
                return base.GetFile(webPagePath);
            }
            else
            {
                return new WebDoorVirtualFileProvider(webPagePath, page["RazorPage"]);
            }
        }

    }
}