#if UNITY_EDITOR
using System;
using System.Linq;
using Arena.__Scripts;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace __Scripts.Assemblies.Utilities.Attributes
{
    [UsedImplicitly]
    public class DefaultValueDrawer : OdinAttributeDrawer<DefaultAttribute>
    {
        private bool _notCastable;
        
        protected override void Initialize()
        {
            if (Property.Attributes.Any(x => x is ReadOnlyAttribute))
                return;
            
            Type type = Property.ValueEntry.TypeOfValue;
            object defaultValue = DefaultValueOf(type);

            if (Property.ValueEntry.WeakSmartValue == null || Property.ValueEntry.WeakSmartValue.Equals(defaultValue))
            {
                try
                {
                    Property.ValueEntry.WeakSmartValue = Convert.ChangeType(Attribute.value, type);
                }
                catch (Exception)
                {
                    _notCastable = true;
                }
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (_notCastable)
                SirenixEditorGUI.ErrorMessageBox($"{Property.NiceName} is not castable to {Attribute.value.GetType().GetNiceName()}.");
            
            CallNextDrawer(label);
        }

        private static object DefaultValueOf(Type t) =>
            t != typeof(string) ? Activator.CreateInstance(t) : string.Empty;
    }
}
#endif