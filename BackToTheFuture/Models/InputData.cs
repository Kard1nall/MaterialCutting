using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackToTheFuture.Models
{
    public class InputData
    {
        public SampleParam[] sampleParams { get; set; }
        public int areaSquare { get; set; }
        public int totalCount { get; set; }
        public SampleType[] sampleTypes { get; set; }
    }
}