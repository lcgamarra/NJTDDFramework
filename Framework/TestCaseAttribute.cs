//
// TestCaseAttribute.cs - NinjaTrader Testing Framework
// Attribute to mark test methods for execution
//

#region Using declarations

using System;

#endregion

namespace NinjaTrader.NinjaScript.AddOns.Testing
{
    /// <summary>
    /// Marks a method as a test case within a test class.
    /// Methods marked with this attribute will be executed as individual tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestCaseAttribute : Attribute
    {
        /// <summary>
        /// Creates a new TestCase attribute with default settings
        /// </summary>
        public TestCaseAttribute()
        {
            Enabled = true;
        }

        /// <summary>
        /// Display name for this test case. If not set, uses the method name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of what this specific test case tests
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether this test case is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Expected result of the test (for documentation purposes)
        /// </summary>
        public object ExpectedResult { get; set; }

        /// <summary>
        /// Timeout in milliseconds for this specific test case.
        /// Default is 0 (no timeout).
        /// </summary>
        public int TimeoutMs { get; set; }

        /// <summary>
        /// Skip this test with a reason
        /// </summary>
        public string Skip { get; set; }

        /// <summary>
        /// Priority/order of execution within the test class (lower runs first)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Returns true if this test should be skipped
        /// </summary>
        public bool ShouldSkip => !string.IsNullOrEmpty(Skip);

        /// <summary>
        /// Returns a formatted string representation of this attribute
        /// </summary>
        public override string ToString()
        {
            string name = !string.IsNullOrEmpty(Name) ? Name : "Unnamed Test Case";
            string skip = ShouldSkip ? " [SKIPPED]" : "";

            return $"{name}{skip}";
        }
    }
}