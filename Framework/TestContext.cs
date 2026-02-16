//
// TestContext.cs - NinjaTrader Testing Framework
// Provides runtime context and state for test execution
//

#region Using declarations
using System;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
#endregion

namespace NinjaTrader.NinjaScript.AddOns.Testing
{
    /// <summary>
    /// Provides runtime context for test execution.
    /// Gives tests access to market data, mock objects, and current state.
    /// </summary>
    public class TestContext
    {
        #region Constructor
        
        /// <summary>
        /// Creates a new test context
        /// </summary>
        /// <param name="script">The NinjaScript running the tests (typically TestRunner)</param>
        internal TestContext(NinjaScriptBase script)
        {
            Script = script ?? throw new ArgumentNullException(nameof(script));
            // MockOrderManager = new MockOrderManager();
            // MockPosition = new MockPosition();
            // MockAccount = new MockAccount();
        }
        
        #endregion
        
        #region Properties - Script Reference
        
        /// <summary>
        /// Reference to the NinjaScript that is running the tests
        /// </summary>
        public NinjaScriptBase Script { get; }
        
        #endregion
        
        #region Properties - Mock Objects
        
        /// <summary>
        /// Mock order manager for tracking orders without real execution
        /// </summary>
        // public MockOrderManager MockOrderManager { get; }
        
        /// <summary>
        /// Mock position for tracking simulated positions
        /// </summary>
        // public MockPosition MockPosition { get; }
        
        /// <summary>
        /// Mock account for simulated account state
        /// </summary>
        // public MockAccount MockAccount { get; }
        
        #endregion
        
        #region Properties - Market Data Access
        
        /// <summary>
        /// Access to Close prices
        /// </summary>
        public ISeries<double> Close => Script.Close;
        
        /// <summary>
        /// Access to Open prices
        /// </summary>
        public ISeries<double> Open => Script.Open;
        
        /// <summary>
        /// Access to High prices
        /// </summary>
        public ISeries<double> High => Script.High;
        
        /// <summary>
        /// Access to Low prices
        /// </summary>
        public ISeries<double> Low => Script.Low;
        
        /// <summary>
        /// Access to Volume
        /// </summary>
        public ISeries<double> Volume => Script.Volume;
        
        /// <summary>
        /// Access to bar Time
        /// </summary>
        /// TODO Consider adding validation for null or invalid time series
        // public ISeries<DateTime> Time => Script.Time;
        
        /// <summary>
        /// Current bar number
        /// </summary>
        public int CurrentBar => Script.CurrentBar;
        
        /// <summary>
        /// Total bar count
        /// </summary>
        public int Count => Script.Count;
        
        /// <summary>
        /// Bars object
        /// </summary>
        public Bars Bars => Script.Bars;
        
        /// <summary>
        /// Current instrument
        /// </summary>
        public Instrument Instrument => Script.Instrument;
        
        #endregion
        
        #region Properties - Additional Data
        
        /// <summary>
        /// Custom data dictionary for storing test-specific data
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> Data { get; } = 
            new System.Collections.Generic.Dictionary<string, object>();
        
        /// <summary>
        /// Test execution start time
        /// </summary>
        public DateTime TestStartTime { get; private set; }
        
        /// <summary>
        /// Test execution end time
        /// </summary>
        public DateTime TestEndTime { get; private set; }
        
        #endregion
        
        #region Methods - Helper Methods
        
        /// <summary>
        /// Stores a value in the test context data dictionary
        /// </summary>
        public void Set(string key, object value)
        {
            Data[key] = value;
        }
        
        /// <summary>
        /// Resets the test context state, clearing all data and resetting timestamps
        /// </summary>
        public void Reset()
        {
            Data.Clear();
            TestStartTime = DateTime.Now;
            TestEndTime = DateTime.MinValue;
        }
        
        /// <summary>
        /// Retrieves a value from the test context data dictionary
        /// </summary>
        public T Get<T>(string key)
        {
            if (Data.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            
            return default(T);
        }
        
        /// <summary>
        /// Checks if a key exists in the test context data dictionary
        /// </summary>
        public bool Contains(string key)
        {
            return Data.ContainsKey(key);
        }
        
        /// <summary>
        /// Prints a message to the Output window with context information
        /// </summary>
        public void Print(string message)
        {
            Script.Print($"[TestContext @ Bar {CurrentBar}] {message}");
        }
        
        #endregion
        
        
    }
}