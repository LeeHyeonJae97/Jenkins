using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Args
{
    // for build player
    public const string ScriptingDefineSymbol = "scriptingDefineSymbol";
    public const string BundleVersion = "bundleVersion";
    public const string BundleVersionCode = "bundleVersionCode";
    public const string Dev = "dev";
    public const string BuildPath = "buildPath";

    // for test
    public const string TestResultPath = "testResultPath";

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

    public bool GetBoolArg(string key)
    {
        return bool.Parse(_dic[key]);
    }

    public int GetIntArg(string key)
    {
        return int.Parse(_dic[key]);
    }

    public float GetFloatArg(string key)
    {
        return float.Parse(_dic[key]);
    }

    public string GetStrArg(string key)
    {
        return _dic[key];
    }

    public T GetEnumArg<T>(string key) where T : Enum
    {
        return Enum.Parse<T>(key);
    }
}
