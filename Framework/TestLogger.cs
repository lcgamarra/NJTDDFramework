//
// TestLogger.cs - NinjaTrader Testing Framework
// Handles logging of test results to the NinjaTrader Output window
//

#region Using declarations

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NinjaTrader.NinjaScript;

#endregion

namespace NinjaTrader.NinjaScript.AddOns.Testing
{
    /// <summary>
    /// Handles logging of test results to the NinjaTrader Output window.
    /// Provides formatted output for test execution results.
    /// </summary>
    public class TestLogger
    {
        #region Variables

        private readonly NinjaScriptBase script;
        private readonly List<string> logBuffer;
        private bool enableTimestamps;
        private bool enableColors;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new TestLogger
        /// </summary>
        /// <param name="script">The NinjaScript that will print the log messages</param>
        public TestLogger(NinjaScriptBase script)
        {
            this.script = script ?? throw new ArgumentNullException(nameof(script));
            this.logBuffer = new List<string>();
            this.enableTimestamps = false;
            this.enableColors = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Enable/disable timestamps in log messages
        /// </summary>
        public bool EnableTimestamps
        {
            get => enableTimestamps;
            set => enableTimestamps = value;
        }

        /// <summary>
        /// Enable/disable color indicators in log messages
        /// </summary>
        public bool EnableColors
        {
            get => enableColors;
            set => enableColors = value;
        }

        #endregion

        #region Methods - Basic Logging

        /// <summary>
        /// Logs a message to the Output window
        /// </summary>
        public void Log(string message)
        {
            string formattedMessage = FormatMessage(message);
            script.Print(formattedMessage);
            logBuffer.Add(formattedMessage);
        }

        /// <summary>
        /// Logs a formatted message to the Output window
        /// </summary>
        public void LogFormat(string format, params object[] args)
        {
            string message = string.Format(format, args);
            Log(message);
        }

        /// <summary>
        /// Logs an empty line
        /// </summary>
        public void LogLine()
        {
            script.Print("");
        }

        /// <summary>
        /// Logs a separator line
        /// </summary>
        public void LogSeparator(char character = '─', int length = 55)
        {
            script.Print(new string(character, length));
        }

        /// <summary>
        /// Logs a header with top and bottom borders
        /// </summary>
        public void LogHeader(string text)
        {
            script.Print("═══════════════════════════════════════════════════════");
            script.Print(text);
            script.Print("═══════════════════════════════════════════════════════");
        }

        #endregion

        #region Methods - Test Result Logging

        /// <summary>
        /// Logs a single test result
        /// </summary>
        public void LogTestResult(TestResult result)
        {
            if (result == null)
                return;

            string icon = GetStatusIcon(result.Status);
            string statusText = result.Status.ToString().ToUpper();
            string timeText = $"({result.ExecutionTime.TotalMilliseconds:F2}ms)";

            script.Print($"  {icon} {result.MethodName} - {statusText} {timeText}");

            // Log error details if failed
            if (result.Failed && !string.IsNullOrEmpty(result.ErrorMessage))
            {
                script.Print($"     Error: {result.ErrorMessage}");

                if (!string.IsNullOrEmpty(result.StackTrace))
                {
                    string[] stackLines = result.StackTrace.Split('\n');
                    if (stackLines.Length > 0)
                    {
                        script.Print($"     Stack: {stackLines[0].Trim()}");
                    }
                }
            }

            // Log skip reason if skipped
            if (result.Skipped && !string.IsNullOrEmpty(result.SkipReason))
            {
                script.Print($"     Reason: {result.SkipReason}");
            }
        }

        /// <summary>
        /// Logs multiple test results grouped by class
        /// </summary>
        public void LogTestResults(IEnumerable<TestResult> results)
        {
            if (results == null || !results.Any())
                return;

            var groupedResults = results.GroupBy(r => r.ClassName);

            foreach (var group in groupedResults)
            {
                script.Print($"\n{group.Key}:");

                foreach (var result in group)
                {
                    LogTestResult(result);
                }
            }
        }

        /// <summary>
        /// Logs a test execution summary
        /// </summary>
        public void LogSummary(int total, int passed, int failed, int skipped)
        {
            LogLine();
            LogSeparator();

            script.Print($"Total: {total} | Passed: {passed} | Failed: {failed} | Skipped: {skipped}");

            if (total > 0)
            {
                double successRate = (passed * 100.0) / total;
                script.Print($"Success Rate: {successRate:F1}%");
            }

            LogSeparator();
        }

        /// <summary>
        /// Logs a detailed test execution summary with timing information
        /// </summary>
        public void LogDetailedSummary(IEnumerable<TestResult> results)
        {
            if (results == null || !results.Any())
                return;

            int total = results.Count();
            int passed = results.Count(r => r.Passed);
            int failed = results.Count(r => r.Failed);
            int skipped = results.Count(r => r.Skipped);

            TimeSpan totalTime = TimeSpan.FromMilliseconds(results.Sum(r => r.ExecutionTime.TotalMilliseconds));
            TimeSpan avgTime = TimeSpan.FromMilliseconds(totalTime.TotalMilliseconds / total);
            TimeSpan maxTime = results.Max(r => r.ExecutionTime);
            TimeSpan minTime = results.Min(r => r.ExecutionTime);

            LogLine();
            LogSeparator();

            script.Print($"Total Tests: {total}");
            script.Print($"Passed: {passed} ({(total > 0 ? passed * 100.0 / total : 0):F1}%)");
            script.Print($"Failed: {failed} ({(total > 0 ? failed * 100.0 / total : 0):F1}%)");
            script.Print($"Skipped: {skipped} ({(total > 0 ? skipped * 100.0 / total : 0):F1}%)");

            LogLine();

            script.Print($"Total Time: {totalTime.TotalMilliseconds:F2}ms");
            script.Print($"Average Time: {avgTime.TotalMilliseconds:F2}ms");
            script.Print($"Min Time: {minTime.TotalMilliseconds:F2}ms");
            script.Print($"Max Time: {maxTime.TotalMilliseconds:F2}ms");

            LogSeparator();
        }

        /// <summary>
        /// Logs a complete test run report
        /// </summary>
        public void LogTestRunReport(IEnumerable<TestResult> results, DateTime startTime, DateTime endTime)
        {
            LogHeader("TestRunner Results");

            script.Print($"Start Time: {startTime:yyyy-MM-dd HH:mm:ss}");
            script.Print($"End Time: {endTime:yyyy-MM-dd HH:mm:ss}");
            script.Print($"Duration: {(endTime - startTime).TotalSeconds:F2}s");

            LogTestResults(results);
            LogDetailedSummary(results);

            LogHeader("End of Report");
        }

        #endregion

        #region Methods - Progress Logging

        /// <summary>
        /// Logs test discovery progress
        /// </summary>
        public void LogDiscovery(int classCount, int methodCount)
        {
            script.Print($"[TestRunner] Discovered {classCount} test class(es) with {methodCount} test method(s)");
        }

        /// <summary>
        /// Logs test execution start
        /// </summary>
        public void LogTestStart(string className, string methodName, int bar)
        {
            script.Print($"[TestRunner] Running {className}.{methodName} at bar {bar}");
        }

        /// <summary>
        /// Logs test execution completion
        /// </summary>
        public void LogTestComplete(string className, string methodName, TestStatus status, double milliseconds)
        {
            string icon = GetStatusIcon(status);
            script.Print($"[TestRunner] {icon} {className}.{methodName} - {status} ({milliseconds:F2}ms)");
        }

        #endregion

        #region Methods - Error Logging

        /// <summary>
        /// Logs an error message
        /// </summary>
        public void LogError(string message)
        {
            script.Print($"[ERROR] {message}");
        }

        /// <summary>
        /// Logs an error with exception details
        /// </summary>
        public void LogError(string message, Exception ex)
        {
            script.Print($"[ERROR] {message}");
            script.Print($"[ERROR] Exception: {ex.GetType().Name} - {ex.Message}");

            if (ex.StackTrace != null)
            {
                string[] stackLines = ex.StackTrace.Split('\n');
                if (stackLines.Length > 0)
                {
                    script.Print($"[ERROR] Stack: {stackLines[0].Trim()}");
                }
            }
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        public void LogWarning(string message)
        {
            script.Print($"[WARNING] {message}");
        }

        #endregion

        #region Methods - Utility

        /// <summary>
        /// Formats a message with optional timestamp
        /// </summary>
        private string FormatMessage(string message)
        {
            if (enableTimestamps)
            {
                return $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
            }

            return message;
        }

        /// <summary>
        /// Gets the status icon for a test status
        /// </summary>
        private string GetStatusIcon(TestStatus status)
        {
            switch (status)
            {
                case TestStatus.Passed:
                    return "✓";
                case TestStatus.Failed:
                case TestStatus.Error:
                    return "✗";
                case TestStatus.Skipped:
                    return "○";
                case TestStatus.Running:
                    return "►";
                case TestStatus.Timeout:
                    return "⏱";
                case TestStatus.Inconclusive:
                    return "?";
                default:
                    return "·";
            }
        }

        /// <summary>
        /// Clears the log buffer
        /// </summary>
        public void Clear()
        {
            logBuffer.Clear();
        }

        /// <summary>
        /// Gets all logged messages
        /// </summary>
        public IReadOnlyList<string> GetLogBuffer()
        {
            return logBuffer.AsReadOnly();
        }

        /// <summary>
        /// Exports the log buffer to a string
        /// </summary>
        public string ExportLog()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string message in logBuffer)
            {
                sb.AppendLine(message);
            }

            return sb.ToString();
        }

        #endregion
    }
}