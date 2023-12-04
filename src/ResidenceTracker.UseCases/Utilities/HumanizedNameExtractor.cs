using System.Reflection;
using ResidenceTracker.Domain.Attributes;

namespace ResidenceTracker.UseCases.Utilities;

public static class HumanizedNameExtractor
{
    public static string GetLocalizedDisplayName(this PropertyInfo propertyInfo)
    {
        if (Attribute.GetCustomAttribute(propertyInfo, typeof(HumanizedNameAttribute)) is
            HumanizedNameAttribute localizedNameAttribute)
            return localizedNameAttribute.LocalizedName;

        return propertyInfo.Name;
    }
}