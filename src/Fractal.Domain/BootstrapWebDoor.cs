using System.IO;

namespace Fractal.Domain
{
    public class BootstrapWebDoor
    {
        public static bool WebDoorBootstrapComplete()
        {
            return FractalDb.Dcis("Site").Count > 0;
        }
        public static void Go()
        {
            if (WebDoorBootstrapComplete()){return;}
            FractalDb.InitAudit();

//            FractalDb.CreateDomainConcept("JsFile");
//              FractalDb.AddDomainConceptField( "JsFile", "Name");
//              FractalDb.AddDomainConceptField( "JsFile", "Order");
//              FractalDb.AddDomainConceptField( "JsFile", "Path");
//
//             FractalDb.CreateDomainConcept( "JsCssGroup");
//              FractalDb.AddDomainConceptField( "JsCssGroup", "Name");
//
//             FractalDb.CreateDomainConcept( "jsvar");
//              FractalDb.AddDomainConceptField( "jsvar", "name");
//              FractalDb.AddDomainConceptField( "jsvar", "funcstr");
//
//
//             FractalDb.CreateDomainConcept( "js");
//              FractalDb.AddDomainConceptField( "js", "name");
//              FractalDb.AddDomainConceptField( "js", "description");
//              FractalDb.AddDomainConceptField( "js", "jscode");
//
//             FractalDb.CreateDomainConcept( "csfile");
//              FractalDb.AddDomainConceptField( "csfile", "name");
//              FractalDb.AddDomainConceptField( "csfile", "order");
//              FractalDb.AddDomainConceptField( "csfile", "cscode");
//              FractalDb.AddDomainConceptField( "csfile", "jsPath");
//
//             FractalDb.CreateDomainConcept( "cssfile");
//              FractalDb.AddDomainConceptField( "cssfile", "name");
//              FractalDb.AddDomainConceptField( "cssfile", "Path");
//
//             FractalDb.CreateDomainConcept( "css");
//              FractalDb.AddDomainConceptField( "css", "selector");
//
//             FractalDb.CreateDomainConcept( "csselem");
//              FractalDb.AddDomainConceptField( "csselem", "name");
//              FractalDb.AddDomainConceptField( "csselem", "value");
//
//
//             FractalDb.CreateDomainConcept( "validation");
//              FractalDb.AddDomainConceptField( "validation", "name");
//              FractalDb.AddDomainConceptField( "validation", "sort");
//              FractalDb.AddDomainConceptField( "validation", "funcstr");

             FractalDb.CreateDomainConcept("SiteController");
                FractalDb.AddDomainConceptField("SiteController", "Name");
                FractalDb.AddDomainConceptField("SiteController", "Description");
                FractalDb.AddDomainConceptField("SiteController", "Order");
                FractalDb.AddDomainConceptField("SiteController", FF.HANDLE);
                FractalDb.AddDomainConceptField("SiteController", FF.FFUNC);

            FractalDb.CreateDomainConcept("PageController");
                FractalDb.AddDomainConceptField("PageController", "Path");
                FractalDb.AddDomainConceptField("PageController", FF.HANDLE);
                FractalDb.AddDomainConceptField("PageController", FF.FFUNC);

            FractalDb.CreateDomainConcept("WebPage");
                FractalDb.AddDomainConceptField("WebPage", "Path");
                FractalDb.AddDomainConceptField("WebPage", "RazorPage");

                FractalDb.CreateConnectionDescription("SiteController", "WebPage", "SiteControllerToPage", Cardinality.OneToOne, true, false, true);
                FractalDb.CreateConnectionDescription("PageController", "WebPage", "PageControllerToPage", Cardinality.OneToOne, true, false, true);
//            FractalDb.CreateConnectionDescription("PageController", "validation", "validations", Cardinality.OneToMany, true, false, false);
//            FractalDb.CreateConnectionDescription("WebPage", "validation", "validations", Cardinality.OneToMany, true, false, false);
//            FractalDb.CreateConnectionDescription("WebPage", "cssfile", "cssfiles", Cardinality.OneToMany, true, false, false);
//            FractalDb.CreateConnectionDescription("WebPage", "JsFile", "JsFiles", Cardinality.OneToMany, true, false, false);
//            FractalDb.CreateConnectionDescription("WebPage", "jsvar", "jsvars", Cardinality.OneToMany, true, false, false);
//            FractalDb.CreateConnectionDescription("WebPage", "js", "jss", Cardinality.OneToMany, true, false, false);
//            FractalDb.CreateConnectionDescription("WebPage", "csfile", "clojurescripts", Cardinality.OneToMany, true, false, false);
//            FractalDb.CreateConnectionDescription("WebPage", "css", "csses", Cardinality.OneToMany, true, false, false);
//            FractalDb.CreateConnectionDescription("WebPage", "JsCssGroup", "JsCssGroups", Cardinality.OneToMany, true, false, false);
//            FractalDb.CreateConnectionDescription("css", "csselem", "csselems", Cardinality.OneToMany, true, false, false);

            FractalDb.CreateDomainConcept("HostName");
            FractalDb.AddDomainConceptField("HostName", "Name");

            FractalDb.CreateDomainConcept("Site");
            FractalDb.AddDomainConceptField("Site", "Name");

            FractalDb.CreateConnectionDescription("Site", "SiteController", "SiteToSiteControllers", Cardinality.OneToMany, true, false, true);
            FractalDb.CreateConnectionDescription("Site", "PageController", "SiteToPageControllers", Cardinality.OneToMany, true, false, true);
            FractalDb.CreateConnectionDescription("Site", "HostName", "SiteToHostNames", Cardinality.OneToMany, true, false, true);

//            FractalDb.CreateDomainConcept("Snippetf");
//            FractalDb.AddDomainConceptField("Snippetf", "Name");
//            FractalDb.AddDomainConceptField("Snippetf", FF.HANDLE);
//            FractalDb.AddDomainConceptField("Snippetf", FF.FFUNC);

            DomainConceptInstance emidiumSite = FractalDb.CreateDomainConceptInstance("Site", "Emidium");

            FractalDb.Connect(emidiumSite, FractalDb.CreateDomainConceptInstance("HostName", "emidium.com"), "SiteToHostNames");
            FractalDb.Connect(emidiumSite, FractalDb.CreateDomainConceptInstance("HostName", "dev.emidium.com"), "SiteToHostNames");
            FractalDb.Connect(emidiumSite, FractalDb.CreateDomainConceptInstance("HostName", "localhost.emidium.com"), "SiteToHostNames");
            FractalDb.Connect(emidiumSite, FractalDb.CreateDomainConceptInstance("HostName", "localhost"), "SiteToHostNames");

            DomainConceptInstance indexPageController = FractalDb.CreateDomainConceptInstance("PageController", "/", "HandleIndex", "public FModel HandleIndex(Request request, DomainConceptInstance thisPc) { FModel fmodel = new FModel(); fmodel.Request = request;fmodel.ThisDci = thisPc; return fmodel; }");
            FractalDb.Connect(emidiumSite, indexPageController, "SiteToPageControllers");

            FractalDb.Connect(indexPageController, FractalDb.CreateDomainConceptInstance("WebPage", "/", INDEX), "PageControllerToPage");
        }         

        public const string INDEX = @"
<!DOCTYPE html>
<html>
    <head>
        <title>E M I D I U M</title>
    </head>
    <body>
        <h1 align=center> EMIDIUM </h1>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
    </body>
</html>
";
    }
}