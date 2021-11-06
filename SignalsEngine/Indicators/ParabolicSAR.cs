// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sma.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Simple Moving Average Indicator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using BrokerLib.Market;
using BrokerLib.Models;
using SignalsEngine.Indicators;
using System;
using static BrokerLib.BrokerLib;

namespace SignalsEngine
{
    /// <summary>
    /// Simple Moving Average Indicator.
    /// </summary>
    public class ParabolicSAR : Indicator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParabolicSAR"/> class. 
        /// </summary>

        private float extremePoint = 0;
        private float accelarationFactor = 0;
        private float initAccelarationFactor = 0;
        private float maximum = 0;
        private bool uptrend = true;
        private float step = 0.02f;


        public ParabolicSAR(int Period, float AccelarationFactor, float Step, float Maximum, TimeFrames TimeFrame, MarketInfo marketInfo)
        : base("PSAR:" + Period, Period, TimeFrame, marketInfo, "Parabolic Stop and Reverse")
        {
            AddArgument("Period");
            AddArgument("AccelarationFactor");
            AddArgument("Step");
            AddArgument("Maximum");

            this.accelarationFactor = AccelarationFactor;
            this.initAccelarationFactor = AccelarationFactor;
            this.step = Step;
            this.maximum = Maximum;
            this.ShorDescriptionName = GetShorDescriptionName();
        }

        public override void Init(Indicator indicator)
        {
            try
            {
                Candle candle = indicator.GetLastValue("middle");
                Update(candle, candle.Max);
                AddLastClose(candle.Max, indicator.GetLastTimestamp());
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
        }

        public override bool CalculateNext(Indicator indicator)
        {
            try
            {
                if (!base.CalculateNext(indicator))
                {
                    return false;
                }

                Candle candle = indicator.GetLastValue("middle");
                float PriorSAR = GetLastClose();
                float CurrentSAR = Update(candle, PriorSAR);

                if (Count() >= Period)
                {
                    RemoveFirst();
                }

                AddLastClose(CurrentSAR, indicator.GetLastTimestamp());

                return true;
            }
            catch (Exception e)
            {
                SignalsEngine.DebugMessage(e);
            }
            return false;
        }

        public float Update(Candle candle, float priorSAR) 
        {
            if (uptrend)
            {
                if (candle.Max > extremePoint)
                {
                    UpdateAccFactor();
                    extremePoint = candle.Max;
                }
            }
            else
            {
                if (candle.Min < extremePoint)
                {
                    UpdateAccFactor();
                    extremePoint = candle.Min;
                }
            }
            if (uptrend)
            {
                if (priorSAR > candle.Min)
                {
                    uptrend = !uptrend;
                    accelarationFactor = initAccelarationFactor;
                    return extremePoint;
                }
            }
            else
            {
                if (priorSAR < candle.Max)
                {
                    uptrend = !uptrend;
                    accelarationFactor = initAccelarationFactor;
                    return extremePoint;
                }
            }
            float CurrentSAR = 0;
            if (uptrend)
            {
                CurrentSAR = priorSAR + accelarationFactor * (extremePoint - priorSAR);// Rising SAR
            }
            else
            {
                CurrentSAR = priorSAR - accelarationFactor * (priorSAR - extremePoint);// Falling SAR
            }
            return CurrentSAR;
        }

        public void UpdateAccFactor() 
        {
            if (accelarationFactor < maximum)
            {
                accelarationFactor += step;
            }
        }
    }
}
