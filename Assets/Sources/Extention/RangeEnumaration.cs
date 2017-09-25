using System.Collections.Generic;
using Smooth.Algebraics;
using Smooth.Collections;
using UnityEngine;

namespace Tools
{
    public static class Utils
    {
        public static IEnumerable<int> Range(int duration)
        {
            return Range(0, duration);
        }
        
        public static IEnumerable<int> Range(int start, int end)
        {
           return new FuncEnumerable<int>(start, prev => ++prev < end ? prev.ToSome() : Option<int>.None);
        }
        
        public static T GetRandomItem<T>(this T[] array, int max = -1)
        {
            if (max < 0 || max >= array.Length)
            {
                max = array.Length;
            }
            return array[Random.Range(0, max)];
        }
    }
}