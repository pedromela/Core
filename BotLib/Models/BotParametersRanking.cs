using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;

namespace BotLib.Models
{
    public class BotParametersRanking : DBModelBase
    {
        [Key]
        public int Rank { get; set; }
        [Column(TypeName = "nvarchar(450)")]
        public string BotId { get; set; }

        public BotParametersRanking(int Rank, string BotId)
        : base(BotDBContext.providers)
        {
            this.Rank = Rank;
            this.BotId = BotId;
        }

        public BotParametersRanking()
        : base(BotDBContext.providers)
        {

        }
    }
}
