//
// Assert.cs - NinjaTrader Testing Framework
// Static assertion class providing standard and trading-specific assertions
//

#region Using declarations

using System;
using System.Collections.Generic;
using System.Linq;
using NinjaTrader.Cbi;
using NinjaTrader.NinjaScript;
using NinjaTrader.Data;

#endregion

namespace NinjaTrader.NinjaScript.AddOns.Testing
{
    /// <summary>
    /// Static assertion class for NinjaTrader unit tests.
    /// Provides both standard assertions (like NUnit/xUnit) and trading-specific assertions.
    /// </summary>
    public static class Assert
    {
        #region Standard Assertions

        /// <summary>
        /// Asserts that two values are equal
        /// </summary>
        public static void AreEqual<T>(T expected, T actual, string message = null)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new AssertionException(
                    message ?? $"Expected: <{expected}>, but was: <{actual}>");
            }
        }

        /// <summary>
        /// Asserts that two double values are equal within a tolerance
        /// </summary>
        public static void AreEqual(double expected, double actual, double tolerance, string message = null)
        {
            if (Math.Abs(expected - actual) > tolerance)
            {
                throw new AssertionException(
                    message ??
                    $"Expected: <{expected}> ±{tolerance}, but was: <{actual}> (difference: {Math.Abs(expected - actual)})");
            }
        }

        /// <summary>
        /// Asserts that two values are not equal
        /// </summary>
        public static void AreNotEqual<T>(T notExpected, T actual, string message = null)
        {
            if (EqualityComparer<T>.Default.Equals(notExpected, actual))
            {
                throw new AssertionException(
                    message ?? $"Expected: not <{notExpected}>, but was: <{actual}>");
            }
        }

        /// <summary>
        /// Asserts that a condition is true
        /// </summary>
        public static void IsTrue(bool condition, string message = null)
        {
            if (!condition)
            {
                throw new AssertionException(message ?? "Expected: True, but was: False");
            }
        }

        /// <summary>
        /// Asserts that a condition is false
        /// </summary>
        public static void IsFalse(bool condition, string message = null)
        {
            if (condition)
            {
                throw new AssertionException(message ?? "Expected: False, but was: True");
            }
        }

        /// <summary>
        /// Asserts that an object is null
        /// </summary>
        public static void IsNull(object obj, string message = null)
        {
            if (obj != null)
            {
                throw new AssertionException(
                    message ?? $"Expected: null, but was: <{obj}>");
            }
        }

        /// <summary>
        /// Asserts that an object is not null
        /// </summary>
        public static void IsNotNull(object obj, string message = null)
        {
            if (obj == null)
            {
                throw new AssertionException(message ?? "Expected: not null, but was: null");
            }
        }

        /// <summary>
        /// Asserts that a value is greater than another
        /// </summary>
        public static void Greater<T>(T actual, T threshold, string message = null) where T : IComparable<T>
        {
            if (actual.CompareTo(threshold) <= 0)
            {
                throw new AssertionException(
                    message ?? $"Expected: <{actual}> > <{threshold}>, but was not");
            }
        }

        /// <summary>
        /// Asserts that a value is greater than or equal to another
        /// </summary>
        public static void GreaterOrEqual<T>(T actual, T threshold, string message = null) where T : IComparable<T>
        {
            if (actual.CompareTo(threshold) < 0)
            {
                throw new AssertionException(
                    message ?? $"Expected: <{actual}> >= <{threshold}>, but was not");
            }
        }

        /// <summary>
        /// Asserts that a value is less than another
        /// </summary>
        public static void Less<T>(T actual, T threshold, string message = null) where T : IComparable<T>
        {
            if (actual.CompareTo(threshold) >= 0)
            {
                throw new AssertionException(
                    message ?? $"Expected: <{actual}> < <{threshold}>, but was not");
            }
        }

        /// <summary>
        /// Asserts that a value is less than or equal to another
        /// </summary>
        public static void LessOrEqual<T>(T actual, T threshold, string message = null) where T : IComparable<T>
        {
            if (actual.CompareTo(threshold) > 0)
            {
                throw new AssertionException(
                    message ?? $"Expected: <{actual}> <= <{threshold}>, but was not");
            }
        }

        /// <summary>
        /// Asserts that an action throws a specific exception type
        /// </summary>
        public static void Throws<TException>(Action action, string message = null)
            where TException : Exception
        {
            try
            {
                action();
                throw new AssertionException(
                    message ?? $"Expected exception of type <{typeof(TException).Name}>, but no exception was thrown");
            }
            catch (TException)
            {
                // Expected exception, test passes
            }
            catch (Exception ex)
            {
                throw new AssertionException(
                    message ??
                    $"Expected exception of type <{typeof(TException).Name}>, but got <{ex.GetType().Name}>: {ex.Message}");
            }
        }

        /// <summary>
        /// Asserts that an action does not throw any exception
        /// </summary>
        public static void DoesNotThrow(Action action, string message = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                throw new AssertionException(
                    message ?? $"Expected no exception, but got <{ex.GetType().Name}>: {ex.Message}");
            }
        }

        /// <summary>
        /// Always fails with a message
        /// </summary>
        public static void Fail(string message = "Test failed")
        {
            throw new AssertionException(message);
        }

        #endregion

        #region Trading-Specific Assertions - Indicators

        /// <summary>
        /// Asserts that an indicator has the expected value at the current bar
        /// </summary>
        public static void IndicatorValue(ISeries<double> indicator, double expectedValue, double tolerance = 0.0001,
            string message = null)
        {
            if (indicator == null)
                throw new ArgumentNullException(nameof(indicator));

            double actualValue = indicator[0];

            if (double.IsNaN(actualValue))
            {
                throw new AssertionException(
                    message ?? "Expected indicator value, but got NaN");
            }

            if (Math.Abs(expectedValue - actualValue) > tolerance)
            {
                throw new AssertionException(
                    message ?? $"Expected indicator value: {expectedValue} ±{tolerance}, but was: {actualValue}");
            }
        }

        /// <summary>
        /// Asserts that an indicator has the expected value at a specific bars ago
        /// </summary>
        public static void IndicatorValue(ISeries<double> indicator, int barsAgo, double expectedValue,
            double tolerance = 0.0001, string message = null)
        {
            if (indicator == null)
                throw new ArgumentNullException(nameof(indicator));

            double actualValue = indicator[barsAgo];

            if (double.IsNaN(actualValue))
            {
                throw new AssertionException(
                    message ?? $"Expected indicator value at bar {barsAgo}, but got NaN");
            }

            if (Math.Abs(expectedValue - actualValue) > tolerance)
            {
                throw new AssertionException(
                    message ??
                    $"Expected indicator value at bar {barsAgo}: {expectedValue} ±{tolerance}, but was: {actualValue}");
            }
        }

        /// <summary>
        /// Asserts that an indicator is not NaN
        /// </summary>
        public static void IndicatorIsValid(ISeries<double> indicator, string message = null)
        {
            if (indicator == null)
                throw new ArgumentNullException(nameof(indicator));

            if (double.IsNaN(indicator[0]))
            {
                throw new AssertionException(
                    message ?? "Expected valid indicator value, but got NaN");
            }
        }

        #endregion

        #region Trading-Specific Assertions - Series Comparisons

        /// <summary>
        /// Asserts that series1 crossed above series2 at specified bars ago
        /// </summary>
        public static void CrossedAbove(ISeries<double> series1, ISeries<double> series2, int barsAgo = 1,
            string message = null)
        {
            if (series1 == null)
                throw new ArgumentNullException(nameof(series1));
            if (series2 == null)
                throw new ArgumentNullException(nameof(series2));

            bool crossAbove = series1[barsAgo] > series2[barsAgo] &&
                              series1[barsAgo + 1] <= series2[barsAgo + 1];

            if (!crossAbove)
            {
                throw new AssertionException(
                    message ?? $"Expected series1 to cross above series2 at bar {barsAgo}. " +
                    $"Series1: [{series1[barsAgo + 1]:F4}] -> [{series1[barsAgo]:F4}], " +
                    $"Series2: [{series2[barsAgo + 1]:F4}] -> [{series2[barsAgo]:F4}]");
            }
        }

        /// <summary>
        /// Asserts that series1 crossed below series2 at specified bars ago
        /// </summary>
        public static void CrossedBelow(ISeries<double> series1, ISeries<double> series2, int barsAgo = 1,
            string message = null)
        {
            if (series1 == null)
                throw new ArgumentNullException(nameof(series1));
            if (series2 == null)
                throw new ArgumentNullException(nameof(series2));

            bool crossBelow = series1[barsAgo] < series2[barsAgo] &&
                              series1[barsAgo + 1] >= series2[barsAgo + 1];

            if (!crossBelow)
            {
                throw new AssertionException(
                    message ?? $"Expected series1 to cross below series2 at bar {barsAgo}. " +
                    $"Series1: [{series1[barsAgo + 1]:F4}] -> [{series1[barsAgo]:F4}], " +
                    $"Series2: [{series2[barsAgo + 1]:F4}] -> [{series2[barsAgo]:F4}]");
            }
        }

        /// <summary>
        /// Asserts that series1 is above series2
        /// </summary>
        public static void SeriesAbove(ISeries<double> series1, ISeries<double> series2, int barsAgo = 0,
            string message = null)
        {
            if (series1 == null)
                throw new ArgumentNullException(nameof(series1));
            if (series2 == null)
                throw new ArgumentNullException(nameof(series2));

            if (series1[barsAgo] <= series2[barsAgo])
            {
                throw new AssertionException(
                    message ??
                    $"Expected series1 ({series1[barsAgo]:F4}) to be above series2 ({series2[barsAgo]:F4}) at bar {barsAgo}");
            }
        }

        /// <summary>
        /// Asserts that series1 is below series2
        /// </summary>
        public static void SeriesBelow(ISeries<double> series1, ISeries<double> series2, int barsAgo = 0,
            string message = null)
        {
            if (series1 == null)
                throw new ArgumentNullException(nameof(series1));
            if (series2 == null)
                throw new ArgumentNullException(nameof(series2));

            if (series1[barsAgo] >= series2[barsAgo])
            {
                throw new AssertionException(
                    message ??
                    $"Expected series1 ({series1[barsAgo]:F4}) to be below series2 ({series2[barsAgo]:F4}) at bar {barsAgo}");
            }
        }

        #endregion

        #region Trading-Specific Assertions - Price

        /// <summary>
        /// Asserts that price is within a range
        /// </summary>
        public static void PriceInRange(double price, double low, double high, string message = null)
        {
            if (price < low || price > high)
            {
                throw new AssertionException(
                    message ?? $"Expected price to be between {low} and {high}, but was: {price}");
            }
        }

        /// <summary>
        /// Asserts that Close is above a specific price
        /// </summary>
        public static void CloseAbove(TestContext context, double price, string message = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            double close = context.Close[0];

            if (close <= price)
            {
                throw new AssertionException(
                    message ?? $"Expected Close ({close}) to be above {price}, but it was not");
            }
        }

        /// <summary>
        /// Asserts that Close is below a specific price
        /// </summary>
        public static void CloseBelow(TestContext context, double price, string message = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            double close = context.Close[0];

            if (close >= price)
            {
                throw new AssertionException(
                    message ?? $"Expected Close ({close}) to be below {price}, but it was not");
            }
        }

        #endregion

        #region Trading-Specific Assertions - Bar Patterns

        /// <summary>
        /// Asserts that the current bar is bullish (Close > Open)
        /// </summary>
        public static void BullishBar(TestContext context, string message = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Close[0] <= context.Open[0])
            {
                throw new AssertionException(
                    message ??
                    $"Expected bullish bar (Close > Open), but Close={context.Close[0]}, Open={context.Open[0]}");
            }
        }

        /// <summary>
        /// Asserts that the current bar is bearish (Close < Open)
        /// </summary>
        public static void BearishBar(TestContext context, string message = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Close[0] >= context.Open[0])
            {
                throw new AssertionException(
                    message ??
                    $"Expected bearish bar (Close < Open), but Close={context.Close[0]}, Open={context.Open[0]}");
            }
        }

        #endregion
    }
}