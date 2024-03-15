using System;
using System.Diagnostics;
using SysConsole = System.Console;
using SysDebug = System.Diagnostics.Debug;
using UConsole = UnityEngine.Debug;
using UDebug = UnityEngine.Debug;

namespace EngineAdapter.Diagnostics
{
    [DebuggerStepThrough]
    public class Console
    {
        static bool isUnityEnv;
        static Console()
        {
            try
            {
                UConsole.Log("test UConsole validate");
                isUnityEnv = true;
            }
            catch (Exception)
            {
                isUnityEnv = false;
            }
        }

        public static void Log(object message)
        {
            if (isUnityEnv)
            {
                UConsole.Log(message);
            }
            else
            {
                SysConsole.WriteLine(message);
            }
        }

        public static void LogWarning(object message)
        {
            if (isUnityEnv)
            {
                UConsole.LogWarning(message);
            }
            else
            {
                SysConsole.WriteLine(message);
            }
        }

        public static void LogError(object message)
        {
            if (isUnityEnv)
            {
                UConsole.LogError(message);
            }
            else
            {
                SysConsole.WriteLine(message);
            }
        }

        public static void LogException(Exception exception)
        {
            if (isUnityEnv)
            {
                UConsole.LogException(exception);
            }
            else
            {
                SysConsole.WriteLine(exception);
            }
        }

        public static void Assert(bool condition)
        {
            if (isUnityEnv)
            {
                UDebug.Assert(condition);
            }
            else
            {
                SysDebug.Assert(condition);
            }
        }
        public static void Assert(bool condition, string message)
        {
            if (isUnityEnv)
            {
                UDebug.Assert(condition, message);
            }
            else
            {
                SysDebug.Assert(condition, message);
            }
        }
        
        public static void Error(params object[] ps)
        {
            var ret = string.Join(string.Empty,ps);
            LogError(ret);
        }
        public static void Log(params object[] ps)
        {
            var ret = string.Join(string.Empty,ps);
            Log(ret);
        }
        public static void Warn(params object[] ps)
        {
            var ret = string.Join(string.Empty,ps);
            LogWarning(ret);
        }
        public static void Exception(Exception exception)
        {
            LogException(exception);
        }
    }

}
