using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Telinha.Enums;

namespace Telinha.Entity
{
    public class GenericHelpers

    {

        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = (DescriptionAttribute?)Attribute.GetCustomAttribute(field!, typeof(DescriptionAttribute));
            return attr?.Description ?? value.ToString();
        }
    }
}