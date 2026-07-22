using System;
using SaintsField.SaintsSerialization;
using UnityEngine;

namespace SaintsField.Editor.Utils
{
    public static class SaintsSerializedEditorUtil
    {
        public static (string error, object value) GetValue(SaintsSerializedProperty ssp)
        {
            switch (ssp.propertyType)
            {
                case SaintsPropertyType.Undefined:
                    return ("Undefined SaintsSerializedProperty", null);
                case SaintsPropertyType.EnumLong:
                    return (string.Empty, ssp.longValue);
                case SaintsPropertyType.EnumULong:
                    return (string.Empty, ssp.uLongValue);
                case SaintsPropertyType.Interface:
                    return (string.Empty, ssp.IsVRef ? ssp.VRef : ssp.V);
                case SaintsPropertyType.DateTime:
                    return (string.Empty, new DateTime(ssp.longValue));
                case SaintsPropertyType.TimeSpan:
                    return (string.Empty, new TimeSpan(ssp.longValue));
                case SaintsPropertyType.Guid:
                {
                    string stringValue = ssp.stringValue;
                    if (string.IsNullOrEmpty(stringValue))
                    {
                        return (string.Empty, Guid.Empty);
                    }

                    // ReSharper disable once ConvertIfStatementToReturnStatement
                    if (Guid.TryParse(stringValue, out Guid guid))
                    {
                        return (string.Empty, guid);
                    }

                    return ($"Invalid guid value: {stringValue}", Guid.Empty);
                }
                case SaintsPropertyType.Decimal:
                {
                    int[] bites = ssp.intValues;
                    try
                    {
                        return (string.Empty, new decimal(bites));
                    }
#pragma warning disable CS0168 // Variable is declared but never used
                    catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
                    {
#if SAINTSFIELD_DEBUG
                        Debug.LogWarning(e);
#endif
                        return (e.Message, float.NaN);
                    }
                }
                default:
                    return ($"Unknown type {ssp.propertyType}", null);
            }
        }
    }
}
