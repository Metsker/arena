using System;
using UnityEngine;
namespace __Scripts.Generic.Utils
{
    public static class AwaitableUtils
    {
        public static async Awaitable WaitUntil(Func<bool> func, float step = 0.05f)
        {
            while (!func.Invoke())
                await Awaitable.WaitForSecondsAsync(step);
        }
        
        public static async Awaitable WaitWhile(Func<bool> func, float step = 0.05f)
        {
            while (func.Invoke())
                await Awaitable.WaitForSecondsAsync(step);
        }
    }
}
