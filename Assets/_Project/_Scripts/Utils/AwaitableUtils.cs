using System;
using System.Threading.Tasks;
using UnityEngine;
namespace Arena._Project._Scripts.Utils
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
