using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYSerial.Models
{

    public class mycfg
    {
        public string Language { get; set; }
        public bool FormatDisColor { get; set; }
        public string SendColor { get; set; }
        public string RecColor { get; set; }
        //public string LogPath { get; set; }
        public bool CheckUpdate { get; set; }
    }
    public class hiscfg
    {
        public List<string> his { get; set; }
    }

}
