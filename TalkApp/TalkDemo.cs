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
        "Factory method: Order.Place",
        "Backing field and read-only Lines",
        "Private setters",
        "Domain methods control Status",
        "Total as a Money value object",
        "Private constructor and EF materialization",
        "Guard clauses",
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

            switch (choice)
            {
                case 1: await Problem1AnyoneCanBuildAnyOrderAsync(options); break;
                case 2: Problem2LinesChangedBehindTheOrdersBack(); break;
                case 3: Problem3ChangingAPlacedOrder(); break;
                case 4: Problem4ShipWithoutConfirming(); break;
                case 5: Problem5TotalIsJustADecimal(); break;
                case 6: Problem6EmptyOrder(); break;
                case 7: Problem7BadInputIsAccepted(); break;
            }

            //----------------------------------------------------------------//
            Console.WriteLine();
            InputHelpers.WaitForUserInput(ConsoleColor.DarkYellow);
            Console.WriteLine();
            //----------------------------------------------------------------//

            try
            {
                switch (choice)
                {
                    case 1: Solution1PlaceCreatesACompleteOrder(); break;
                    case 2: await Solution2AddLineKeepsTotalInStepAsync(options); break;
                    case 3: Solution3PrivateSettersLockTheOrder(); break;
                    case 4: Solution4DomainMethodsGuardTransitions(); break;
                    case 5: await Solution5TotalIsMoneyAsync(options); break;
                    case 6: await Solution6PrivateConstructorAndMaterializationAsync(options); break;
                    case 7: Solution7GuardClausesRejectBadInput(); break;
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

    // Step 1 (course Module 2 Clip 1): the factory method

    private static async Task Problem1AnyoneCanBuildAnyOrderAsync(DbContextOptions<TalkDbContext> options)
    {
        var order = new Anemic.Order { Status = "Banana", Total = -99.99m };

        await using (var context = new TalkDbContext(options))
        {
            context.AnemicOrders.Add(order);
            await context.SaveChangesAsync();
        }

        await using (var context = new TalkDbContext(options))
        {
            var reloaded = await context.AnemicOrders.Include(o => o.Lines).SingleAsync(o => o.Id == order.Id);

            Console.Write(OutputHelpers.BoxedArrayWithTitle(
                "Bad: Any Caller Can Build Any Order",
                new[]
                {
                    "new Order { Status = \"Banana\", Total = -99.99m }",
                    "",
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

    private static void Solution1PlaceCreatesACompleteOrder()
    {
        //TODO: Talk Step 1 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[]
        //{
        //    (2001, 2, Domain.Money.Create(19.99m, "USD")),
        //    (2002, 1, Domain.Money.Create(5.00m, "USD"))
        //});
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //    "Good: Order.Place Creates a Complete Order",
        //    new[]
        //    {
        //        "Order.Place(\"Contoso\", two lines)",
        //        "",
        //        $"Customer:    {order.CustomerName}",
        //        $"Status:      {order.Status}",
        //        $"Order date:  {order.OrderDate:d}",
        //        $"Lines:       {order.Lines.Count}",
        //        $"Total:       {order.Total}",
        //        "",
        //        "Order.Place sets every value an order needs, in one call."
        //    }
        //));
    }

    // Step 2 (course Module 2 Clip 2): the backing field and HasField

    private static void Problem2LinesChangedBehindTheOrdersBack()
    {
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed" };
        order.Lines.Add(new Anemic.OrderLine(2001, 1, Anemic.Money.Create(10.00m, "USD")));
        order.Total = 10.00m;

        order.Lines.Add(new Anemic.OrderLine(2002, 2, Anemic.Money.Create(5.00m, "USD")));
        var linesAfterAdd = order.Lines.Count;
        var sumAfterAdd = order.Lines.Sum(l => l.LineTotal.Amount);

        order.Lines.Clear();

        Console.Write(OutputHelpers.BoxedArrayWithTitle(
            "Bad: Lines Change, Total Doesn't",
            new[]
            {
                "order.Lines.Add(new OrderLine(2002, 2, 5.00 USD))",
                $"Lines: {linesAfterAdd}   Sum of lines: {sumAfterAdd}   Total: {order.Total}",
                "",
                "order.Lines.Clear()",
                $"Lines: {order.Lines.Count}   Total: {order.Total}"
            }
        ));
    }

    private static async Task Solution2AddLineKeepsTotalInStepAsync(DbContextOptions<TalkDbContext> options)
    {
        //TODO: Talk Step 2 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 1, Domain.Money.Create(10.00m, "USD")) });
        //var totalBefore = order.Total;
        //
        //order.AddLine(2002, 2, Domain.Money.Create(5.00m, "USD"));
        //
        //await using (var context = new TalkDbContext(options))
        //{
        //    context.DomainOrders.Add(order);
        //    await context.SaveChangesAsync();
        //}
        //
        //await using (var context = new TalkDbContext(options))
        //{
        //    var reloaded = await context.DomainOrders.Include(o => o.Lines).SingleAsync(o => o.Id == order.Id);
        //
        //    Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //        "Good: AddLine Keeps Total in Step",
        //        new[]
        //        {
        //            $"Before AddLine:      Lines: 1   Total: {totalBefore}",
        //            $"After AddLine:       Lines: {order.Lines.Count}   Total: {order.Total}",
        //            $"Saved and reloaded:  Lines: {reloaded.Lines.Count}   Total: {reloaded.Total}",
        //            "",
        //            "EF Core filled _lines, the field named by HasField(\"_lines\").",
        //            "order.Lines.Add(...) and order.Lines.Clear() no longer compile.",
        //            "new OrderLine(...) no longer compiles outside the model."
        //        }
        //    ));
        //}
    }

    // Step 3 (course Module 2 Clip 3): private setters

    private static void Problem3ChangingAPlacedOrder()
    {
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed", Total = 39.98m };
        var placedOn = order.OrderDate;

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

    private static void Solution3PrivateSettersLockTheOrder()
    {
        //TODO: Talk Step 3 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 2, Domain.Money.Create(19.99m, "USD")) });
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

    // Step 4 (course Module 2 Clip 3): domain methods for the status transitions

    private static void Problem4ShipWithoutConfirming()
    {
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed", Total = 39.98m };
        order.Status = "Shipped";

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

    private static void Solution4DomainMethodsGuardTransitions()
    {
        //TODO: Talk Step 4 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 2, Domain.Money.Create(19.99m, "USD")) });
        //var shipWhilePlaced = Attempt(() => order.Ship());
        //
        //order.Process();
        //order.Confirm();
        //order.Ship();
        //
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

    // Step 5 (course Module 2 Clip 4): Total as a Money value object with OwnsOne

    private static void Problem5TotalIsJustADecimal()
    {
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed" };
        order.Lines.Add(new Anemic.OrderLine(2001, 1, Anemic.Money.Create(19.99m, "USD")));
        order.Lines.Add(new Anemic.OrderLine(2002, 1, Anemic.Money.Create(25.00m, "EUR")));
        order.Total = order.Lines.Sum(l => l.LineTotal.Amount);

        Console.Write(OutputHelpers.BoxedArrayWithTitle(
            "Bad: Total Is Just a Decimal",
            new[]
            {
                "Line 1: 19.99 USD    Line 2: 25.00 EUR",
                $"Total:  {order.Total}   (dollars? euros?)",
                "",
                "Total is a decimal, so the currency is lost,",
                "and the Total column in SQL Server is a bare number."
            }
        ));
    }

    private static async Task Solution5TotalIsMoneyAsync(DbContextOptions<TalkDbContext> options)
    {
        //TODO: Talk Step 5 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 1, Domain.Money.Create(19.99m, "USD")), (2002, 2, Domain.Money.Create(19.99m, "USD")) });
        //
        //await using (var context = new TalkDbContext(options))
        //{
        //    context.DomainOrders.Add(order);
        //    await context.SaveChangesAsync();
        //}
        //
        //await using (var context = new TalkDbContext(options))
        //{
        //    var reloaded = await context.DomainOrders.SingleAsync(o => o.Id == order.Id);
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
        //            "OwnsOne maps Money to two columns on the DomainOrders table.",
        //            "order.Total = -42.50m no longer compiles."
        //        }
        //    ));
        //}
    }

    // Step 6 (course Module 2 Clip 7): the private constructor and EF Core materialization

    private static void Problem6EmptyOrder()
    {
        var order = new Anemic.Order();

        Console.Write(OutputHelpers.BoxedArrayWithTitle(
            "Bad: new Order() Creates an Empty Order",
            new[]
            {
                $"Customer:    '{order.CustomerName}'",
                $"Status:      '{order.Status}'",
                $"Order date:  {order.OrderDate:d}",
                $"Lines:       {order.Lines.Count}",
                $"Total:       {order.Total}",
                "",
                "A public constructor lets any caller skip Order.Place."
            }
        ));
    }

    private static async Task Solution6PrivateConstructorAndMaterializationAsync(DbContextOptions<TalkDbContext> options)
    {
        //TODO: Talk Step 6 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 2, Domain.Money.Create(19.99m, "USD")) });
        //var publicConstructor = typeof(Domain.Order).GetConstructor(Type.EmptyTypes);
        //
        //await using (var context = new TalkDbContext(options))
        //{
        //    context.DomainOrders.Add(order);
        //    await context.SaveChangesAsync();
        //}
        //
        //await using (var context = new TalkDbContext(options))
        //{
        //    var reloaded = await context.DomainOrders.Include(o => o.Lines).SingleAsync(o => o.Id == order.Id);
        //    var lineOrderId = context.Entry(reloaded.Lines.First()).Property<int?>("OrderId").CurrentValue;
        //
        //    Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //        "Good: Private Constructor, EF Core Still Rebuilds the Order",
        //        new[]
        //        {
        //            $"Public Order() constructor:  {(publicConstructor is null ? "none" : "public")}",
        //            "",
        //            $"Saved and reloaded:  Id {reloaded.Id}   {reloaded.CustomerName}   {reloaded.Status}",
        //            $"                     Lines {reloaded.Lines.Count}   Total {reloaded.Total}",
        //            "EF Core rebuilt the order through the private constructor.",
        //            $"OrderLine has no OrderId property: shadow foreign key {lineOrderId}",
        //            "",
        //            "new Order() no longer compiles. Order.Place is the only way in."
        //        }
        //    ));
        //}
    }

    // Step 7 (course Module 2 Clip 8): guard clauses

    private static void Problem7BadInputIsAccepted()
    {
        var order = new Anemic.Order { CustomerName = "", OrderDate = DateTime.Now, Status = "Placed" };
        order.Lines.Add(new Anemic.OrderLine(2001, -3, Anemic.Money.Create(19.99m, "USD")));
        order.Lines.Add(new Anemic.OrderLine(0, 1, Anemic.Money.Create(5.00m, "USD")));
        order.Lines.Add(new Anemic.OrderLine(2001, 2, Anemic.Money.Create(19.99m, "USD")));

        var negative = Anemic.Money.Create(-42.50m, "USD");
        var mixed = Anemic.Money.Create(19.99m, "USD").Add(Anemic.Money.Create(25.00m, "EUR"));

        var rows = order.Lines
            .Select(l => $"Product {l.ProductId,5}   Quantity {l.Quantity,3}   Line total {l.LineTotal.Amount,8}")
            .ToList();
        rows.Add($"Customer name: '{order.CustomerName}'");
        rows.Add("");
        rows.Add($"Money.Create(-42.50m):   {negative.Amount} {negative.Currency}");
        rows.Add($"19.99 USD + 25.00 EUR:   {mixed.Amount} {mixed.Currency}");
        rows.Add("");
        rows.Add("Nothing checks the input, so every value is accepted.");

        Console.Write(OutputHelpers.BoxedArrayWithTitle("Bad: Every Input Is Accepted", rows.ToArray()));
    }

    private static void Solution7GuardClausesRejectBadInput()
    {
        //TODO: Talk Step 7 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var price = Domain.Money.Create(19.99m, "USD");
        //var euros = Domain.Money.Create(25.00m, "EUR");
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //    "Good: Guard Clauses Reject Bad Input",
        //    new[]
        //    {
        //        $"Quantity -3:          {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, -3, price) }))}",
        //        $"Product 0:            {Attempt(() => Domain.Order.Place("Contoso", new[] { (0, 1, price) }))}",
        //        $"Product 2001 twice:   {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, 1, price), (2001, 2, price) }))}",
        //        $"Blank customer:       {Attempt(() => Domain.Order.Place("", new[] { (2001, 1, price) }))}",
        //        $"No lines:             {Attempt(() => Domain.Order.Place("Contoso", Array.Empty<(int, int, Domain.Money)>()))}",
        //        $"Money.Create(-42.50): {Attempt(() => Domain.Money.Create(-42.50m, "USD"))}",
        //        $"USD + EUR:            {Attempt(() => price.Add(euros))}",
        //        "",
        //        $"Valid order:          {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, 2, price) }))}"
        //    }
        //));
    }

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
