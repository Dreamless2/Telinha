using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telinha.Utils
{
    public static class EnumExtensions
    {
        public static T GetValueFromDescription<T>(string description) where T : struct, Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                {
                    if (attr.Description == description)
                        return (T)field.GetValue(null)!;
                }
                if (field.Name == description)
                    return (T)field.GetValue(null)!;
            }
            return default;
        }
    }
}