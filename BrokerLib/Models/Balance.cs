using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UtilsLib.Utils;

namespace BrokerLib.Models
{
    public class Balance
    {
        public string Currency { get; set; }
        public float Available { get; set; }
        public float Reserved { get; set; }
    }
}
