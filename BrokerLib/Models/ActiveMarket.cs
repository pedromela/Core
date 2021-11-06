using BrokerLib.Brokers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UtilsLib.Utils;

namespace BrokerLib.Models
{
    public class ActiveMarket : DBModelBase
    {
        [Key]
        public int id { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public string Market { get; set; }

        [Column(TypeName = "bigint")]
        public BrokerLib.Brokers BrokerId { get; set; }
        [Column(TypeName = "bigint")]
        public BrokerLib.BrokerType BrokerType { get; set; }

        public ActiveMarket()
        : base(BrokerDBContext.providers)
        {

        }

        public ActiveMarket(string Market, BrokerDescription brokerDescription)
        : base(BrokerDBContext.providers)
        {
            this.Market = Market;
            this.BrokerId = brokerDescription.BrokerId;
            this.BrokerType = brokerDescription.BrokerType;

        }

        //public override void Store()
        //{
        //    try
        //    {
        //        using (BrokerDBContext botContext = BrokerDBContext.newDBContext())
        //        {
        //            botContext.ActiveMarkets.Add(this);
        //            botContext.SaveChanges();
        //        }
        //        using (BrokerDBContext botContext = BrokerDBContext.newDBContextClient())
        //        {
        //            botContext.ActiveMarkets.Add(this);
        //            botContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        BrokerLib.DebugMessage(e);
        //    }
        //}

        //public override void Update()
        //{
        //    try
        //    {
        //        using (BrokerDBContext botContext = BrokerDBContext.newDBContext())
        //        {
        //            botContext.ActiveMarkets.Update(this);
        //            botContext.SaveChanges();
        //        }
        //        using (BrokerDBContext botContext = BrokerDBContext.newDBContextClient())
        //        {
        //            botContext.ActiveMarkets.Update(this);
        //            botContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        BrokerLib.DebugMessage(e);
        //    }
        //}

        //public override void Delete()
        //{
        //    try
        //    {
        //        using (BrokerDBContext botContext = BrokerDBContext.newDBContext())
        //        {
        //            botContext.ActiveMarkets.Remove(this);
        //            botContext.SaveChanges();
        //        }
        //        using (BrokerDBContext botContext = BrokerDBContext.newDBContextClient())
        //        {
        //            botContext.ActiveMarkets.Remove(this);
        //            botContext.SaveChanges();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        BrokerLib.DebugMessage(e);
        //    }
        //}

        public override bool Equals(Object obj)
        {
            try
            {
                if (obj == null || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    ActiveMarket activeMarket = (ActiveMarket)obj;
                    if (Market == activeMarket.Market &&
                        BrokerId == activeMarket.BrokerId &&
                        BrokerType == activeMarket.BrokerType)
                    {
                        return true;
                    }
                }

            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Market.GetHashCode() + BrokerId.GetHashCode() + BrokerType.GetHashCode();
        }
    }
}
