using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;

namespace BotLib.Models
{
    public class Score : DBModelBase
    {
        [Key]
        [Column(TypeName = "nvarchar(450)")]
        public string BotParametersId { get; set; }
        [Column(TypeName = "bigint")]
        public int Positions { get; set; }
        [Column(TypeName = "bigint")]
        public float Successes { get; set; }
        [Column(TypeName = "float")]
        public float AmountGained { get; set; }
        [Column(TypeName = "float")]
        public float AmountGainedDaily { get; set; }
        [Column(TypeName = "float")]
        public float CurrentProfit { get; set; }
        [Column(TypeName = "int")]
        public int ActiveTransactions { get; set; }
        [Column(TypeName = "float")]
        public float MaxDrawBack { get; set; }

        public Score()
        : base(BotDBContext.providers)
        {

        }

        public float CalculateFinalScore(/*Score score*/)
        {
            float Score1 = Positions > 0 ? Successes / Positions : 0;
            float Score2 = Sigmoid2(AmountGainedDaily, 1);
            float Score3 = Sigmoid2(AmountGained, 0.1f);
            float Score4 = Sigmoid2(CurrentProfit, 1);
            float Score5 = Sigmoid2(AmountGained / -MaxDrawBack, 1);

            float DeScore1 = Sigmoid2(ActiveTransactions, 0.01f);

            if (float.IsNaN(Score1))
            {
                Score1 = 0;

            }
            else if (float.IsNaN(Score2))
            {
                Score2 = 0;
            }
            else if (float.IsNaN(Score3))
            {
                Score3 = 0;
            }
            else if (float.IsNaN(Score4))
            {
                Score4 = 0;
            }
            else if (float.IsNaN(Score5))
            {
                Score5 = 0;
            }
            else if (float.IsNaN(DeScore1))
            {
                DeScore1 = 0;
            }

            float FinalScore = Score1 * 0.01f + Score2 * 0.5f + Score3 * 0.5f  + Score4 * 0.3f + Score5 * 0.5f - DeScore1 * 0.1f;

            return FinalScore;
        }

        public void reset() 
        {
            ActiveTransactions = 0;
            AmountGained = 0;
            AmountGainedDaily = 0;
            Positions = 0;
            Successes = 0;
            CurrentProfit = 0;
        }
        public float Sigmoid(float value, float c = 1)
        {
            float e_power_x = MathF.Pow(MathF.E, value);
            return (e_power_x / (e_power_x + c) - 0.5f) * 2;
        }

        public float Sigmoid2(float x, float k = 1, float L = 1, float x0 = 0)
        {
            float e_power_x = MathF.Pow(MathF.E, -k * (x - x0));
            return L / (1 + e_power_x) - 0.5f;
        }
    }
}
