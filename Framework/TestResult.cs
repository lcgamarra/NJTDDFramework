//
// TestResult.cs - NinjaTrader Testing Framework
// Represents the result of a test execution
//

#region Using declarations

using System;
using System.Text;

#endregion

namespace NinjaTrader.NinjaScript.AddOns.Testing
{
    /// <summary>
    /// Represents the result of a test execution.
    /// Contains information about test status, timing, errors, and context.
    /// </summary>
    public class TestResult
    {
        #region Constructor

        /// <summary>
        /// Creates a new TestResult
        /// </summary>
        public TestResult()
        {
            Status = TestStatus.Pending;
            ExecutionTime = TimeSpan.Zero;
        }

        #endregion

        #region Properties - Test Identification

        /// <summary>
        /// Name of the test class
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Name of the test method
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Full name of the test (ClassName.MethodName)
        /// </summary>
        public string FullName => $"{ClassName}.{MethodName}";

        /// <summary>
        /// Display name of the test (if specified in attribute)
        /// </summary>
        public string DisplayName { get; set; }

        #endregion

        #region Properties - Test Status

        /// <summary>
        /// Status of the test execution (Passed, Failed, Skipped, etc.)
        /// </summary>
        public TestStatus Status { get; set; }

        /// <summary>
        /// Returns true if the test passed
        /// </summary>
        public bool Passed => Status == TestStatus.Passed;

        /// <summary>
        /// Returns true if the test failed
        /// </summary>
        public bool Failed => Status == TestStatus.Failed;

        /// <summary>
        /// Returns true if the test was skipped
        /// </summary>
        public bool Skipped => Status == TestStatus.Skipped;

        #endregion

        #region Properties - Error Information

        /// <summary>
        /// Error message if the test failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Stack trace if the test failed
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Exception that caused the test failure (if any)
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Skip reason if the test was skipped
        /// </summary>
        public string SkipReason { get; set; }

        #endregion

        #region Properties - Timing

        /// <summary>
        /// Time taken to execute the test
        /// </summary>
        public TimeSpan ExecutionTime { get; set; }

        /// <summary>
        /// Start time of test execution
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// End time of test execution
        /// </summary>
        public DateTime EndTime { get; set; }

        #endregion

        #region Properties - Context

        /// <summary>
        /// Bar number at which the test was executed
        /// </summary>
        public int Bar { get; set; }

        /// <summary>
        /// Timestamp of the bar at which the test was executed
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Instrument being tested
        /// </summary>
        public string Instrument { get; set; }

        /// <summary>
        /// Bar period type (Minute, Day, Tick, etc.)
        /// </summary>
        public string BarPeriodType { get; set; }

        /// <summary>
        /// Bar period value (e.g., 5 for 5-minute bars)
        /// </summary>
        public int BarPeriodValue { get; set; }

        #endregion

        #region Properties - Additional Information

        /// <summary>
        /// Test category (if specified)
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Test tags (if specified)
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Custom output message from the test
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// Number of assertions executed in the test
        /// </summary>
        public int AssertionCount { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a formatted string representation of the test result
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // Status icon
            string icon = Status == TestStatus.Passed ? "✓" :
                Status == TestStatus.Failed ? "✗" :
                Status == TestStatus.Skipped ? "○" : "?";

            sb.Append($"{icon} {FullName}");

            if (!string.IsNullOrEmpty(DisplayName))
            {
                sb.Append($" [{DisplayName}]");
            }

            sb.Append($" - {Status.ToString().ToUpper()}");
            sb.Append($" ({ExecutionTime.TotalMilliseconds:F2}ms)");

            return sb.ToString();
        }

        /// <summary>
        /// Returns a detailed string representation including error information
        /// </summary>
        public string ToDetailedString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(ToString());
            sb.AppendLine($"  Class: {ClassName}");
            sb.AppendLine($"  Method: {MethodName}");
            sb.AppendLine($"  Bar: {Bar} @ {Time:yyyy-MM-dd HH:mm:ss}");

            if (!string.IsNullOrEmpty(Instrument))
            {
                sb.AppendLine($"  Instrument: {Instrument}");
            }

            if (!string.IsNullOrEmpty(BarPeriodType))
            {
                sb.AppendLine($"  Period: {BarPeriodValue} {BarPeriodType}");
            }

            if (!string.IsNullOrEmpty(Category))
            {
                sb.AppendLine($"  Category: {Category}");
            }

            if (Failed)
            {
                sb.AppendLine($"  Error: {ErrorMessage}");

                if (!string.IsNullOrEmpty(StackTrace))
                {
                    sb.AppendLine($"  Stack Trace:");
                    sb.AppendLine($"    {StackTrace.Replace("\n", "\n    ")}");
                }
            }

            if (Skipped && !string.IsNullOrEmpty(SkipReason))
            {
                sb.AppendLine($"  Skip Reason: {SkipReason}");
            }

            if (!string.IsNullOrEmpty(Output))
            {
                sb.AppendLine($"  Output:");
                sb.AppendLine($"    {Output.Replace("\n", "\n    ")}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a short summary suitable for display in limited space
        /// </summary>
        public string ToShortString()
        {
            string icon = Status == TestStatus.Passed ? "✓" :
                Status == TestStatus.Failed ? "✗" :
                Status == TestStatus.Skipped ? "○" : "?";

            return $"{icon} {MethodName} ({ExecutionTime.TotalMilliseconds:F0}ms)";
        }

        #endregion
    }

    /// <summary>
    /// Represents the status of a test execution
    /// </summary>
    public enum TestStatus
    {
        /// <summary>
        /// Test has not yet been executed
        /// </summary>
        Pending,

        /// <summary>
        /// Test is currently running
        /// </summary>
        Running,

        /// <summary>
        /// Test passed successfully
        /// </summary>
        Passed,

        /// <summary>
        /// Test failed due to assertion failure or exception
        /// </summary>
        Failed,

        /// <summary>
        /// Test was skipped
        /// </summary>
        Skipped,

        /// <summary>
        /// Test execution was inconclusive
        /// </summary>
        Inconclusive,

        /// <summary>
        /// Test timed out
        /// </summary>
        Timeout,

        /// <summary>
        /// Test execution resulted in an error
        /// </summary>
        Error
    }
}