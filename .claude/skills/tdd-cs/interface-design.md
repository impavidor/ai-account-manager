# Interface Design for Testability

Good interfaces make testing natural:

1. **Accept dependencies, don't create them**

   ```csharp
   // Testable
   public class OrderProcessor
   {
       private readonly IPaymentGateway _gateway;
       
       public OrderProcessor(IPaymentGateway gateway)
       {
           _gateway = gateway;
       }
       
       public async Task ProcessOrder(Order order)
       {
           // uses injected gateway
       }
   }

   // Hard to test
   public class OrderProcessor
   {
       public async Task ProcessOrder(Order order)
       {
           var gateway = new StripeGateway();  // Hard to mock
           // uses gateway
       }
   }
   ```

2. **Return results, don't produce side effects**

   ```csharp
   // Testable
   public decimal CalculateDiscount(Cart cart)
   {
       return cart.Total * 0.1m;
   }

   // Hard to test
   public void ApplyDiscount(Cart cart)
   {
       cart.Total -= cart.Total * 0.1m;  // Modifies input
   }
   ```

3. **Small surface area**
   - Fewer methods = fewer tests needed
   - Fewer parameters = simpler test setup
   
   ```csharp
   // Good: Simple interface
   public interface ICheckoutService
   {
       Task<CheckoutResult> Process(Order order, IPaymentMethod payment);
   }

   // Bad: Large surface area
   public interface ICheckoutService
   {
       Task<CheckoutResult> Process(Order order, IPaymentMethod payment, bool sendEmail, 
           bool updateInventory, bool notifyWarehouse, string invoiceTemplate, ...);
   }
   ```

4. **Constructor-based dependency injection over property injection**

   ```csharp
   // GOOD: Dependencies clear from constructor
   public ShippingCalculator(IWarehouseApi warehouse, ITaxService tax)
   {
       _warehouse = warehouse;
       _tax = tax;
   }

   // BAD: Hidden dependencies
   public ShippingCalculator()
   {
   }
   
   public IWarehouseApi Warehouse { get; set; }  // May be null at runtime
   ```
