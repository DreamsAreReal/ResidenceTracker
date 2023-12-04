using System.ComponentModel.DataAnnotations;
using ResidenceTracker.Domain.Attributes;

namespace ResidenceTracker.Domain.Abstractions;

public abstract class AbstractEntity
{
    [HumanizedName("Создано в (UTC)")]
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    [HumanizedName("Индетификатор")]
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    public bool IsReadonly { get; set; }

    [HumanizedName("Изменено в (UTC)")]
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    // ReSharper disable once EmptyConstructor
    public AbstractEntity()
    {
    }

    public void UpModifiedAt()
    {
        ModifiedAt = DateTime.UtcNow;
    }
}