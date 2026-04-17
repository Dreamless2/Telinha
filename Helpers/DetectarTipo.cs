using System.ComponentModel;

namespace Telinha.Helpers
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