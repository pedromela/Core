using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;

namespace BotLib.Models
{
    public class Profit : DBModelBase
    {
        [Key]
        [Column(Order = 1, TypeName = "nvarchar(450)")]
        public string BotId { get; set; }
        [Key]
        [Column(Order = 2, TypeName = "datetime")]
        public DateTime Timestamp { get; set; }
        [Column(TypeName = "float")]
        public float ProfitValue { get; set; }
        [Column(TypeName = "float")]
        public float DrawBack { get; set; }


        public Profit()
        : base(BotDBContext.providers)
        {

        }

    }
}
