//
// TestRunner.cs - NinjaTrader Testing Framework
// Executes unit tests during bar updates at runtime
//

#region Using declarations

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.Data;
using System.Xml.Serialization;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.NinjaScript.Indicators;

#endregion

namespace NinjaTrader.NinjaScript.AddOns.Testing
{
    /// <summary>
    /// TestRunner is an Indicator that discovers and executes NinjaTrader tests at runtime.
    /// Add this indicator to a chart to run your tests against live or historical data.
    /// </summary>
    public class TestRunner : Indicator
    {
        #region Variables

        private List<TestClass> discoveredTests;
        private List<TestResult> testResults;
        private bool testsHaveRun;
        private bool isInitialized;
        private TestContext testContext;

        // Statistics
        private int totalTests;
        private int passedTests;
        private int failedTests;
        private int skippedTests;

        #endregion

        #region OnStateChange

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "Runtime test runner for NinjaTrader unit tests";
                Name = "TestRunner";
                Calculate = Calculate.OnBarClose;
                IsOverlay = true;
                DisplayInDataBox = true;
                DrawOnPricePanel = true;
                ScaleJustification = ScaleJustification.Right;
                IsSuspendedWhileInactive = true;

                // Parameters
                StartTestAtBar = 50;
                RunTestsOnce = true;
                EnableLogging = true;
                ShowResultsOnChart = true;
                TestNamespaceFilter = "NinjaTrader.NinjaScript.AddOns.Tests";
                RunOnlyTestsMatching = "";

                // Initialize collections
                discoveredTests = new List<TestClass>();
                testResults = new List<TestResult>();
                testsHaveRun = false;
                isInitialized = false;
            }
            else if (State == State.Configure)
            {
                // Nothing to configure
            }
            else if (State == State.DataLoaded)
            {
                // Discover all test classes when data is loaded
                DiscoverTests();

                // Create test context
                testContext = new TestContext(this);

                isInitialized = true;

                if (EnableLogging)
                {
                    Print(
                        $"TestRunner: Discovered {discoveredTests.Count} test classes with {totalTests} test methods");
                }
            }
            else if (State == State.Terminated)
            {
                // Print final summary
                if (EnableLogging && testsHaveRun)
                {
                    PrintFinalSummary();
                }
            }
        }

        #endregion

        #region OnBarUpdate

        protected override void OnBarUpdate()
        {
            if (!isInitialized)
                return;

            // Check if we should run tests at this bar
            if (CurrentBar < StartTestAtBar)
                return;

            // If RunTestsOnce is true and tests have already run, skip
            if (RunTestsOnce && testsHaveRun)
                return;

            // Run all discovered tests
            RunAllTests();

            testsHaveRun = true;

            // Display results on chart if enabled
            if (ShowResultsOnChart)
            {
                DrawResultsOnChart();
            }

            // Print results to Output window
            if (EnableLogging)
            {
                PrintTestResults();
            }
        }

        #endregion

        #region Test Discovery

        /// <summary>
        /// Discovers all test classes marked with [NinjaTest] attribute
        /// </summary>
        private void DiscoverTests()
        {
            discoveredTests.Clear();
            totalTests = 0;

            try
            {
                // Get all assemblies in the current domain
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assembly in assemblies)
                {
                    try
                    {
                        // Get all types in the assembly
                        Type[] types = assembly.GetTypes();

                        foreach (Type type in types)
                        {
                            // Check if type has [NinjaTest] attribute
                            var testAttribute = type.GetCustomAttribute<NinjaTestAttribute>();

                            if (testAttribute != null)
                            {
                                // Apply namespace filter if specified
                                if (!string.IsNullOrEmpty(TestNamespaceFilter) &&
                                    !type.Namespace?.StartsWith(TestNamespaceFilter) == true)
                                {
                                    continue;
                                }

                                // Apply name filter if specified
                                if (!string.IsNullOrEmpty(RunOnlyTestsMatching) &&
                                    !type.Name.Contains(RunOnlyTestsMatching))
                                {
                                    continue;
                                }

                                // Check if this test should run at current bar
                                if (testAttribute.RunAtBar > 0 && testAttribute.RunAtBar != CurrentBar)
                                {
                                    continue;
                                }

                                // Discover test methods in this class
                                var testClass = new TestClass
                                {
                                    Type = type,
                                    Attribute = testAttribute,
                                    TestMethods = new List<TestMethod>()
                                };

                                // Find all methods with [TestCase] attribute
                                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                                foreach (MethodInfo method in methods)
                                {
                                    var testCaseAttribute = method.GetCustomAttribute<TestCaseAttribute>();

                                    if (testCaseAttribute != null)
                                    {
                                        testClass.TestMethods.Add(new TestMethod
                                        {
                                            Method = method,
                                            Attribute = testCaseAttribute
                                        });

                                        totalTests++;
                                    }
                                }

                                if (testClass.TestMethods.Count > 0)
                                {
                                    discoveredTests.Add(testClass);
                                }
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // Skip assemblies that can't be loaded
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Print($"TestRunner Error during test discovery: {ex.Message}");
            }
        }

        #endregion

        #region Test Execution

        /// <summary>
        /// Runs all discovered tests
        /// </summary>
        private void RunAllTests()
        {
            testResults.Clear();
            passedTests = 0;
            failedTests = 0;
            skippedTests = 0;

            foreach (TestClass testClass in discoveredTests)
            {
                RunTestClass(testClass);
            }
        }

        /// <summary>
        /// Runs all test methods in a test class
        /// </summary>
        private void RunTestClass(TestClass testClass)
        {
            try
            {
                // Create instance of test class
                object testInstance = Activator.CreateInstance(testClass.Type);

                // If it derives from NinjaTestBase, initialize it with context
                if (testInstance is NinjaTestBase testBase)
                {
                    testBase.Initialize(testContext);
                }

                // Run each test method
                foreach (TestMethod testMethod in testClass.TestMethods)
                {
                    RunTestMethod(testClass, testInstance, testMethod);
                }
            }
            catch (Exception ex)
            {
                // Failed to instantiate test class
                var result = new TestResult
                {
                    ClassName = testClass.Type.Name,
                    MethodName = "<constructor>",
                    Status = TestStatus.Failed,
                    ErrorMessage = $"Failed to create test instance: {ex.Message}",
                    ExecutionTime = TimeSpan.Zero,
                    Bar = CurrentBar,
                    Time = Time[0]
                };

                testResults.Add(result);
                failedTests++;
            }
        }

        /// <summary>
        /// Runs a single test method
        /// </summary>
        private void RunTestMethod(TestClass testClass, object testInstance, TestMethod testMethod)
        {
            var result = new TestResult
            {
                ClassName = testClass.Type.Name,
                MethodName = testMethod.Method.Name,
                Bar = CurrentBar,
                Time = Time[0]
            };

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                // Reset test context state before each test
                testContext.Reset();

                // Invoke the test method
                testMethod.Method.Invoke(testInstance, null);

                // Test passed
                result.Status = TestStatus.Passed;
                passedTests++;
            }
            catch (TargetInvocationException tie)
            {
                // Unwrap the real exception
                Exception innerException = tie.InnerException ?? tie;

                if (innerException is AssertionException assertEx)
                {
                    // Test failed due to assertion
                    result.Status = TestStatus.Failed;
                    result.ErrorMessage = assertEx.Message;
                    failedTests++;
                }
                else
                {
                    // Test failed due to unexpected exception
                    result.Status = TestStatus.Failed;
                    result.ErrorMessage =
                        $"Unexpected exception: {innerException.GetType().Name} - {innerException.Message}";
                    result.StackTrace = innerException.StackTrace;
                    failedTests++;
                }
            }
            catch (Exception ex)
            {
                // Test failed
                result.Status = TestStatus.Failed;
                result.ErrorMessage = ex.Message;
                result.StackTrace = ex.StackTrace;
                failedTests++;
            }
            finally
            {
                stopwatch.Stop();
                result.ExecutionTime = stopwatch.Elapsed;
            }

            testResults.Add(result);
        }

        #endregion

        #region Output and Display

        /// <summary>
        /// Prints test results to Output window
        /// </summary>
        private void PrintTestResults()
        {
            Print("═══════════════════════════════════════════════════════");
            Print($"TestRunner Results (Bar {CurrentBar}, {Time[0]:yyyy-MM-dd HH:mm:ss})");
            Print("═══════════════════════════════════════════════════════");

            // Group results by class
            var groupedResults = testResults.GroupBy(r => r.ClassName);

            foreach (var group in groupedResults)
            {
                Print($"\n{group.Key}:");

                foreach (var result in group)
                {
                    string statusIcon = result.Status == TestStatus.Passed ? "✓" : "✗";
                    string statusText = result.Status.ToString().ToUpper();

                    Print(
                        $"  {statusIcon} {result.MethodName} - {statusText} ({result.ExecutionTime.TotalMilliseconds:F2}ms)");

                    if (result.Status == TestStatus.Failed)
                    {
                        Print($"     Error: {result.ErrorMessage}");

                        if (!string.IsNullOrEmpty(result.StackTrace))
                        {
                            Print($"     Stack: {result.StackTrace.Split('\n')[0]}");
                        }
                    }
                }
            }

            Print("\n───────────────────────────────────────────────────────");
            Print($"Total: {totalTests} | Passed: {passedTests} | Failed: {failedTests} | Skipped: {skippedTests}");
            Print($"Success Rate: {(totalTests > 0 ? (passedTests * 100.0 / totalTests) : 0):F1}%");
            Print("═══════════════════════════════════════════════════════\n");
        }

        /// <summary>
        /// Prints final summary when indicator is terminated
        /// </summary>
        private void PrintFinalSummary()
        {
            Print("\n═══════════════════════════════════════════════════════");
            Print("TestRunner Final Summary");
            Print("═══════════════════════════════════════════════════════");
            Print($"Tests Run: {totalTests}");
            Print($"Passed: {passedTests}");
            Print($"Failed: {failedTests}");
            Print($"Success Rate: {(totalTests > 0 ? (passedTests * 100.0 / totalTests) : 0):F1}%");
            Print("═══════════════════════════════════════════════════════\n");
        }

        /// <summary>
        /// Draws test results on the chart
        /// </summary>
        private void DrawResultsOnChart()
        {
            // Draw a marker on the chart showing test results
            string resultText = $"Tests: {passedTests}/{totalTests}";

            if (failedTests > 0)
            {
                Draw.Text(this, $"TestResult_{CurrentBar}", resultText, 0, High[0] + (TickSize * 10), Brushes.Red);
                Draw.Diamond(this, $"TestMarker_{CurrentBar}", true, 0, High[0] + (TickSize * 5), Brushes.Red);
            }
            else
            {
                Draw.Text(this, $"TestResult_{CurrentBar}", resultText, 0, High[0] + (TickSize * 10), Brushes.Green);
                Draw.Diamond(this, $"TestMarker_{CurrentBar}", true, 0, High[0] + (TickSize * 5), Brushes.Green);
            }
        }

        #endregion

        #region Properties

        [NinjaScriptProperty]
        [Range(1, int.MaxValue)]
        [Display(Name = "Start Test At Bar", Description = "Bar number at which to start running tests", Order = 1,
            GroupName = "Parameters")]
        public int StartTestAtBar { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Run Tests Once",
            Description = "If true, tests run only once. If false, tests run on every bar.", Order = 2,
            GroupName = "Parameters")]
        public bool RunTestsOnce { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Enable Logging", Description = "Print test results to Output window", Order = 3,
            GroupName = "Parameters")]
        public bool EnableLogging { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Show Results On Chart", Description = "Draw test results on the chart", Order = 4,
            GroupName = "Parameters")]
        public bool ShowResultsOnChart { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Namespace Filter", Description = "Only run tests in this namespace (empty = all)", Order = 5,
            GroupName = "Filters")]
        public string TestNamespaceFilter { get; set; }

        [NinjaScriptProperty]
        [Display(Name = "Test Name Filter",
            Description = "Only run tests with names containing this text (empty = all)", Order = 6,
            GroupName = "Filters")]
        public string RunOnlyTestsMatching { get; set; }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Represents a discovered test class
    /// </summary>
    internal class TestClass
    {
        public Type Type { get; set; }
        public NinjaTestAttribute Attribute { get; set; }
        public List<TestMethod> TestMethods { get; set; }
    }

    /// <summary>
    /// Represents a test method within a test class
    /// </summary>
    internal class TestMethod
    {
        public MethodInfo Method { get; set; }
        public TestCaseAttribute Attribute { get; set; }
    }
    

    #endregion
}