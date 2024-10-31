using BrokerLib.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using UtilsLib.Utils;
using static BrokerLib.BrokerLib;

namespace BrokerLib.Models
{
    public class Candle : DBModelBase
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
        [Column(TypeName = "float")]
        public float Max { get; set; }

        [Column(TypeName = "float")]
        public float Min { get; set; }
        [Column(TypeName = "float")]
        public float Open { get; set; }

        [Column(TypeName = "float")]
        public float Close { get; set; }

        [Column(TypeName = "float")]
        public float Volume { get; set; }

        [Column(TypeName = "float")]
        public float VolumeQuote { get; set; }

        public Candle()
        : base(BrokerDBContext.providers)
        {

        }

        public Candle(TimeFrames _TimeFrame, string _Symbol, DateTime _Timestamp, float _Min, float _Max, float _Close, float _Open, float _Volume, float _VolumeQuote)
        : base(BrokerDBContext.providers)
        {
            TimeFrame = _TimeFrame;
            Symbol = _Symbol;
            Timestamp = _Timestamp;
            Min = _Min;
            Max = _Max;
            Close = _Close;
            Open = _Open;
            Volume = _Volume;
            VolumeQuote = _VolumeQuote;
        }

        public override bool Equals(object obj)
        {
            var candle = obj as Candle;

            if (candle == null)
            {
                return false;
            }

            if (TimeFrame.Equals(candle.TimeFrame) &&
                Symbol.Equals(candle.Symbol) &&
                Timestamp.Equals(candle.Timestamp))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + TimeFrame.GetHashCode();
                hash = hash * 23 + Symbol.GetHashCode();
                hash = hash * 23 + Timestamp.GetHashCode();
                return hash;
            }
        }

        public bool IsEqual(Candle candle) 
        {
            try
            {
                if (TimeFrame.Equals(candle.TimeFrame) &&
                    Symbol.Equals(candle.Symbol) &&
                    Timestamp.Equals(candle.Timestamp))
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }

            return false;

        }

        public override void Store()
        {
            try
            {
                if (IsAbnormalCandle())
                {
                    return;
                }
                base.Store();
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public override void Update()
        {
            try
            {
                if (IsAbnormalCandle())
                {
                    return;
                }
                base.Update();
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public override void Delete()
        {
            try
            {
                if (IsAbnormalCandle())
                {
                    return;
                }
                base.Delete();
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
        }

        public bool IsAbnormalCandle() 
        {
            try
            {
                if (Timestamp.Second == 0 && Timestamp.Millisecond == 0)
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
            BrokerLib.DebugMessage("Candle::IsAbnormalCandle() : candle seconds are different than 0.");
            return true;
        }

        public static List<Candle> SaveCandlesInDB(List<Candle> candles)
        {
            try
            {
                if (!Candles.IsConsistent(candles))
                {
                    candles = Candles.FixConsistency(candles);
                }

                return BrokerDBContext.Execute(dataContext => {
                    List<Candle> toRemove = new List<Candle>();
                    HashSet<Candle> candleSet = candles.ToHashSet();
                    foreach (Candle candle in candleSet)
                    {
                        try
                        {
                            if (candle.IsAbnormalCandle())
                            {
                                continue;
                            }
                            EntityEntry<Candle> entity = dataContext.Entry(candle);
                            if (entity.State != EntityState.Detached)
                            {
                                continue;
                            }
                            Candle findCandle = dataContext.Candles.Find(candle.TimeFrame, candle.Symbol, candle.Timestamp);
                            if (findCandle == null)
                            {
                                dataContext.Candles.Add(candle);
                            }
                        }
                        catch (Exception e)
                        {
                            //dataContext.Candles.
                            EntityEntry<Candle> entity = dataContext.Entry(candle);
                            entity.State = EntityState.Unchanged;
                            toRemove.Add(candle);
                            BrokerLib.DebugMessage("Candle::SaveCandlesInDB() : " + e.Message);
                        }
                    }

                    foreach (Candle candle in toRemove)
                    {
                        candles.Remove(candle);
                    }

                    dataContext.SaveChanges();
                    return candles;
                }, true);
            }
            catch (Exception e)
            {
                BrokerLib.DebugMessage(e);
            }
            return null;
        }
    }
}
