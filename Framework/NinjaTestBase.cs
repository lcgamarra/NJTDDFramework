//
// NinjaTestBase.cs - NinjaTrader Testing Framework
// Base class for all NinjaTrader test classes
//

#region Using declarations

using System;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;

#endregion

namespace NinjaTrader.NinjaScript.AddOns.Testing
{
    /// <summary>
    /// Base class for all NinjaTrader test classes.
    /// Provides access to market data, indicators, and test context during test execution.
    /// </summary>
    public abstract class NinjaTestBase
    {
        #region Properties

        /// <summary>
        /// Test context providing access to runtime state and mock objects
        /// </summary>
        protected TestContext Context { get; private set; }

        /// <summary>
        /// Reference to the NinjaScript that is running the tests (typically TestRunner indicator)
        /// </summary>
        protected NinjaScriptBase Script { get; private set; }

        /// <summary>
        /// Access to Close prices
        /// </summary>
        protected ISeries<double> Close => Script.Close;

        /// <summary>
        /// Access to Open prices
        /// </summary>
        protected ISeries<double> Open => Script.Open;

        /// <summary>
        /// Access to High prices
        /// </summary>
        protected ISeries<double> High => Script.High;

        /// <summary>
        /// Access to Low prices
        /// </summary>
        protected ISeries<double> Low => Script.Low;

        /// <summary>
        /// Access to Volume
        /// </summary>
        protected ISeries<double> Volume => Script.Volume;

        /// <summary>
        /// Access to Median price (HL/2)
        /// </summary>
        protected ISeries<double> Median => Script.Median;

        /// <summary>
        /// Access to Typical price (HLC/3)
        /// </summary>
        protected ISeries<double> Typical => Script.Typical;

        /// <summary>
        /// Access to Weighted price (HLCC/4)
        /// </summary>
        protected ISeries<double> Weighted => Script.Weighted;

        /// <summary>
        /// Current bar number
        /// </summary>
        protected int CurrentBar => Script.CurrentBar;

        /// <summary>
        /// Total number of bars
        /// </summary>
        protected int Count => Script.Count;

        /// <summary>
        /// Current bar time
        /// </summary>
        /// TODO Consider adding validation for null or invalid time series
        // protected ISeries<DateTime> Time => Script.Time;

        /// <summary>
        /// Bars object
        /// </summary>
        protected Bars Bars => Script.Bars;

        /// <summary>
        /// Current instrument
        /// </summary>
        protected Instrument Instrument => Script.Instrument;

        /// <summary>
        /// Tick size
        /// </summary>
        protected double TickSize => Script.Instrument.MasterInstrument.TickSize;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the test base with the test context.
        /// Called by TestRunner before executing tests.
        /// </summary>
        internal void Initialize(TestContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Script = context.Script;

            OnInitialize();
        }

        /// <summary>
        /// Override this method to perform custom initialization logic.
        /// Called after Context and Script are set.
        /// </summary>
        protected virtual void OnInitialize()
        {
            // Override in derived classes if needed
        }

        #endregion

        #region Indicator Access

        /// <summary>
        /// Creates an SMA indicator with the specified period
        /// </summary>
        /// TODO Verify if this declaration is necessary
        // protected SMA SMA(int period)
        // {
        //     return Script.SMA(period);
        // }
        

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if series1 crossed above series2
        /// </summary>
        protected bool CrossAbove(ISeries<double> series1, ISeries<double> series2, int lookBackPeriod)
        {
            return Script.CrossAbove(series1, series2, lookBackPeriod);
        }

        /// <summary>
        /// Checks if series1 crossed below series2
        /// </summary>
        protected bool CrossBelow(ISeries<double> series1, ISeries<double> series2, int lookBackPeriod)
        {
            return Script.CrossBelow(series1, series2, lookBackPeriod);
        }

        // TODO Verify if these methods are necessary
        // /// <summary>
        // /// Returns the minimum value over a specified number of bars
        // /// </summary>
        // protected double MIN(ISeries<double> series, int period)
        // {
        //     return Script.MIN(series, period)[0];
        // }
        //
        // /// <summary>
        // /// Returns the maximum value over a specified number of bars
        // /// </summary>
        // protected double MAX(ISeries<double> series, int period)
        // {
        //     return Script.MAX(series, period)[0];
        // }
        //
        // /// <summary>
        // /// Returns the sum over a specified number of bars
        // /// </summary>
        // protected double SUM(ISeries<double> series, int period)
        // {
        //     return Script.SUM(series, period)[0];
        // }

        /// <summary>
        /// Prints a message to the NinjaTrader Output window
        /// </summary>
        protected void Print(string message)
        {
            Script.Print($"[{GetType().Name}] {message}");
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Called before each test method executes.
        /// Override to provide per-test setup logic.
        /// </summary>
        protected virtual void SetUp()
        {
            // Override in derived classes
        }

        /// <summary>
        /// Called after each test method executes.
        /// Override to provide per-test cleanup logic.
        /// </summary>
        protected virtual void TearDown()
        {
            // Override in derived classes
        }

        #endregion
    }
}