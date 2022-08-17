using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginLib.Models
{
    public class SubscriptionPackage
    {
        [Key]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; }
        [Column(TypeName = "bigint")]
        public int MaxAllowedBots { get; set; }
        [Column(TypeName = "bigint")]
        public bool BotCreation { get; set; }
        [Column(TypeName = "bigint")]
        public bool StrategyCreation { get; set; }
        [Column(TypeName = "bigint")]
        public bool Backtesting { get; set; }
    }
}
