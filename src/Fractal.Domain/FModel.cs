using System.Collections.Generic;

namespace Fractal.Domain
{
    public class FModel
    {
        public Request Request { get; set; }
        public DomainConceptInstance ThisDci { get; set; }
        public Dictionary<string, string> ModelParams { get; set; }        
    }
}