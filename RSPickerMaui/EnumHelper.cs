namespace RSPickerMaui
{
    public static class EnumHelper
    {
        public static IList<string> GetEnumValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Provided type must be an enum", nameof(enumType));
            }

            return Enum.GetNames(enumType).ToList();
        }
    }
}
