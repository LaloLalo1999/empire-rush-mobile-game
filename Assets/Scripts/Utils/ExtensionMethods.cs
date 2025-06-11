using System.Collections.Generic;
using UnityEngine;
using System;

namespace EmpireRush.Utils
{
    public static class ExtensionMethods
    {
        #region Transform Extensions
        
        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        
        public static void SetChildrenActive(this Transform transform, bool active)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(active);
            }
        }
        
        #endregion
        
        #region Number Formatting
        
        public static string ToShortString(this long number)
        {
            if (number >= 1000000000000)
                return (number / 1000000000000f).ToString("F1") + "T";
            else if (number >= 1000000000)
                return (number / 1000000000f).ToString("F1") + "B";
            else if (number >= 1000000)
                return (number / 1000000f).ToString("F1") + "M";
            else if (number >= 1000)
                return (number / 1000f).ToString("F1") + "K";
            else
                return number.ToString();
        }
        
        public static string ToShortString(this int number)
        {
            return ((long)number).ToShortString();
        }
        
        public static string ToShortString(this float number)
        {
            return ((long)number).ToShortString();
        }
        
        #endregion
        
        #region Time Extensions
        
        public static string ToTimeString(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours:00}h {timeSpan.Minutes:00}m";
            else if (timeSpan.TotalHours >= 1)
                return $"{timeSpan.Hours:00}h {timeSpan.Minutes:00}m {timeSpan.Seconds:00}s";
            else
                return $"{timeSpan.Minutes:00}m {timeSpan.Seconds:00}s";
        }
        
        public static string ToShortTimeString(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours}h";
            else if (timeSpan.TotalHours >= 1)
                return $"{timeSpan.Hours}h {timeSpan.Minutes}m";
            else
                return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }
        
        #endregion
        
        #region Collection Extensions
        
        public static T GetRandomElement<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
                return default(T);
            
            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            return list[randomIndex];
        }
        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }
        
        #endregion
        
        #region Color Extensions
        
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
        
        public static Color Lighten(this Color color, float amount)
        {
            return Color.Lerp(color, Color.white, amount);
        }
        
        public static Color Darken(this Color color, float amount)
        {
            return Color.Lerp(color, Color.black, amount);
        }
        
        #endregion
        
        #region Vector Extensions
        
        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }
        
        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }
        
        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }
        
        public static Vector2 WithX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }
        
        public static Vector2 WithY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }
        
        #endregion
        
        #region Animation Extensions
        
        public static void SetTriggerSafe(this Animator animator, string triggerName)
        {
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.SetTrigger(triggerName);
            }
        }
        
        public static void SetBoolSafe(this Animator animator, string paramName, bool value)
        {
            if (animator != null && animator.isActiveAndEnabled)
            {
                animator.SetBool(paramName, value);
            }
        }
        
        #endregion
        
        #region Audio Extensions
        
        public static void PlayOneShotSafe(this AudioSource audioSource, AudioClip clip, float volumeScale = 1f)
        {
            if (audioSource != null && clip != null && audioSource.isActiveAndEnabled)
            {
                audioSource.PlayOneShot(clip, volumeScale);
            }
        }
        
        #endregion
        
        #region Math Extensions
        
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        
        public static bool IsApproximately(this float a, float b, float threshold = 0.01f)
        {
            return Mathf.Abs(a - b) < threshold;
        }
        
        public static int Wrap(this int value, int min, int max)
        {
            if (value < min)
                return max - (min - value) % (max - min + 1);
            else
                return min + (value - min) % (max - min + 1);
        }
        
        #endregion
    }
}