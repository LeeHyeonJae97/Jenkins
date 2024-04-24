using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

public class Jenkins
{
    [MenuItem("Jenkins/RunTest")]
    public static void RunTest()
    {
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.PlayMode            
        };        

        testRunnerApi.Execute(new ExecutionSettings(filter));
    }

    [MenuItem("Jenkins/BuilPlayer")]
    public static void BuildPlayer()
    {
        var args = new Args();

        var target = EditorUserBuildSettings.activeBuildTarget;
        var path = $"Build/Player/{target}";
        var option = BuildOptions.None;

        if (args.TryGetBooleanArg("dev", out var enableDev) && enableDev)
        {
            option &= BuildOptions.Development;
        }

        var buildPlayerOption = new BuildPlayerOptions()
        {
            scenes = GetScenesFromBuildSettings(),
            target = target,
            targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup,
            locationPathName = $"{path}/{GetFileNameWithExtension("Build")}",
            options = option,
        };

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var report = BuildPipeline.BuildPlayer(buildPlayerOption);

        if (Application.isBatchMode)
        {
            EditorApplication.Exit(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
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
            string extension = "";
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";

                case BuildTarget.Android:
                    return ".apk";
            }

            return $"{fileName}{extension}";
        }
    }

    public static void BuildAssetBundle()
    {

    }

    class Args
    {
        Dictionary<string, string> _dic;

        public Args() 
        {
            if (_dic == null)
            {
                _dic = new Dictionary<string, string>();
            }

            var args = Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith('-') && !args[i + 1].StartsWith('-') && !string.IsNullOrEmpty(args[i + 1]))
                {
                    var key = args[i].Substring(1);
                    var value = args[i + 1];

                    _dic[key] = value;
                }
            }
        }

        public bool TryGetBooleanArg(string key, out bool value)
        {
            value = default;

            return _dic != null && _dic.TryGetValue(key, out var arg) && bool.TryParse(arg, out value);
        }

        public bool TryGetIntArg(string key, out int value)
        {
            value = default;

            return _dic != null && _dic.TryGetValue(key, out var arg) && int.TryParse(arg, out value);
        }

        public bool TryGetFloatArg(string key, out float value)
        {
            value = default;

            return _dic != null && _dic.TryGetValue(key, out var arg) && float.TryParse(arg, out value);
        }

        public bool TryGetEnumArg<T>(string key, out T value) where T : Enum
        {
            value = default;

            if (_dic != null && _dic.TryGetValue(key, out var arg))
            {
                value = Enum.Parse<T>(arg);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool TryGetStrArg(string key, out string value)
        {
            value = default;

            return _dic != null && _dic.TryGetValue(key, out value);
        }
    }
}