# Mocking Guidelines

Mocking is a tool, not a requirement. Use it strategically.

## When to Mock

Mock **external dependencies** that are:

- Slow (databases, external APIs)
- Non-deterministic (time-based, random)
- Hard to set up (specific error states)
- Out of scope for this test

```csharp
// GOOD: Mock external service
[Fact]
public async Task UserCanCheckoutWithValidPayment()
{
    var mockGateway = new Mock<IPaymentGateway>();
    mockGateway.Setup(g => g.Charge(It.IsAny<decimal>()))
        .ReturnsAsync(new PaymentResult { Success = true });
    
    var processor = new OrderProcessor(mockGateway.Object);
    var result = await processor.Process(order, "valid-card");
    
    Assert.Equal("charged", result.Status);
}
```

The mock is legitimate here because:
- We're testing `OrderProcessor` behavior, not the payment gateway
- The gateway is external and slow
- We want to test payment success path without hitting real Stripe

## When NOT to Mock

**DO NOT mock** just to verify method calls were made:

```csharp
// BAD: Tests implementation detail (how OrderProcessor uses gateway)
[Fact]
public async Task ProcessorCallsGateway()
{
    var mockGateway = new Mock<IPaymentGateway>();
    mockGateway.Setup(g => g.Charge(It.IsAny<decimal>()))
        .ReturnsAsync(new PaymentResult { Success = true });
    
    var processor = new OrderProcessor(mockGateway.Object);
    await processor.Process(order);
    
    mockGateway.Verify(g => g.Charge(It.IsAny<decimal>()), Times.Once);  // ← Problem
}
```

This test breaks if you refactor the payment logic internally. You've coupled the test to implementation.

**GOOD equivalent**: Test the observable behavior

```csharp
[Fact]
public async Task UserCanCheckoutWithValidPayment()
{
    var mockGateway = new Mock<IPaymentGateway>();
    mockGateway.Setup(g => g.Charge(It.IsAny<decimal>()))
        .ReturnsAsync(new PaymentResult { Success = true });
    
    var processor = new OrderProcessor(mockGateway.Object);
    var result = await processor.Process(order);
    
    Assert.Equal("confirmed", result.Status);  // ← Behavior assertion
}
```

## Mocking Patterns

**Stub** (return a fixed value):

```csharp
var mockClock = new Mock<IClock>();
mockClock.Setup(c => c.UtcNow)
    .Returns(new DateTime(2024, 1, 1));
```

**Spy** (verify interaction, then check behavior):

```csharp
var mockLogger = new Mock<ILogger>();

await service.CreateUser(userData);

// Only verify if behavior depends on it being logged
mockLogger.Verify(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<string>()), 
    Times.Once);
```

**Exception handling**:

```csharp
var mockGateway = new Mock<IPaymentGateway>();
mockGateway.Setup(g => g.Charge(It.IsAny<decimal>()))
    .ThrowsAsync(new PaymentFailedException("Declined"));

var processor = new OrderProcessor(mockGateway.Object);
var result = await processor.Process(order);

Assert.Equal("declined", result.Status);
```

## Test Fixtures vs Mocking

Use test fixtures for complex setup, not mocks:

```csharp
// GOOD: Real database in test, fast enough
[Fact]
public async Task UserCanRetrieveCreatedUser()
{
    using var db = new TestDatabase();
    var user = await db.CreateUser(new User { Name = "Alice" });
    
    var retrieved = await db.GetUser(user.Id);
    
    Assert.Equal("Alice", retrieved.Name);
}

// NOT: Mock the database unless it's actually slow/complex
var mockDb = new Mock<IUserRepository>();
// ...
```
