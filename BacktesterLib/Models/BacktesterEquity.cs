using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BacktesterLib.Models
{
    public class BacktesterEquity
    {
        [Key]
        public int id { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        public string Name { get; set; }

        [Column(TypeName = "money")]
        public float Amount { get; set; }
        [Column(TypeName = "money")]
        public float RealAvailableAmountSymbol2 { get; set; }
        [Column(TypeName = "money")]
        public float RealAvailableAmountSymbol1 { get; set; }
        [Column(TypeName = "money")]
        public float EquityValue { get; set; }
        [Column(TypeName = "money")]
        public float VirtualBalance { get; set; } // Amount to Balance
        [Column(TypeName = "money")]
        public float VirtualNAV { get; set; } //rename EquityValue to NAV
    }
}
