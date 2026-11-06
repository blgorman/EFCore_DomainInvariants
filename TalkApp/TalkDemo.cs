using ConsoleHelpers;
using Microsoft.EntityFrameworkCore;
using TalkApp.Data;
using Anemic = OrderModels.Anemic;
using Domain = OrderModels.Domain;

namespace TalkApp;

public static class TalkDemo
{
    private static readonly string[] MenuOptions =
    {
        "Private constructor and Order.Place",
        "Private setters",
        "Domain methods control Status",
        "Total as a Money value object",
        "Backing field and read-only Lines",
        "Exit"
    };

    public static async Task RunAsync(DbContextOptions<TalkDbContext> options)
    {
        while (true)
        {
            if (!Console.IsOutputRedirected) Console.Clear();
            OutputHelpers.WriteColored(OutputHelpers.SectionBanner("Creating Domain Invariants"), ConsoleColor.DarkBlue);
            for (int i = 0; i < MenuOptions.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {MenuOptions[i]}");
            }
            Console.WriteLine();

            var choice = InputHelpers.GetInputAsInt("Choose an option:", 1, MenuOptions.Length);
            if (choice == MenuOptions.Length)
            {
                return;
            }

            if (!Console.IsOutputRedirected) Console.Clear();
            OutputHelpers.WriteColored(OutputHelpers.SectionBanner($"{choice}. {MenuOptions[choice - 1]}"), ConsoleColor.DarkBlue);

            // Bad: the problem, shown on the anemic model.
            switch (choice)
            {
                case 1: await Problem1AnyoneCanBuildAnyOrderAsync(options); break;
                case 2: Problem2ChangingAPlacedOrder(); break;
                case 3: Problem3ShipWithoutConfirming(); break;
                case 4: Problem4TotalIsJustADecimal(); break;
                case 5: Problem5LinesChangedBehindTheOrdersBack(); break;
            }

            //----------------------------------------------------------------//
            Console.WriteLine();
            InputHelpers.WaitForUserInput(ConsoleColor.DarkYellow);
            Console.WriteLine();
            //----------------------------------------------------------------//

            // Good: the solution, shown on the domain model.
            // Each solution throws NotImplementedException until its step is applied, which shows the box below.
            try
            {
                switch (choice)
                {
                    case 1: await Solution1PrivateConstructorAndPlaceAsync(options); break;
                    case 2: Solution2PrivateSettersLockTheOrder(); break;
                    case 3: Solution3DomainMethodsGuardTransitions(); break;
                    case 4: await Solution4TotalIsMoneyAsync(options); break;
                    case 5: await Solution5AddLineKeepsTotalInStepAsync(options); break;
                }
            }
            catch (NotImplementedException)
            {
                Console.Write(OutputHelpers.BoxedArrayWithTitle(
                    "Not Yet Implemented",
                    new[]
                    {
                        $"This solution works after Talk Step {choice}.",
                        $"Apply every //TODO: Talk Step {choice} in the solution.",
                        "The last one is in TalkDemo.cs: delete the throw line under",
                        $"//TODO: Talk Step {choice} and uncomment the code below it."
                    }
                ));
            }

            //----------------------------------------------------------------//
            Console.WriteLine();
            InputHelpers.WaitForUserInput(ConsoleColor.DarkYellow);
            Console.WriteLine();
            //----------------------------------------------------------------//
        }
    }

    // Step 1: the private constructor, Order.Place, and the Order.Place guard clauses

    // Problem: the anemic Order has a public constructor and public setters, so any caller can build any order.
    private static async Task Problem1AnyoneCanBuildAnyOrderAsync(DbContextOptions<TalkDbContext> options)
    {
        // The public constructor returns an order with no customer, no status, and no lines.
        var empty = new Anemic.Order();
        // The object initializer sets values no real order could have. Nothing in the anemic Order objects.
        var order = new Anemic.Order { Status = "Banana", Total = -99.99m };

        // EF Core saves the invalid order without complaint.
        await using (var context = new TalkDbContext(options))
        {
            context.AnemicOrders.Add(order);
            await context.SaveChangesAsync();
        }

        // A new DbContext reads the row back from SQL Server, which proves the bad values were stored.
        await using (var context = new TalkDbContext(options))
        {
            var reloaded = await context.AnemicOrders.Include(o => o.Lines).SingleAsync(o => o.Id == order.Id);

            Console.Write(OutputHelpers.BoxedArrayWithTitle(
                "Bad: Any Caller Can Build Any Order",
                new[]
                {
                    "new Order()",
                    $"Customer: '{empty.CustomerName}'   Status: '{empty.Status}'   Lines: {empty.Lines.Count}   Total: {empty.Total}",
                    "",
                    "new Order { Status = \"Banana\", Total = -99.99m }",
                    $"Customer:    '{reloaded.CustomerName}'",
                    $"Status:      '{reloaded.Status}'",
                    $"Order date:  {reloaded.OrderDate:d}",
                    $"Lines:       {reloaded.Lines.Count}",
                    $"Total:       {reloaded.Total}",
                    "",
                    "EF Core saved and reloaded every one of those values."
                }
            ));
        }
    }

    // Solution: Order.Place is the only way to create a domain Order, and its guards reject a bad order.
    private static async Task Solution1PrivateConstructorAndPlaceAsync(DbContextOptions<TalkDbContext> options)
    {
        //TODO: Talk Step 1 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //// Order.Place sets the customer, date, status, lines, and total, so the order starts valid.
        //var order = Domain.Order.Place("Contoso", new[]
        //{
        //    (2001, 2, Domain.Money.Create(19.99m, "USD")),
        //    (2002, 1, Domain.Money.Create(5.00m, "USD"))
        //});
        //// Reflection looks for a public parameterless constructor. After step 1 there is none.
        //var publicConstructor = typeof(Domain.Order).GetConstructor(Type.EmptyTypes);
        //
        //// Save the order through EF Core.
        //await using (var context = new TalkDbContext(options))
        //{
        //    context.DomainOrders.Add(order);
        //    await context.SaveChangesAsync();
        //}
        //
        //// A new DbContext reloads the order. EF Core rebuilds the order through the private constructor.
        //await using (var context = new TalkDbContext(options))
        //{
        //    var reloaded = await context.DomainOrders.Include(o => o.Lines).SingleAsync(o => o.Id == order.Id);
        //    // OrderLine has no OrderId property, so read the shadow foreign key from the change tracker.
        //    var lineOrderId = context.Entry(reloaded.Lines.First()).Property<int?>("OrderId").CurrentValue;
        //
        //    Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //        "Good: Order.Place Is the Only Way In",
        //        new[]
        //        {
        //            $"Public Order() constructor:  {(publicConstructor is null ? "none" : "public")}",
        //            "",
        //            "Order.Place(\"Contoso\", two lines), saved and reloaded",
        //            $"Customer:    {reloaded.CustomerName}",
        //            $"Status:      {reloaded.Status}",
        //            $"Order date:  {reloaded.OrderDate:d}",
        //            $"Lines:       {reloaded.Lines.Count}",
        //            $"Total:       {reloaded.Total}",
        //            "",
        //            // Attempt calls Order.Place with bad input and prints the message from the guard.
        //            $"Blank customer:  {Attempt(() => Domain.Order.Place("", new[] { (2001, 1, Domain.Money.Create(19.99m, "USD")) }))}",
        //            $"No lines:        {Attempt(() => Domain.Order.Place("Contoso", Array.Empty<(int, int, Domain.Money)>()))}",
        //            "",
        //            "EF Core rebuilt the order through the private constructor.",
        //            $"OrderLine has no OrderId property: shadow foreign key {lineOrderId}"
        //        }
        //    ));
        //}
    }

    // Step 2: private setters

    // Problem: every setter on the anemic Order is public, so any caller can rewrite an order after it is placed.
    private static void Problem2ChangingAPlacedOrder()
    {
        // Place an order for Contoso and remember the original date.
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed", Total = 39.98m };
        var placedOn = order.OrderDate;

        // Rewrite the placed order from outside the class. The anemic Order accepts all three assignments.
        order.OrderDate = DateTime.Now.AddDays(-30);
        order.CustomerName = "Someone Else";
        order.Total = 0.01m;

        Console.Write(OutputHelpers.BoxedArrayWithTitle(
            "Bad: Any Caller Can Rewrite a Placed Order",
            new[]
            {
                $"Order date:  {placedOn:d}  ->  {order.OrderDate:d}",
                "Customer:    Contoso  ->  Someone Else",
                $"Total:       39.98  ->  {order.Total}",
                "",
                "Every property has a public setter, so nothing stops the caller."
            }
        ));
    }

    // Solution: private setters mean only code inside the domain Order can change the five properties.
    private static void Solution2PrivateSettersLockTheOrder()
    {
        //TODO: Talk Step 2 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //// Order.Place sets the properties from inside the Order class, which private setters allow.
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 2, Domain.Money.Create(19.99m, "USD")) });
        //// Assigning from here would not compile, so reflection reports the visibility of each setter instead.
        //var rows = new[] { "Id", "Status", "Total", "OrderDate", "CustomerName" }
        //    .Select(name => $"{name,-13} setter: {(typeof(Domain.Order).GetProperty(name)!.SetMethod!.IsPublic ? "public" : "private")}")
        //    .ToList();
        //rows.Add("");
        //rows.Add($"Order.Place still sets them inside Order: {order.CustomerName}, {order.Status}");
        //rows.Add("");
        //rows.Add("order.OrderDate = ..., order.CustomerName = ..., and order.Total = ...");
        //rows.Add("no longer compile outside the Order class.");
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle("Good: Private Setters Lock the Order", rows.ToArray()));
    }

    // Step 3: domain methods for the status transitions

    // Problem: Status on the anemic Order is a public string, so any status can follow any other status.
    private static void Problem3ShipWithoutConfirming()
    {
        // A Placed order jumps to Shipped without being processed or confirmed.
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed", Total = 39.98m };
        order.Status = "Shipped";

        // A Cancelled order ships too.
        var cancelled = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Cancelled", Total = 39.98m };
        cancelled.Status = "Shipped";

        Console.Write(OutputHelpers.BoxedArrayWithTitle(
            "Bad: Status Jumps Straight to Shipped",
            new[]
            {
                "order.Status = 'Shipped' on a Placed order",
                $"Status:  {order.Status}   (never Processing, never Confirmed)",
                "",
                "order.Status = 'Shipped' on a Cancelled order",
                $"Status:  {cancelled.Status}"
            }
        ));
    }

    // Solution: Process, Confirm, Ship, and Cancel are the only way to change Status, and each one checks the current Status first.
    private static void Solution3DomainMethodsGuardTransitions()
    {
        //TODO: Talk Step 3 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //// Ship checks Status first, so a Placed order is rejected and stays Placed.
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 2, Domain.Money.Create(19.99m, "USD")) });
        //var shipWhilePlaced = Attempt(() => order.Ship());
        //
        //// The only route to Shipped is Process, then Confirm, then Ship.
        //order.Process();
        //order.Confirm();
        //order.Ship();
        //
        //// A Cancelled order is rejected by the same guard in Ship.
        //var cancelled = Domain.Order.Place("Contoso", new[] { (2001, 2, Domain.Money.Create(19.99m, "USD")) });
        //cancelled.Cancel();
        //var shipWhileCancelled = Attempt(() => cancelled.Ship());
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //    "Good: Domain Methods Guard Every Transition",
        //    new[]
        //    {
        //        $"Ship() while Placed:    {shipWhilePlaced}",
        //        $"Ship() while Cancelled: {shipWhileCancelled}",
        //        "",
        //        "Process() -> Confirm() -> Ship()",
        //        $"Status:  {order.Status}"
        //    }
        //));
    }

    // Step 4: Total as a Money value object with OwnsOne, and the Money guard clauses

    // Problem: the anemic Total is a bare decimal with no currency, and the anemic Money checks nothing.
    private static void Problem4TotalIsJustADecimal()
    {
        // One dollar line and one euro line on the same order.
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed" };
        order.Lines.Add(new Anemic.OrderLine(2001, 1, Anemic.Money.Create(19.99m, "USD")));
        order.Lines.Add(new Anemic.OrderLine(2002, 1, Anemic.Money.Create(25.00m, "EUR")));
        // Sum adds the two amounts, and the currencies are lost.
        order.Total = order.Lines.Sum(l => l.LineTotal.Amount);

        // The anemic Money accepts a negative amount, and adding EUR to USD returns USD.
        var negative = Anemic.Money.Create(-42.50m, "USD");
        var mixed = Anemic.Money.Create(19.99m, "USD").Add(Anemic.Money.Create(25.00m, "EUR"));

        Console.Write(OutputHelpers.BoxedArrayWithTitle(
            "Bad: Total Is Just a Decimal",
            new[]
            {
                "Line 1: 19.99 USD    Line 2: 25.00 EUR",
                $"Total:  {order.Total}   (dollars? euros?)",
                "",
                "Total is a decimal, so the currency is lost,",
                "and the Total column in SQL Server is a bare number.",
                "",
                $"Money.Create(-42.50m):   {negative.Amount} {negative.Currency}",
                $"19.99 USD + 25.00 EUR:   {mixed.Amount} {mixed.Currency}"
            }
        ));
    }

    // Solution: Total is a Money value object, so it carries a currency, and the Money guards reject bad values.
    private static async Task Solution4TotalIsMoneyAsync(DbContextOptions<TalkDbContext> options)
    {
        //TODO: Talk Step 4 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //// Two USD lines. RecalculateTotal adds the line totals with Money.Add, so Total is 59.97 USD.
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 1, Domain.Money.Create(19.99m, "USD")), (2002, 2, Domain.Money.Create(19.99m, "USD")) });
        //
        //// Save the order. OwnsOne stores Total in the Total_Amount and Total_Currency columns.
        //await using (var context = new TalkDbContext(options))
        //{
        //    context.DomainOrders.Add(order);
        //    await context.SaveChangesAsync();
        //}
        //
        //// A new DbContext reloads the order, and Total comes back as Money.
        //await using (var context = new TalkDbContext(options))
        //{
        //    var reloaded = await context.DomainOrders.SingleAsync(o => o.Id == order.Id);
        //    // Raw SQL reads the two columns directly, which proves SQL Server holds the currency.
        //    var columns = await context.Database
        //        .SqlQueryRaw<string>("SELECT CONCAT('Total_Amount ', Total_Amount, ', Total_Currency ', Total_Currency) AS Value FROM DomainOrders WHERE Id = {0}", order.Id)
        //        .SingleAsync();
        //
        //    Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //        "Good: Total Is Money",
        //        new[]
        //        {
        //            $"Total type:          {order.Total.GetType().Name}",
        //            $"Order total:         {order.Total}",
        //            $"Saved and reloaded:  {reloaded.Total}",
        //            $"SQL Server columns:  {columns}",
        //            "",
        //            // Each Attempt hits a Money guard: a negative amount, USD plus EUR, and an order with both currencies.
        //            $"Money.Create(-42.50):  {Attempt(() => Domain.Money.Create(-42.50m, "USD"))}",
        //            $"USD + EUR:             {Attempt(() => Domain.Money.Create(19.99m, "USD").Add(Domain.Money.Create(25.00m, "EUR")))}",
        //            $"USD and EUR lines:     {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, 1, Domain.Money.Create(19.99m, "USD")), (2002, 1, Domain.Money.Create(25.00m, "EUR")) }))}",
        //            "",
        //            "order.Total = -42.50m no longer compiles."
        //        }
        //    ));
        //}
    }

    // Step 5: the backing field, AddLine, HasField, and the line guard clauses

    // Problem: the anemic Lines is a public List, so any caller can change the lines, nothing recalculates Total, and nothing checks a line.
    private static void Problem5LinesChangedBehindTheOrdersBack()
    {
        // The order starts consistent: one 10.00 line and a Total of 10.00.
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed" };
        order.Lines.Add(new Anemic.OrderLine(2001, 1, Anemic.Money.Create(10.00m, "USD")));
        order.Total = 10.00m;

        // Add a second line directly. The lines now sum to 20.00, and Total still says 10.00.
        order.Lines.Add(new Anemic.OrderLine(2002, 2, Anemic.Money.Create(5.00m, "USD")));
        var linesAfterAdd = order.Lines.Count;
        var sumAfterAdd = order.Lines.Sum(l => l.LineTotal.Amount);

        // Clear the lines directly. The order has no lines, and Total still says 10.00.
        order.Lines.Clear();
        var linesAfterClear = order.Lines.Count;

        // Add three bad lines: quantity -3, product 0, and product 2001 a second time. All three are accepted.
        order.Lines.Add(new Anemic.OrderLine(2001, -3, Anemic.Money.Create(19.99m, "USD")));
        order.Lines.Add(new Anemic.OrderLine(0, 1, Anemic.Money.Create(5.00m, "USD")));
        order.Lines.Add(new Anemic.OrderLine(2001, 2, Anemic.Money.Create(19.99m, "USD")));

        var rows = new List<string>
        {
            "order.Lines.Add(new OrderLine(2002, 2, 5.00 USD))",
            $"Lines: {linesAfterAdd}   Sum of lines: {sumAfterAdd}   Total: {order.Total}",
            "",
            "order.Lines.Clear()",
            $"Lines: {linesAfterClear}   Total: {order.Total}",
            ""
        };
        rows.AddRange(order.Lines.Select(l => $"Product {l.ProductId,5}   Quantity {l.Quantity,3}   Line total {l.LineTotal.Amount,8}"));

        Console.Write(OutputHelpers.BoxedArrayWithTitle("Bad: Lines Change, Total Doesn't", rows.ToArray()));
    }

    // Solution: _lines is private, Lines is read-only, and AddLine is the one path that adds a line and recalculates Total.
    private static async Task Solution5AddLineKeepsTotalInStepAsync(DbContextOptions<TalkDbContext> options)
    {
        //TODO: Talk Step 5 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //// The order starts with one 10.00 USD line and a Total of 10.00 USD.
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 1, Domain.Money.Create(10.00m, "USD")) });
        //var totalBefore = order.Total;
        //
        //// AddLine adds the line and recalculates, so Total moves to 20.00 USD.
        //order.AddLine(2002, 2, Domain.Money.Create(5.00m, "USD"));
        //
        //// Save the order through EF Core.
        //await using (var context = new TalkDbContext(options))
        //{
        //    context.DomainOrders.Add(order);
        //    await context.SaveChangesAsync();
        //}
        //
        //// A new DbContext reloads the order. EF Core fills the _lines field named by HasField.
        //await using (var context = new TalkDbContext(options))
        //{
        //    var reloaded = await context.DomainOrders.Include(o => o.Lines).SingleAsync(o => o.Id == order.Id);
        //    var price = Domain.Money.Create(19.99m, "USD");
        //
        //    Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //        "Good: AddLine Keeps Total in Step",
        //        new[]
        //        {
        //            $"Before AddLine:      Lines: 1   Total: {totalBefore}",
        //            $"After AddLine:       Lines: {order.Lines.Count}   Total: {order.Total}",
        //            $"Saved and reloaded:  Lines: {reloaded.Lines.Count}   Total: {reloaded.Total}",
        //            "",
        //            // Each Attempt hits a line guard: quantity -3, product 0, and product 2001 twice.
        //            $"Quantity -3:         {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, -3, price) }))}",
        //            $"Product 0:           {Attempt(() => Domain.Order.Place("Contoso", new[] { (0, 1, price) }))}",
        //            $"Product 2001 twice:  {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, 1, price), (2001, 2, price) }))}",
        //            "",
        //            "EF Core filled _lines, the field named by HasField(\"_lines\").",
        //            "order.Lines.Add(...) and order.Lines.Clear() no longer compile.",
        //            "new OrderLine(...) no longer compiles outside the model."
        //        }
        //    ));
        //}
    }

    // Runs a call that should be rejected and returns the guard message, so the demo prints the rejection instead of crashing.
    private static string Attempt(Action action)
    {
        try
        {
            action();
            return "succeeded";
        }
        catch (ArgumentException ex)
        {
            return $"rejected: {ex.Message.Replace($" (Parameter '{ex.ParamName}')", string.Empty)}";
        }
        catch (InvalidOperationException ex)
        {
            return $"rejected: {ex.Message}";
        }
    }
}
