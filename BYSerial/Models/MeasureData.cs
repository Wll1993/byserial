using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BYSerial.Models
{
    public class MeasureData
    {
        public DateTime DateTime { get; set; } = DateTime.Now;
        public double Value { get; set; } = 0;
    }
}
