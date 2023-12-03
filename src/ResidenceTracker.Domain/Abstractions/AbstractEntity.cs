namespace ResidenceTracker.Domain.Abstractions;

public abstract class AbstractEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    public void UpModifiedAt()
    {
        ModifiedAt = DateTime.UtcNow;
    }
}