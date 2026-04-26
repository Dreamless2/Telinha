using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Telinha.Utils
{
    public static class EnumExtensions
    {
        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                {
                    if (attr.Description == description)
                    {
                        if (field.GetValue(null) != null)
                            return (T)field.GetValue(null);
                    }
                }
                if (field.Name == description)
                {
                    if (field.GetValue(null) != null)
                        return (T)field.GetValue(null);
                }
            }
            return default;
        }