namespace AccountManager.Common.Domain;

public abstract class Entity<T> : IEquatable<Entity<T>> where T : IEquatable<T>
{
    public T Id { get; }

    protected Entity(T id) => Id = id;

    public bool Equals(Entity<T>? other) => other is not null && Id.Equals(other.Id);

    public override bool Equals(object? obj) => obj is Entity<T> other && Equals(other);

    public override int GetHashCode() => Id.GetHashCode();
}
