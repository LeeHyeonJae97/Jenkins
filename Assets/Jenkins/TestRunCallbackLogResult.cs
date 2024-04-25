using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestRunner;

[assembly: TestRunCallback(typeof(TestRunCallbackLogResult))]
public class TestRunCallbackLogResult : ITestRunCallback
{
    public void RunStarted(ITest testsToRun)
    {

    }

    public void RunFinished(ITestResult testResults)
    {
        Log(testResults);
    }

    public void TestStarted(ITest test)
    {

    }

    public void TestFinished(ITestResult result)
    {

    }

    void Log(ITestResult testResults)
    {
        Dictionary<TestStatus, List<ITestResult>> results = new Dictionary<TestStatus, List<ITestResult>>()
        {
            { TestStatus.Passed, new List<ITestResult>() },
            { TestStatus.Failed, new List<ITestResult>() },
            { TestStatus.Skipped, new List<ITestResult>() },
        };

        GetTestMethodResults(testResults);
        Log();

        // quit automatically with return code only when launched in batch mode (for Jenkins)
        if (Application.isBatchMode)
        {
            EditorApplication.Exit(testResults.FailCount == 0 ? 0 : 1);
        }

        void GetTestMethodResults(ITestResult testResult)
        {
            if (testResult.HasChildren)
            {
                foreach (var child in testResult.Children)
                {
                    GetTestMethodResults(child);
                }
            }
            else
            {
                results[testResult.ResultState.Status].Add(testResult);
            }
        }

        void Log()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var result in results)
            {
                if (result.Value.Count == 0) continue;

                if (sb.Length > 0)
                {
                    sb.AppendLine();
                }

                sb.AppendLine($"[{result.Key} Tests]");

                foreach (var value in result.Value)
                {
                    sb.AppendLine($"- {value.Name}");
                }
            }
            sb.AppendLine();

            foreach (var result in results)
            {
                sb.Append($"[{result.Key}]: {result.Value.Count} ");
            }
            sb.AppendLine();

            var resultPath = EditorPrefs.GetString(Args.TestResultPath);

            // remove used key
            EditorPrefs.DeleteKey(Args.TestResultPath);

            if (!Directory.Exists(resultPath))
            {
                Directory.CreateDirectory(resultPath);
            }

            File.WriteAllText(Path.Combine(resultPath, "TestResult.txt"), sb.ToString(), Encoding.UTF8);
        }
    }
}
