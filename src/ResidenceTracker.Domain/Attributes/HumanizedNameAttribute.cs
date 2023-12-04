namespace ResidenceTracker.Domain.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class HumanizedNameAttribute(string localizedName) : Attribute
{
    public string LocalizedName { get; } = localizedName;
}