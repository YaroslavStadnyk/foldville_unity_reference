using Core.Managers;
using UnityEngine;

namespace Core
{
    public static class DebugUtility
    {
        public static void LogCondition<T>(T target, bool result)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log($"{typeof(T).Name} was triggered ({result}). \nParams: {DataManager.Instance.ToJson(target)}.");
            }
        }

        public static void LogInvoke<T>(T target)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log($"{typeof(T).Name} was called. \nParams: {DataManager.Instance.ToJson(target)}.");
            }
        }

        public static void LogWarning<T>(T target, string warning)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogWarning($"{typeof(T).Name} wasn't called. \n{warning} \nParams: {DataManager.Instance.ToJson(target)}.");
            }
        }

        public static void LogError<T>(T target, string error)
        {
            if (Debug.isDebugBuild)
            {
                Debug.LogError($"{typeof(T).Name} wasn't called. \n{error} \nParams: {DataManager.Instance.ToJson(target)}.");
            }
        }
    }
}