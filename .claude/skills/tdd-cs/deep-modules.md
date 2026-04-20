# Deep Modules

**Goal**: Small interface, complex implementation. Users of the module get a lot of power from a simple contract.

## Anti-Pattern: Shallow Modules

```csharp
// SHALLOW: Interface complexity mirrors implementation
public interface IUserRepository
{
    Task<User> GetById(int id);
    Task<User> GetByEmail(string email);
    Task<IEnumerable<User>> GetActive();
    Task<IEnumerable<User>> GetInactive();
    Task<IEnumerable<User>> GetByRole(string role);
    Task<IEnumerable<User>> GetByDepartment(string dept);
    Task<int> CountActive();
    Task<int> CountInactive();
    Task Insert(User user);
    Task Update(User user);
    Task Delete(int id);
    Task<bool> Exists(int id);
    // ... 20 more methods
}
```

Problems:
- Caller must know which method to use
- Many methods = many tests needed
- Small behavior changes require interface changes
- Hard to mock (too many methods)

## Pattern: Deep Module

```csharp
// DEEP: Simple interface, complex implementation
public interface IUserRepository
{
    Task<User?> Get(UserQuery query);
    Task<IEnumerable<User>> List(UserQuery query);
    Task Upsert(User user);
    Task Remove(int userId);
}

public class UserQuery
{
    public int? Id { get; set; }
    public string? Email { get; set; }
    public UserStatus? Status { get; set; }
    public string? Role { get; set; }
    // Extensible for future filters
}
```

Benefits:
- Simple interface hides complexity
- Fewer tests needed (fewer methods)
- Implementation can evolve internally
- Easy to mock (few methods)
- Client code is simpler

## Pattern: Use Queries for Flexibility

Instead of specific "GetBy*" methods, accept a query object:

```csharp
// SHALLOW
Task<User?> GetById(int id);
Task<User?> GetByEmail(string email);
Task<User?> GetByIdAndStatus(int id, UserStatus status);
Task<User?> GetByEmailAndRole(string email, string role);

// DEEP
Task<User?> Get(UserQuery query);

// Client code is always the same:
var user = await repo.Get(new UserQuery { Id = 123 });
var user = await repo.Get(new UserQuery { Email = "alice@example.com" });
var user = await repo.Get(new UserQuery { Email = "bob@example.com", Status = UserStatus.Active });
```

## When to Apply Deep Modules

When you notice:

- A class has many similar methods (GetBy*)
- Methods differ only in parameters
- Callers have to know which specific method to use
- New requirements add new methods instead of changing behavior

**Refactor**: Extract a query/filter object, collapse methods, hide complexity.

## Implementation Example

```csharp
public class UserRepository : IUserRepository
{
    private readonly SqlConnection _conn;
    
    public async Task<User?> Get(UserQuery query)
    {
        var sql = BuildQuery(query);
        return await _conn.QuerySingleOrDefaultAsync<User>(sql, query);
    }
    
    public async Task<IEnumerable<User>> List(UserQuery query)
    {
        var sql = BuildListQuery(query);
        return await _conn.QueryAsync<User>(sql, query);
    }
    
    // Complex query building hidden inside
    private string BuildQuery(UserQuery query)
    {
        var conditions = new List<string>();
        if (query.Id.HasValue) conditions.Add("Id = @Id");
        if (query.Email != null) conditions.Add("Email = @Email");
        if (query.Status.HasValue) conditions.Add("Status = @Status");
        // ... more complex logic
        
        return $"SELECT * FROM Users WHERE {string.Join(" AND ", conditions)}";
    }
    
    private string BuildListQuery(UserQuery query)
    {
        // Similar, but returns multiple
    }
}
```

The implementation is complex, but the interface is clean and the caller doesn't care.
