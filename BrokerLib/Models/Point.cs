using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Models
{
    public class Point : DBModelBase
    {
        [Key]
        [Column(Order = 1, TypeName = "bigint")]
        public TimeFrames TimeFrame { get; set; }
        [Key]
        [Column(Order = 2, TypeName = "nvarchar(7)")]
        public string Symbol { get; set; }
        [Key]
        [Column(Order = 3, TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
        [Key]
        [Column(Order = 4, TypeName = "nvarchar(50)")]
        public string Line { get; set; }
        [Column(TypeName = "float")]
        public float Value { get; set; }

        public Point()
        : base(BrokerDBContext.providers)
        {

        }

    }
}
