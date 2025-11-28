using System.Text;
using Core.Extensions;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public static class CoreUtility
    {
        public static Tweener Timer(TweenCallback action, float delay)
        {
            var timer = DOTween.To(_ => { }, 0.0f, 1.0f, delay);
            timer.OnComplete(action);
            timer.SetEase(Ease.Linear);
            return timer;
        }

        public static string SHA256(string value)
        {
            var sha256Managed = new System.Security.Cryptography.SHA256Managed();
            var hash = sha256Managed.ComputeHash(Encoding.UTF8.GetBytes(value));

            var hashString = new StringBuilder();
            foreach (var b in hash)
            {
                hashString.Append(b.ToString("x2"));
            }

            return hashString.ToString();
        }

        public static bool IsPrefab(this GameObject obj)
        {
            return obj.scene.name.IsNullOrEmpty() || obj.scene.name == obj.name;
        }
    }
}