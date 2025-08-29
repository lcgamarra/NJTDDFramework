#region Using declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.TestKit
{
	public class TestKitTester : IndicatorRenderBase
	{
		private static int FailedTests { get; set; } = 0;
		private static int TotalTests { get; set; } = 0;
		private static List<Action> _testsList =  new List<Action>();
		private static List<string> _failedTestsNames = new List<string>();


		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "TestKit";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
			if (CurrentBar == 0)
			{
				
			}
		}

		public static void RunAllTests()
		{
			foreach (var test in _testsList)
			{
				test();
			}
			
			LogTestsResults();
		}

		public static void ResetTestKit()
		{
			_testsList.Clear();
			_failedTestsNames.Clear();
			TotalTests = 0;
			FailedTests = 0;
		}

		private static void LogTestsResults()
		{
			
			Log($"Tests Results: Failed: {FailedTests} | Total: {TotalTests}", LogLevel.Information);

			foreach (var testName in _failedTestsNames)
			{
				Log($"- {testName}", LogLevel.Information);
			}
		}

		public static void RegisterTest(Action action)
		{
			_testsList.Add(action);
		}

		public static void AssertTrue(bool condition, [CallerMemberName] string caller = "")
		{
			TotalTests++;
			if (!condition)
			{
				FailedTests++;
				_failedTestsNames.Add(caller);
			}
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private TestKit.TestKitTester[] cacheTestKitTester;
		public TestKit.TestKitTester TestKitTester()
		{
			return TestKitTester(Input);
		}

		public TestKit.TestKitTester TestKitTester(ISeries<double> input)
		{
			if (cacheTestKitTester != null)
				for (int idx = 0; idx < cacheTestKitTester.Length; idx++)
					if (cacheTestKitTester[idx] != null &&  cacheTestKitTester[idx].EqualsInput(input))
						return cacheTestKitTester[idx];
			return CacheIndicator<TestKit.TestKitTester>(new TestKit.TestKitTester(), input, ref cacheTestKitTester);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.TestKit.TestKitTester TestKitTester()
		{
			return indicator.TestKitTester(Input);
		}

		public Indicators.TestKit.TestKitTester TestKitTester(ISeries<double> input )
		{
			return indicator.TestKitTester(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.TestKit.TestKitTester TestKitTester()
		{
			return indicator.TestKitTester(Input);
		}

		public Indicators.TestKit.TestKitTester TestKitTester(ISeries<double> input )
		{
			return indicator.TestKitTester(input);
		}
	}
}

#endregion
