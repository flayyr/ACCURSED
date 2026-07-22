using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SaintsField.Editor.Utils;
using UnityEditor;

namespace SaintsField.Editor.Drawers.FieldReadOnlyAttributeDrawer
{
    public static class FieldReadOnlyUtils
    {
        public static (string error, bool disabled) IsDisabled(
            IReadOnlyList<FieldReadOnlyAttribute> fieldReadOnlyAttributes,
            SerializedProperty property, FieldInfo info, object target)
        {
            List<bool> allResults = new List<bool>();

            foreach (FieldReadOnlyAttribute targetAttribute in fieldReadOnlyAttributes)
            {
                (IReadOnlyList<string> errors, IReadOnlyList<bool> boolResults) =
                    Util.ConditionChecker(targetAttribute.ConditionInfos, property, info, target);

                if (errors.Count > 0)
                {
                    return (string.Join("\n\n", errors), false);
                }

                // And Mode
                bool boolResultsOk = boolResults.All(each => each);

                bool reverse = targetAttribute is FieldEnableIfAttribute;
                if (reverse)
                {
                    boolResultsOk = !boolResultsOk;
                }

                allResults.Add(boolResultsOk);
            }

            // Or Mode
            bool truly = allResults.Any(each => each);

#if SAINTSFIELD_DEBUG && SAINTSFIELD_DEBUG_READ_ONLY
            Debug.Log($"{property.name} final={truly}/ars={string.Join(",", allResults)}");
#endif
            return ("", truly);
        }
    }
}
