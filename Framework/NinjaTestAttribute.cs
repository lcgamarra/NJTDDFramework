//
// NinjaTestAttribute.cs - NinjaTrader Testing Framework
// Attribute to mark test classes for runtime discovery and execution
//

#region Using declarations
using System;
#endregion

namespace NinjaTrader.NinjaScript.AddOns.Testing
{
    /// <summary>
    /// Marks a class as a NinjaTrader test class.
    /// Test classes marked with this attribute will be discovered and executed by TestRunner.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NinjaTestAttribute : Attribute
    {
        /// <summary>
        /// Creates a new NinjaTest attribute with default settings
        /// </summary>
        public NinjaTestAttribute()
        {
            RunAtBar = -1; // -1 means run at StartTestAtBar (from TestRunner)
            Enabled = true;
            Category = "General";
        }
        
        /// <summary>
        /// The display name of the test class. If not set, uses the class name.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Description of what this test class tests
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// The specific bar number at which this test should run.
        /// Set to -1 to use TestRunner's StartTestAtBar parameter (default).
        /// Set to a specific bar number to run at that bar only.
        /// </summary>
        /// <example>
        /// [NinjaTest(RunAtBar = 100)] // Run only at bar 100
        /// [NinjaTest(RunAtBar = -1)]  // Run at TestRunner's configured bar
        /// </example>
        public int RunAtBar { get; set; }
        
        /// <summary>
        /// Category for organizing tests (e.g., "Indicators", "Strategies", "Integration")
        /// </summary>
        public string Category { get; set; }
        
        /// <summary>
        /// Whether this test is enabled. Set to false to temporarily disable a test.
        /// </summary>
        public bool Enabled { get; set; }
        
        /// <summary>
        /// Author of the test
        /// </summary>
        public string Author { get; set; }
        
        /// <summary>
        /// Timeout in milliseconds for the entire test class.
        /// If all test methods take longer than this, the test class will be marked as failed.
        /// Default is 0 (no timeout).
        /// </summary>
        public int TimeoutMs { get; set; }
        
        /// <summary>
        /// Minimum bar count required before this test can run.
        /// Test will be skipped if CurrentBar is less than this value.
        /// </summary>
        /// <example>
        /// [NinjaTest(MinimumBars = 50)] // Requires at least 50 bars of data
        /// </example>
        public int MinimumBars { get; set; }
        
        /// <summary>
        /// Specific bar period type required for this test (e.g., "Minute", "Day", "Tick").
        /// If set, test will only run on charts of this period type.
        /// Leave empty to run on any period type.
        /// </summary>
        /// <example>
        /// [NinjaTest(RequiredPeriodType = "Minute")] // Only run on minute charts
        /// </example>
        public string RequiredPeriodType { get; set; }
        
        /// <summary>
        /// Specific bar period value required (e.g., 5 for 5-minute charts).
        /// Only used if RequiredPeriodType is also set.
        /// </summary>
        /// <example>
        /// [NinjaTest(RequiredPeriodType = "Minute", RequiredPeriodValue = 5)] // Only on 5-min charts
        /// </example>
        public int RequiredPeriodValue { get; set; }
        
        /// <summary>
        /// Run this test only if the specified pattern/condition was detected.
        /// This is an optional filter that can be used for pattern-based testing.
        /// The TestRunner can use this to conditionally execute tests.
        /// </summary>
        /// <example>
        /// [NinjaTest(RequireCondition = "BullishEngulfing")] // Only run if bullish engulfing detected
        /// </example>
        public string RequireCondition { get; set; }
        
        /// <summary>
        /// Run this test every N bars instead of just once.
        /// Set to 1 to run every bar (use with caution - performance impact).
        /// Set to 10 to run every 10 bars.
        /// Default is 0 (run once based on RunAtBar or StartTestAtBar).
        /// </summary>
        /// <example>
        /// [NinjaTest(RunEveryNBars = 10)] // Run every 10 bars
        /// </example>
        public int RunEveryNBars { get; set; }
        
        /// <summary>
        /// Priority/order in which tests should run (lower numbers run first).
        /// Default is 0. Useful when tests have dependencies or setup requirements.
        /// </summary>
        public int Priority { get; set; }
        
        /// <summary>
        /// Tags for filtering and organizing tests (comma-separated).
        /// Can be used by TestRunner to filter which tests to run.
        /// </summary>
        /// <example>
        /// [NinjaTest(Tags = "fast,unit,critical")]
        /// [NinjaTest(Tags = "integration,slow")]
        /// </example>
        public string Tags { get; set; }
        
        /// <summary>
        /// Checks if a specific tag is present in the Tags property
        /// </summary>
        public bool HasTag(string tag)
        {
            if (string.IsNullOrEmpty(Tags) || string.IsNullOrEmpty(tag))
                return false;
            
            string[] tags = Tags.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string t in tags)
            {
                if (t.Trim().Equals(tag, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Returns a formatted string representation of this attribute
        /// </summary>
        public override string ToString()
        {
            string name = !string.IsNullOrEmpty(Name) ? Name : "Unnamed Test";
            string category = !string.IsNullOrEmpty(Category) ? $" ({Category})" : "";
            string runInfo = RunAtBar > 0 ? $" @ Bar {RunAtBar}" : "";
            
            return $"{name}{category}{runInfo}";
        }
    }
    
    /// <summary>
    /// Marks a method as a test case within a test class.
    /// Methods marked with this attribute will be executed as individual tests.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NinjaTestCaseAttribute : Attribute
    {
        /// <summary>
        /// Creates a new TestCase attribute with default settings
        /// </summary>
        public NinjaTestCaseAttribute()
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
    
    /// <summary>
    /// Marks a method to be run before each test case in the class.
    /// Similar to NUnit's [SetUp] or xUnit's constructor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestSetUpAttribute : Attribute
    {
    }
    
    /// <summary>
    /// Marks a method to be run after each test case in the class.
    /// Similar to NUnit's [TearDown] or xUnit's Dispose.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestTearDownAttribute : Attribute
    {
    }
    
    /// <summary>
    /// Marks a method to be run once before all test cases in the class.
    /// Similar to NUnit's [OneTimeSetUp] or xUnit's class fixture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestClassSetUpAttribute : Attribute
    {
    }
    
    /// <summary>
    /// Marks a method to be run once after all test cases in the class.
    /// Similar to NUnit's [OneTimeTearDown].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestClassTearDownAttribute : Attribute
    {
    }
    
    /// <summary>
    /// Marks a test as testing a specific known issue or bug.
    /// Useful for tracking tests related to bug fixes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class BugAttribute : Attribute
    {
        public BugAttribute(string bugId)
        {
            BugId = bugId;
        }
        
        /// <summary>
        /// Bug tracking ID (e.g., "BUG-123", "JIRA-456")
        /// </summary>
        public string BugId { get; set; }
        
        /// <summary>
        /// Description of the bug
        /// </summary>
        public string Description { get; set; }
        
        public override string ToString()
        {
            return !string.IsNullOrEmpty(Description) 
                ? $"Bug {BugId}: {Description}" 
                : $"Bug {BugId}";
        }
    }
    
    /// <summary>
    /// Marks a test as performance-critical.
    /// Can specify maximum execution time allowed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PerformanceBenchmarkAttribute : Attribute
    {
        public PerformanceBenchmarkAttribute()
        {
        }
        
        public PerformanceBenchmarkAttribute(int maxExecutionMs)
        {
            MaxExecutionMs = maxExecutionMs;
        }
        
        /// <summary>
        /// Maximum allowed execution time in milliseconds
        /// </summary>
        public int MaxExecutionMs { get; set; }
        
        /// <summary>
        /// Whether to fail the test if it exceeds MaxExecutionMs (default: true)
        /// </summary>
        public bool FailOnTimeout { get; set; } = true;
    }
}