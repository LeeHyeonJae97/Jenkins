using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

public class Jenkins
{
    [MenuItem("Jenkins/Build Player")]
    static void BuildPlayer()
    {
        var args = new Args();

        SetScriptingDefineSymbol();
        SetVersion();

        var report = BuildPlayer();

        if (Application.isBatchMode)
        {
            EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
        }

        void SetScriptingDefineSymbol()
        {
            var scriptingDefineSymbol = args.GetStrArg(Args.ScriptingDefineSymbol);
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, scriptingDefineSymbol);
        }

        void SetVersion()
        {
            PlayerSettings.bundleVersion = args.GetStrArg(Args.BundleVersion);

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                Debug.Log($"BundleVersionCode : {args.GetIntArg(Args.BundleVersionCode)}");

                PlayerSettings.Android.bundleVersionCode = args.GetIntArg(Args.BundleVersionCode);
            }
        }

        BuildReport BuildPlayer()
        {
            var buildPath = args.GetStrArg(Args.BuildPath);

            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            var buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = GetScenesFromBuildSettings(),
                target = EditorUserBuildSettings.activeBuildTarget,
                targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup,
                locationPathName = Path.Combine(buildPath, GetFileNameWithExtension(PlayerSettings.bundleVersion)),
                options = args.GetBoolArg(Args.Dev) ? BuildOptions.Development : BuildOptions.None,
            };

            return BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        string[] GetScenesFromBuildSettings()
        {
            List<string> scenes = new List<string>();

            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                {
                    scenes.Add(EditorBuildSettings.scenes[i].path);
                }
            }
            return scenes.ToArray();
        }

        string GetFileNameWithExtension(string fileName)
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return $"{fileName}.exe";

                case BuildTarget.Android:
                    return $"{fileName}.apk";
            }
            return "";
        }
    }

    [MenuItem("Jenkins/Build AssetBundle")]
    static void BuildAssetBundle()
    {

    }

    [MenuItem("Jenkins/RunTest")]
    static void RunTest()
    {
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.PlayMode,
        };

        var args = new Args();

        // save key to use at TestCallback.RunFinished()
        EditorPrefs.SetString(Args.TestResultPath, args.GetStrArg(Args.TestResultPath));

        testRunnerApi.Execute(new ExecutionSettings(filter));
    }
}
