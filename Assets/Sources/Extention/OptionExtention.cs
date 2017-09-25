using UnityEngine;

namespace Smooth.Algebraics
{
    public static class OptionExtention
    {
        public static Option<T> TryGetComponent<T>(this GameObject go) where T : Component
        {
            var value = go.GetComponent<T>();
            return value == null ? Option<T>.None : new Option<T>(value);
        }
    }
}