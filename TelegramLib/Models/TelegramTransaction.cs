using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace TelegramLib.Models
{
    public class TelegramTransaction : DBModel
    {
        [Key]
        [Column(TypeName = "nvarchar(450)")]
        public string id { get; set; }
        [Column(TypeName = "nvarchar(10)")]
        public string Market { get; set; }
        [Column(TypeName = "nvarchar(10)")]
        public TransactionType Type { get; set; }
        [Column(TypeName = "nvarchar(100)")]
        public string Channel { get; set; }
        [Column(TypeName = "float")]
        public float EntryValue { get; set; }
        [Column(TypeName = "float")]
        public float TakeProfit { get; set; }
        [Column(TypeName = "float")]
        public float TakeProfit2 { get; set; }
        [Column(TypeName = "float")]
        public float TakeProfit3 { get; set; }
        [Column(TypeName = "float")]
        public float StopLoss { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }

        public TelegramTransaction()
        {
            Market = "";
            //Type = "";
            EntryValue = 0;
            TakeProfit = 0;
            TakeProfit2 = 0;
            TakeProfit3 = 0;
            StopLoss = 0;
        }

        public override void Store()
        {
            try
            {
                using (TelegramDBContext telegramContext = TelegramDBContext.newDBContext())
                {
                    telegramContext.TelegramTransactions.Add(this);
                    telegramContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
        }

        public override void Update()
        {
            try
            {
                using (TelegramDBContext telegramContext = TelegramDBContext.newDBContext())
                {
                    telegramContext.TelegramTransactions.Update(this);
                    telegramContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
        }


        public override void Delete()
        {
            try
            {
                using (TelegramDBContext telegramContext = TelegramDBContext.newDBContext())
                {
                    telegramContext.TelegramTransactions.Remove(this);
                    telegramContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
        }

        public TelegramTransaction Invert()
        {
            try
            {
                if (!IsConsistent())
                {
                    return null;
                }

                float takeProfitDiff = TakeProfit - EntryValue;
                TakeProfit = EntryValue - takeProfitDiff;
                float stopLossDiff = StopLoss - EntryValue;
                StopLoss = EntryValue - stopLossDiff;
                if (TakeProfit2 > 0)
                {
                    float takeProfit2Diff = TakeProfit2 - EntryValue;
                    TakeProfit2 = EntryValue - takeProfit2Diff;
                }
                if (TakeProfit3 > 0)
                {
                    float takeProfit3Diff = TakeProfit3 - EntryValue;
                    TakeProfit3 = EntryValue - takeProfit3Diff;
                }

                if (Type == TransactionType.buy)
                {
                    Type = TransactionType.sell;
                }
                else if (Type == TransactionType.sell)
                {
                    Type = TransactionType.buy;
                }
                else
                {
                    return null;
                }
                return this;

            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
            return null;
        }

        public bool IsConsistent()
        {
            try
            {
                if (Timestamp == DateTime.MinValue)
                {
                    return false;
                }
                if (string.IsNullOrEmpty(Channel))
                {
                    return false;
                }
                //if (string.IsNullOrEmpty(Type.))
                //{
                //    return false;
                //}
                if (string.IsNullOrEmpty(Market))
                {
                    return false;
                }
                if (StopLoss <= 0 || TakeProfit <= 0 || EntryValue <= 0)
                {
                    return false;
                }

                if (Type == TransactionType.buy)
                {
                    if (StopLoss >= EntryValue ||
                        (TakeProfit > 0 && StopLoss >= TakeProfit) ||
                        (TakeProfit2 > 0 && StopLoss >= TakeProfit2) ||
                        (TakeProfit3 > 0 && StopLoss >= TakeProfit3))
                    {
                        return false;
                    }
                }
                else if (Type == TransactionType.sell)
                {
                    if (StopLoss <= EntryValue ||
                        (TakeProfit > 0 && StopLoss <= TakeProfit) ||
                        (TakeProfit2 > 0 && StopLoss <= TakeProfit2) ||
                        (TakeProfit3 > 0 && StopLoss <= TakeProfit3))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
            return false;
        }

        public bool IsEqual(TelegramTransaction t)
        {
            try
            {
                if (t == null)
                {
                    return false;
                }

                if (Market == t.Market &&
                    Type == t.Type &&
                    EntryValue == t.EntryValue &&
                    TakeProfit == t.TakeProfit &&
                    TakeProfit2 == t.TakeProfit2 &&
                    TakeProfit3 == t.TakeProfit3 &&
                    StopLoss == t.StopLoss)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
            return false;
        }

        public string Parse()
        {
            try
            {
                return JsonConvert.SerializeObject(this);
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
            return null;
        }

        public string ParseBeautify()
        {
            try
            {
                string result = "New signal" + Environment.NewLine;
                result += Environment.NewLine;

                if (string.IsNullOrEmpty(Market))
                {
                    return null;
                }
                string type = "";
                if (Type == TransactionType.buy)
                {
                    type = "Buy";
                }
                else if (Type == TransactionType.sell)
                {
                    type = "Sell";
                }
                result += type + " " + Market + " now at " + EntryValue + Environment.NewLine;
                result += Environment.NewLine;

                if (TakeProfit == 0.0f)
                {
                    return null;
                }
                result += "Take profit at " + TakeProfit + Environment.NewLine;
                result += Environment.NewLine;

                if (TakeProfit2 > 0.0f)
                {
                    result += "Take profit 2 at " + TakeProfit2 + Environment.NewLine;
                    result += Environment.NewLine;
                }

                if (TakeProfit3 > 0.0f)
                {
                    result += "Take profit 3 at " + TakeProfit3 + Environment.NewLine;
                    result += Environment.NewLine;
                }

                if (StopLoss == 0.0f)
                {
                    return null;
                }
                result += "Stop loss at " + StopLoss + Environment.NewLine;


                return result;
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
            return null;
        }

        public static List<TelegramTransaction> InvertTelegramTransactions(List<TelegramTransaction> transactions) 
        {
            try
            {
                List<TelegramTransaction> invertedTransactions = new List<TelegramTransaction>();
                foreach (TelegramTransaction transaction in transactions)
                {
                    invertedTransactions.Add(transaction.Invert());
                }
            }
            catch (Exception e)
            {
                TelegramLib.DebugMessage(e);
            }
            return null;
        }
    }
}
