using System;
using System.Linq;
using System.Reflection;
using Assemblies.Utilities.Attributes;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assemblies.Utilities.Extensions
{
    public static class ComponentExtensions
    {
        public static bool HasComponent<T>(this Component obj) =>
            obj.GetComponent<T>() != null;

        public static bool TryGetComponents<T, T1>(this Component obj, out T comp, out T1 comp1)
        {
            if (obj.TryGetComponent(out comp) && obj.TryGetComponent(out comp1))
                return true;
            
            comp = default(T);
            comp1 = default(T1);
            return false;
        }

        public static bool TryGetComponents<T, T1, T2>(this Component obj, out T comp, out T1 comp1, out T2 comp2)
        {
            if (obj.TryGetComponent(out comp) && obj.TryGetComponent(out comp1) && obj.TryGetComponent(out comp2))
                return true;
            
            comp = default(T);
            comp1 = default(T1);
            comp2 = default(T2);
            return false;
        }

        public static void ResetStructs(this Object obj)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            TypedReference rootRef = __makeref(obj);
            
            foreach (FieldInfo fieldInfo in obj.GetType()
                .GetFields(flags)
                .Where(f => f.FieldType is { IsValueType: true, IsPrimitive: false, IsEnum: false }))
            {
                Type fieldType = fieldInfo.FieldType;
                
                object newSt = Activator.CreateInstance(fieldType);
                
                foreach (FieldInfo newStField in fieldType
                    .GetFields(flags)
                    .Where(f => f.IsDefined(typeof(DefaultValueAttribute))))
                {
                    DefaultValueAttribute defaultValueAttribute = newStField.GetCustomAttribute<DefaultValueAttribute>();
                    try
                    {
                        newStField.SetValue(newSt, defaultValueAttribute.value);
                    }
                    catch (Exception)
                    {
                        Debug.LogWarning($"The default value for {newStField.Name} is not castable to the type of the member.");
                    }
                }
                fieldInfo.SetValueDirect(rootRef, newSt);
            }
        }
    }
}
