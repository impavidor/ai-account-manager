# Refactoring

**Rule: Never refactor while RED.** Get all tests to GREEN first, then refactor with safety.

## Safe Refactoring Pattern

After you complete a RED→GREEN cycle:

1. **All tests passing?** → Yes, proceed
2. **Refactor one small thing**
3. **Run tests** → Do they still pass?
4. **Yes?** → Commit this step
5. **No?** → Revert and try differently
6. **Repeat** for next refactoring opportunity

Each refactoring step should be small enough that you can revert and try again in 30 seconds.

## Refactoring Opportunities

### 1. Extract Duplication

```csharp
// Before (duplication)
[Fact]
public async Task UserCanCheckoutWithValidCart()
{
    var cart = new ShoppingCart();
    cart.Add(product);
    var order = await Checkout(cart, payment);
    Assert.Equal("confirmed", order.Status);
}

[Fact]
public async Task UserCanCheckoutWithMultipleItems()
{
    var cart = new ShoppingCart();
    cart.Add(product1);
    cart.Add(product2);
    var order = await Checkout(cart, payment);
    Assert.Equal("confirmed", order.Status);
}

// After (extract helper)
private async Task<Order> Checkout(params Product[] products)
{
    var cart = new ShoppingCart();
    foreach (var product in products)
        cart.Add(product);
    return await Checkout(cart, defaultPayment);
}

[Fact]
public async Task UserCanCheckoutWithValidCart()
{
    var order = await Checkout(product);
    Assert.Equal("confirmed", order.Status);
}

[Fact]
public async Task UserCanCheckoutWithMultipleItems()
{
    var order = await Checkout(product1, product2);
    Assert.Equal("confirmed", order.Status);
}
```

### 2. Simplify Mocks

If tests are full of mock setup, the interface is too complex. Simplify it.

```csharp
// Before: Too much mock setup
[Fact]
public async Task ProcessesOrder()
{
    var mockGateway = new Mock<IPaymentGateway>();
    var mockLogger = new Mock<ILogger>();
    var mockWarehouse = new Mock<IWarehouse>();
    var mockNotifier = new Mock<INotificationService>();
    
    // 20 lines of setup...
    
    var processor = new OrderProcessor(mockGateway.Object, mockLogger.Object, 
        mockWarehouse.Object, mockNotifier.Object);
}

// After: Simpler interface
public interface IOrderContext
{
    IPaymentGateway Gateway { get; }
    ILogger Logger { get; }
    IWarehouse Warehouse { get; }
    INotificationService Notifier { get; }
}

[Fact]
public async Task ProcessesOrder()
{
    var mockContext = new Mock<IOrderContext>();
    // Configure as needed
    var processor = new OrderProcessor(mockContext.Object);
}
```

### 3. Extract Constants

```csharp
// Before
[Fact]
public async Task UserIsChargedCorrectly()
{
    var total = 100m;
    var tax = total * 0.08m;  // Magic number
    var finalAmount = total + tax;
}

// After
private const decimal TaxRate = 0.08m;

[Fact]
public async Task UserIsChargedCorrectly()
{
    var total = 100m;
    var tax = total * TaxRate;
    var finalAmount = total + tax;
}
```

### 4. Deepen Modules (see [deep-modules.md](deep-modules.md))

Replace many similar methods with a single flexible interface.

### 5. Extract Methods

```csharp
// Before: Complex test logic
[Fact]
public async Task OrderWithInvalidPaymentFails()
{
    var cart = new ShoppingCart();
    foreach (var product in testProducts)
        cart.Add(product);
    
    var invalidPayment = new PaymentMethod 
    { 
        CardNumber = "0000000000000000",
        Expiry = DateTime.Now.AddYears(-1)
    };
    
    var result = await Checkout(cart, invalidPayment);
    Assert.Equal("declined", result.Status);
}

// After: Extract setup methods
private ShoppingCart CreateCartWithTestProducts()
{
    var cart = new ShoppingCart();
    foreach (var product in testProducts)
        cart.Add(product);
    return cart;
}

private PaymentMethod CreateInvalidPayment()
{
    return new PaymentMethod 
    { 
        CardNumber = "0000000000000000",
        Expiry = DateTime.Now.AddYears(-1)
    };
}

[Fact]
public async Task OrderWithInvalidPaymentFails()
{
    var cart = CreateCartWithTestProducts();
    var payment = CreateInvalidPayment();
    
    var result = await Checkout(cart, payment);
    
    Assert.Equal("declined", result.Status);
}
```

## Red Flags

If you're refactoring and tests keep failing:

- **Refactor is too big** - split it into smaller steps
- **Interface design is wrong** - tests shouldn't break on internal changes
- **Tests are too specific** - they're testing implementation, not behavior

Stop, revert, and reconsider the design.
