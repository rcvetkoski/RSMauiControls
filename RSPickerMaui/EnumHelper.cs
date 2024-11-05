using System.Collections;

namespace RSPickerMaui
{
    public static class EnumHelper
    {
        public static IList<T> GetEnumValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static IList<object> GetEnumValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Provided type must be an enum", nameof(enumType));
            }

            return Enum.GetValues(enumType).Cast<object>().ToList();
        }
    }
}
