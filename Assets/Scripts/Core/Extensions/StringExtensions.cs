using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Extensions
{
    /// <summary>
    /// Tips:
    /// <para>.ToString("+#;-#;0")</para>
    /// </summary>
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string ReplaceChar(this string str, string chr, int index)
        {
            str = str.Remove(index, chr.Length);
            str = str.Insert(index, chr);
            return str;
        }

        public static string ToShort(this float f)
        {
            return f switch
            {
                > 1000000000 => (f / 1000000000).ToString("0.0") + "B",
                > 1000000 => (f / 1000000).ToString("0.0") + "M",
                > 1000 => (f / 1000).ToString("0.0") + "K",
                _ => f.ToString("0.0")
            };
        }

        public static string ToShort(this int i)
        {
            return i switch
            {
                > 1000000000 => (i / 1000000000).ToString("0.0") + "B",
                > 1000000 => (i / 1000000).ToString("0.0") + "M",
                > 1000 => (i / 1000).ToString("0.0") + "K",
                _ => i.ToString()
            };
        }

        public static string ToRichColor(this string str, string color, float alpha)
        {
            return $"<color={color}><alpha=#{Mathf.RoundToInt(alpha * 100)}>{str}</color>";
        }

        public static string ToRichAlpha(this string str, float alpha)
        {
            return $"<alpha=#{Mathf.RoundToInt(alpha * 100)}>{str}</color>";
        }

        public static string ToStringValues<T>(this IReadOnlyList<T> values, string separator = ",", string subseparator = " or", string spacing = " ")
        {
            var valuesCount = values.Count;
            switch (valuesCount)
            {
                case < 1:
                    return "";
                case 1:
                    return $"{values[0]}";
                case 2:
                    return $"{values[0]}{subseparator}{spacing}{values[^1]}";
                case > 2:
                    var str = $"{values[0]}";
                    for (var i = 1; i < valuesCount - 1; i++)
                    {
                        str += $"{separator}{spacing}{values[i]}";
                    }
                    str += $"{separator}{subseparator}{spacing}{values[^1]}";
                    return str;
            }
        }

        public static string ToStringValues<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> values, Func<int, TKey, TValue, string> selector, string separator = ",", string subseparator = " or", string spacing = " ")
        {
            var valuesArray = values as KeyValuePair<TKey, TValue>[] ?? values.ToArray();
            var valuesCount = valuesArray.Length;
            switch (valuesCount)
            {
                case < 1:
                    return "";
                case 1:
                    return $"{selector?.Invoke(0, valuesArray[0].Key, valuesArray[0].Value)}";
                case 2:
                    return $"{selector?.Invoke(0, valuesArray[0].Key, valuesArray[0].Value)}{subseparator}{spacing}{selector?.Invoke(valuesCount - 1, valuesArray[^1].Key, valuesArray[^1].Value)}";
                case > 2:
                    var str = $"{selector?.Invoke(0, valuesArray[0].Key, valuesArray[0].Value)}";
                    for (var i = 1; i < valuesCount - 1; i++)
                    {
                        str += $"{separator}{spacing}{selector?.Invoke(i, valuesArray[i].Key, valuesArray[i].Value)}";
                    }
                    str += $"{separator}{subseparator}{spacing}{selector?.Invoke(valuesCount - 1, valuesArray[^1].Key, valuesArray[^1].Value)}";
                    return str;
            }
        }
    }
}