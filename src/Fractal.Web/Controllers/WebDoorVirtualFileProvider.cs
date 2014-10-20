using System.Collections;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace Fractal.Web.Controllers
{
    public class WebDoorVirtualFileProvider : VirtualFile
    {
        private string data;

        public WebDoorVirtualFileProvider(string virtualPath, string data)
            : base(virtualPath)
        {
            this.data = data;
        }

        public override System.IO.Stream Open()
        {
            return new MemoryStream(ASCIIEncoding.Default.GetBytes(data));
        }
    }

    public class WebDoorVirtualDirectoryProvider : VirtualDirectory
    {
        public WebDoorVirtualDirectoryProvider(string virtualPath) : base(virtualPath)
        {
        }

        public override IEnumerable Directories
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override IEnumerable Files
        {
            get { throw new System.NotImplementedException(); }
        }

        public override IEnumerable Children
        {
            get { throw new System.NotImplementedException(); }
        }
    }
    
}