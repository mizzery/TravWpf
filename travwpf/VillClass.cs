using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace travwpf
{
    public class VillClass
    {
        public string name { get; set; }
        public string href { get; set; }
        public string type { get; set; }
        public bool cap { get; set; }
        public string newdid { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public List<FieldClass> dorf1 { get; set; }
        public List<CityClass> dorf2 { get; set; }
    }

}
