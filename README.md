# NinjaTrader Testing Framework

A comprehensive unit testing framework for NinjaTrader 8 indicators, strategies, and custom NinjaScript components. Run tests directly on charts with live or historical market data.

## Features

- **Runtime Test Execution**: Tests run on actual charts with real market data
- **Attribute-Based Test Discovery**: Mark test classes and methods with `[NinjaTest]` and `[TestCase]` attributes
- **Rich Assertion Library**: Standard assertions plus trading-specific assertions for indicators, price action, and series comparisons
- **Visual Feedback**: Test results displayed on charts and in the NinjaTrader Output window
- **Test Context**: Access to market data (OHLCV), bars, instrument info, and custom data storage
- **Flexible Configuration**: Filter tests by namespace, name, bar number, and more

## Quick Start

### 1. Basic Test Example

```csharp
using NinjaTrader.NinjaScript.AddOns.Testing;

namespace NinjaTrader.NinjaScript.Indicators
{
    [NinjaTest]
    public class MyIndicator : Indicator
    {
        [TestCase]
        public void TestIndicatorCalculation()
        {
            // Arrange
            double expected = 100.0;

            // Act
            double actual = Close[0] * 2;

            // Assert
            Assert.AreEqual(expected, actual, 0.01);
        }
    }
}
```
### 2. Running Tests

1. **Add TestRunner to a Chart**:
    - Open a chart in NinjaTrader
    - Right-click ’ Indicators ’ TestRunner

2. **Configure TestRunner Parameters**:
    - **Namespace Filter**: Set to your test namespace (e.g., `"NinjaTrader.NinjaScript.Indicators"`) or empty for all
    - **Start Test At Bar**: Bar number when tests should run (default: 50)
    - **Enable Logging**: `true` to see results in Output window
    - **Show Results On Chart**: `true` for visual feedback

3. **View Results**:
    - Check the Output window for detailed test results
    - Look for green/red markers on the chart
