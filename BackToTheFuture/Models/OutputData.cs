using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BackToTheFuture.Models;


namespace BackToTheFuture.Models
{
    public class OutputData
    {
        public int[] chooses { get; set; } = new int[4];
        public int border1 { get; set; }
        public int border2 { get; set; }
        public int border3 { get; set; }

        public double remainder1 { get; set; }
        public double remainder2 { get; set; }
        public double remainder3 { get; set; }
        public double remainder4 { get; set; }
    }
}