using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace SaintsField.Editor.Drawers.PrefixToggleDrawer
{
    public static class PrefixToggleUtils
    {
        // public class TargetResultCallback
        // {
        //     public object Target;
        //     public FieldInfo FieldInfo;
        //     public PropertyInfo PropertyInfo;
        // }
        //
        // public readonly struct TargetResult
        // {
        //     public readonly bool IsProperty;
        //     public readonly SerializedProperty Property;
        //     public readonly TargetResultCallback TargetResultCallback;
        //     public readonly string Error;
        //
        //     public TargetResult(bool isProperty, SerializedProperty property, TargetResultCallback targetResultCallback, string error)
        //     {
        //         IsProperty = isProperty;
        //         Property = property;
        //         TargetResultCallback = targetResultCallback;
        //         Error = error;
        //     }
        //
        //     public static TargetResult ErrorResult(string error) => new TargetResult(false, null, null, error);
        // }

        // public readonly struct TargetResult
        // {
        //     public readonly string Error;
        //     public readonly SerializedProperty Property;
        //
        // }

        public static (string error, SerializedProperty toggleProperty) GetToggleTarget(string targetName, SerializedProperty modProperty)
        {
            string modPropPath = modProperty.propertyPath;
            List<string> modPathSeg = modPropPath.Split('.').ToList();

            List<string> targetSepPath = targetName.Split('/').ToList();
            int targetLast = targetSepPath.Count - 1;
            string targetLastName = targetSepPath[targetLast];
            // targetSepPath.RemoveAt(targetLast);

            for (int index = 0; index < targetSepPath.Count; index++)
            {
                int lastIndex = modPathSeg.Count - 1;
                if (lastIndex < 0)
                {
                    return ($"Unable to up-walk {targetName} in {modPropPath}", null);
                }

                string lastName = modPathSeg[lastIndex];
                modPathSeg.RemoveAt(lastIndex);
                if (lastName.StartsWith("data["))  // Array.data[INDEX]
                {
                    modPathSeg.RemoveAt(lastIndex - 1);
                }
            }

            modPathSeg.Add(targetLastName);
            string searchPath = string.Join('.', modPathSeg);
            return (string.Empty, modProperty.serializedObject.FindProperty(searchPath));
        }
    }
}
