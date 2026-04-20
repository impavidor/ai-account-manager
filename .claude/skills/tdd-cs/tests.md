# Good and Bad Tests

## Good Tests

**Integration-style**: Test through real interfaces, not mocks of internal parts.

```csharp
// GOOD: Tests observable behavior (xUnit)
[Fact]
public async Task UserCanCheckoutWithValidCart()
{
    var cart = new ShoppingCart();
    cart.Add(product);
    
    var result = await Checkout(cart, paymentMethod);
    
    Assert.Equal("confirmed", result.Status);
}
```

Characteristics:

- Tests behavior users/callers care about
- Uses public API only
- Survives internal refactors
- Describes WHAT, not HOW
- One logical assertion per test

## Bad Tests

**Implementation-detail tests**: Coupled to internal structure.

```csharp
// BAD: Tests implementation details (mocking internals)
[Fact]
public async Task CheckoutCallsPaymentService()
{
    var mockPayment = new Mock<IPaymentService>();
    mockPayment.Setup(p => p.Process(It.IsAny<decimal>()))
        .ReturnsAsync(true);
    
    await Checkout(cart, mockPayment.Object);
    
    mockPayment.Verify(p => p.Process(It.IsAny<decimal>()), Times.Once);
}
```

Red flags:

- Mocking internal collaborators
- Testing private methods
- Asserting on call counts/order
- Test breaks when refactoring without behavior change
- Test name describes HOW not WHAT
- Verifying through external means instead of interface

```csharp
// BAD: Bypasses interface to verify
[Fact]
public async Task CreateUserSavesToDatabase()
{
    await userService.CreateUser(new CreateUserRequest { Name = "Alice" });
    
    using var conn = new SqlConnection(connString);
    var result = conn.QuerySingleOrDefault("SELECT * FROM Users WHERE Name = @Name", 
        new { Name = "Alice" });
    
    Assert.NotNull(result);
}

// GOOD: Verifies through interface
[Fact]
public async Task CreateUserMakesUserRetrievable()
{
    var user = await userService.CreateUser(new CreateUserRequest { Name = "Alice" });
    var retrieved = await userService.GetUser(user.Id);
    
    Assert.Equal("Alice", retrieved.Name);
}
```
