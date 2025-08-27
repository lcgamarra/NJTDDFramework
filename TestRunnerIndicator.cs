using System.Collections.Generic;

namespace NinjaTrader.NinjaScript.Indicators.Testing
{
    public class TestRunnerIndicator: Indicator
    {
        private TestRunnerConfig Config;
        private TestRegistry Registry;
        private ITestLogger Logger;
        private string RunId;
        private RunnerStatus RunStatus;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar == 0)
            {
                RunAll();
            }
        }

        void RunAll()
        {
            
        }

        void PrintSummary(TestRunSummary summary)
        {
            Log(summary.ToString());
        }

        void PrintFailures(IEnumerable<TestCaseResult> failures)
        {
            foreach (var failure in failures)
            {
                Log(failure.ToString());
            }
        }
    }
}