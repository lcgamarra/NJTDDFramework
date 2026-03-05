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

### Test Context

The `TestContext` provides runtime state and market data access:

```csharp
[TestCase]
public void UsingTestContext()
{
    // Store/retrieve data
    Context.Set("lastPrice", Close[0]);
    double price = Context.Get<double>("lastPrice");

    // Check data existence
    bool exists = Context.Contains("lastPrice");

    // Access market data
    double close = Context.Close[0];
    int bar = Context.CurrentBar;

    // Print to Output window
    Context.Print("Test message");
}
```

## Advanced Usage

### Testing Indicators

```csharp
[NinjaTest]
public class ADL : Indicator
{
    protected override void OnBarUpdate()
    {
        // Indicator logic
        AD[0] = /* calculation */;
    }

    [TestCase]
    public void TestADCalculation()
    {
        // Test your indicator logic
        Assert.IsNotNull(AD);
        Assert.IndicatorIsValid(AD);
    }

    #region Properties
    [Browsable(false)]
    public Series<double> AD => Values[0];
    #endregion
}
```

### Testing with TestContext

```csharp
[NinjaTest]
public class CrossoverTests : NinjaTestBase
{
    [TestCase]
    public void TestSMACrossover()
    {
        if (CurrentBar < 20)
            return;

        // Test logic with market data
        bool crossAbove = Close[0] > Close[1];
        Assert.IsTrue(crossAbove, "Price should cross above SMA");
    }
}
```

### Running Tests at Specific Bars

```csharp
// Run at bar 100 only
[NinjaTest(RunAtBar = 100)]
public class SpecificBarTest : NinjaTestBase
{
    [TestCase]
    public void TestAtBar100()
    {
        Assert.AreEqual(100, CurrentBar);
    }
}

// Run every 10 bars
[NinjaTest(RunEveryNBars = 10)]
public class RepeatedTest : NinjaTestBase
{
    [TestCase]
    public void TestEvery10Bars()
    {
        // Runs at bars 10, 20, 30, etc.
    }
}
```

### Filtering Tests

Configure TestRunner to filter which tests run:

- **Namespace Filter**: Only run tests in specific namespace
   - `"NinjaTrader.NinjaScript.Indicators"` - Only indicator tests
   - `"NinjaTrader.NinjaScript.Strategies"` - Only strategy tests
   - `""` (empty) - All tests

- **Test Name Filter**: Only run tests with matching names
   - `"SMA"` - Only tests with "SMA" in class name
   - `""` (empty) - All tests

## Test Results

### Output Window Format

```
PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP
TestRunner Results (Bar 50, 2024-01-15 14:30:00)
PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP

MyTestClass:
   TestMethod1 - PASSED (2.45ms)
   TestMethod2 - FAILED (1.23ms)
     Error: Expected: <100>, but was: <95>
     Stack: at MyTestClass.TestMethod2() in ...

                                                       
Total: 2 | Passed: 1 | Failed: 1 | Skipped: 0
Success Rate: 50.0%
PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP
```

### Chart Display

- Green diamond: All tests passed
- Red diamond: Some tests failed
- Text shows: "Tests: X/Y" (passed/total)

## Troubleshooting

### Tests Not Discovered

1. **Check namespace filter**: Make sure TestRunner's "Namespace Filter" matches your test namespace
2. **Verify attributes**: Ensure class has `[NinjaTest]` and methods have `[TestCase]`
3. **Compile code**: Press F5 to compile your NinjaScript
4. **Check bar number**: Tests run at `StartTestAtBar` (default: 50)

### Tests Not Running

1. **Not enough bars**: Test requires at least `StartTestAtBar` bars loaded
2. **RunAtBar mismatch**: Check if `[NinjaTest(RunAtBar = X)]` matches current bar
3. **RunTestsOnce = true**: Tests only run once; set to false for repeated execution

### Accessing Market Data

If you get null reference errors:
- Inherit from `NinjaTestBase` instead of plain class
- Check `CurrentBar >= MinimumBars` before accessing series
- Ensure chart has sufficient historical data loaded

## File Structure

```
AddOns/Testing/Framework/
   Assert.cs               # Assertion library
   AssertionException.cs   # Exception for failed assertions
   NinjaTestAttribute.cs   # Test attributes
   NinjaTestBase.cs        # Base class for tests
   TestCaseAttribute.cs    # Test method attribute
   TestContext.cs          # Test runtime context
   TestLogger.cs           # Logging functionality
   TestResult.cs           # Test result data
   TestRunner.cs           # Main test runner indicator
```

## Best Practices

1. **Use descriptive test names**: `TestSMA_CrossesAbove_WhenPriceRises()`
2. **One assertion per test**: Keep tests focused and simple
3. **Check bar count**: Always verify enough bars exist before testing
4. **Use tolerance for doubles**: Floating-point comparisons need tolerance
5. **Organize with categories**: Use `Category` property for grouping
6. **Test edge cases**: Test with minimum bars, zero values, etc.

## Example: Complete Test Class

```csharp
using NinjaTrader.NinjaScript.AddOns.Testing;

namespace NinjaTrader.NinjaScript.AddOns.Tests
{
    [NinjaTest(
        Name = "SMA Indicator Tests",
        Description = "Tests for Simple Moving Average indicator",
        Category = "Indicators",
        MinimumBars = 20
    )]
    public class SMATests : NinjaTestBase
    {
        [TestCase(Name = "SMA calculates correctly")]
        public void TestSMACalculation()
        {
            if (CurrentBar < 20)
                return;

            // Calculate expected SMA manually
            double sum = 0;
            for (int i = 0; i < 14; i++)
            {
                sum += Close[i];
            }
            double expected = sum / 14;

            // Get actual SMA value (assuming SMA indicator exists)
            // double actual = SMA(14)[0];

            // Assert.AreEqual(expected, actual, 0.01);
        }

        [TestCase(Name = "Price crosses above SMA")]
        public void TestCrossAbove()
        {
            if (CurrentBar < 20)
                return;

            // Test crossover logic
            bool crossed = Close[0] > Close[1];
            Assert.IsTrue(crossed || !crossed); // Placeholder
        }
    }
}
```

## License

This framework is designed for use with NinjaTrader 8 platform.

## Support

For issues or questions, please send an email to lc@silveralgo.com