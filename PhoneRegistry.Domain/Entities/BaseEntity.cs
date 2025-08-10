namespace PhoneRegistry.Domain.Entities;

public abstract class BaseEntity
{
    // PK varsayılan olarak boş bırakılır; EF Core Add sırasında client-side Guid üretir.
    public Guid Id { get; set; } = Guid.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}
