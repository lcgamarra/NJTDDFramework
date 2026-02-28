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

## Framework Components

### Attributes

#### `[NinjaTest]` - Class Attribute
Marks a class as containing tests. Supports various configuration options:

```csharp
[NinjaTest]
public class BasicTests : NinjaTestBase { }

[NinjaTest(RunAtBar = 100, Category = "Integration")]
public class AdvancedTests : NinjaTestBase { }

[NinjaTest(MinimumBars = 50, RequiredPeriodType = "Minute")]
public class DataTests : NinjaTestBase { }
```

**Properties**:
- `Name`: Display name
- `Description`: Test description
- `RunAtBar`: Specific bar to run tests (default: -1 = use TestRunner setting)
- `Category`: Test category for organization
- `MinimumBars`: Minimum bars required
- `RequiredPeriodType`: Required bar period type
- `RunEveryNBars`: Run tests repeatedly every N bars

#### `[TestCase]` - Method Attribute
Marks a method as a test case:

```csharp
[TestCase]
public void TestMethod() { }

[TestCase(Name = "Custom Name", Description = "What this tests")]
public void DetailedTest() { }

[TestCase(Skip = "Not implemented yet")]
public void SkippedTest() { }
```

### Base Classes

#### `NinjaTestBase`
Inherit from this class for full testing capabilities with market data access:

```csharp
[NinjaTest]
public class MyTests : NinjaTestBase
{
    [TestCase]
    public void TestWithMarketData()
    {
        // Access market data
        double close = Close[0];
        double high = High[0];
        int bar = CurrentBar;

        // Access test context
        Context.Set("myKey", "myValue");
        string value = Context.Get<string>("myKey");

        // Assertions
        Assert.Greater(high, close);
    }
}
```
**Available Properties**:
- `Context`: TestContext instance
- `Close`, `Open`, `High`, `Low`, `Volume`: Price/volume data
- `Median`, `Typical`, `Weighted`: Calculated prices
- `CurrentBar`, `Count`: Bar information
- `Bars`, `Instrument`, `TickSize`: Trading context

### Assertions

#### Standard Assertions

```csharp
// Equality
Assert.AreEqual(expected, actual);
Assert.AreEqual(100.5, actual, 0.1); // With tolerance
Assert.AreNotEqual(notExpected, actual);

// Boolean
Assert.IsTrue(condition);
Assert.IsFalse(condition);

// Null checks
Assert.IsNull(obj);
Assert.IsNotNull(obj);

// Comparisons
Assert.Greater(actual, threshold);
Assert.GreaterOrEqual(actual, threshold);
Assert.Less(actual, threshold);
Assert.LessOrEqual(actual, threshold);

// Exceptions
Assert.Throws<ArgumentException>(() => { /* code */ });
Assert.DoesNotThrow(() => { /* code */ });

// Failure
Assert.Fail("Test failed for reason");
```

#### Trading-Specific Assertions

```csharp
// Indicator values
Assert.IndicatorValue(sma, 100.0, tolerance: 0.01);
Assert.IndicatorValue(sma, barsAgo: 1, expectedValue: 99.5);
Assert.IndicatorIsValid(indicator);

// Series comparisons
Assert.CrossedAbove(sma, ema, barsAgo: 1);
Assert.CrossedBelow(sma, ema, barsAgo: 1);
Assert.SeriesAbove(sma, ema);
Assert.SeriesBelow(sma, ema);

// Price assertions
Assert.PriceInRange(Close[0], 100, 110);
Assert.CloseAbove(Context, 100.0);
Assert.CloseBelow(Context, 110.0);

// Bar patterns
Assert.BullishBar(Context);
Assert.BearishBar(Context);
```