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
        "Status is a free string",
        "Shipping without confirming",
        "Lines changed behind the order's back",
        "An empty order",
        "Bad quantities and duplicate products",
        "Negative and mixed-currency totals",
        "Saving and reloading through EF Core",
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
                case 1: Problem1StatusIsAFreeString(); break;
                case 2: Problem2ShipWithoutConfirming(); break;
                case 3: Problem3LinesChangedBehindTheOrdersBack(); break;
                case 4: Problem4EmptyOrder(); break;
                case 5: Problem5BadQuantitiesAndDuplicateProducts(); break;
                case 6: Problem6NegativeAndMixedCurrencyTotals(); break;
                case 7: await Problem7SaveAndReloadAsync(options); break;
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
                    case 1: Solution1StatusIsAnOrderStatus(); break;
                    case 2: Solution2DomainMethodsGuardTransitions(); break;
                    case 3: Solution3AddLineKeepsTotalInStep(); break;
                    case 4: Solution4PlaceIsTheOnlyWayIn(); break;
                    case 5: Solution5GuardClausesRejectBadInput(); break;
                    case 6: Solution6MoneyRejectsBadTotals(); break;
                    case 7: await Solution7SaveAndReloadAsync(options); break;
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

    private static void Problem1StatusIsAFreeString()
    {
        var typo = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Shiped" };
        var banana = new Anemic.Order { CustomerName = "Test Customer", OrderDate = DateTime.Now, Status = "Banana", Total = -99.99m };

        Console.Write(OutputHelpers.BoxedArrayWithTitle(
            "Bad: Status Is a Free String",
            new[]
            {
                $"order.Status = 'Shiped'  ->  Status: {typo.Status}",
                $"order.Status = 'Banana'  ->  Status: {banana.Status}",
                "",
                "The compiler accepts any string as a status."
            }
        ));
    }

    private static void Solution1StatusIsAnOrderStatus()
    {
        //TODO: Talk Step 1 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var statusType = typeof(Domain.Order).GetProperty(nameof(Domain.Order.Status))!.PropertyType;
        //var typoIsAStatus = Enum.TryParse<Domain.OrderStatus>("Shiped", out _);
        //var bananaIsAStatus = Enum.TryParse<Domain.OrderStatus>("Banana", out _);
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //    "Good: Status Is an OrderStatus",
        //    new[]
        //    {
        //        $"Status property type:      {statusType.Name}",
        //        $"Allowed values:            {string.Join(", ", Enum.GetNames<Domain.OrderStatus>())}",
        //        $"'Shiped' is an OrderStatus: {typoIsAStatus}",
        //        $"'Banana' is an OrderStatus: {bananaIsAStatus}",
        //        "",
        //        "order.Status = 'Banana' no longer compiles."
        //    }
        //));
    }

    private static void Problem2ShipWithoutConfirming()
    {
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed", Total = 39.98m };
        var placedOn = order.OrderDate;

        order.Status = "Shipped";
        order.OrderDate = DateTime.Now.AddDays(-30);
        order.CustomerName = "Someone Else";

        Console.Write(OutputHelpers.BoxedArrayWithTitle(
            "Bad: Status Jumps Straight to Shipped",
            new[]
            {
                "order.Status = 'Shipped' on a Placed order",
                $"Status:      {order.Status}   (never Processing, never Confirmed)",
                "",
                "After shipping, the caller rewrites history:",
                $"Order date:  {placedOn:d}  ->  {order.OrderDate:d}",
                $"Customer:    Contoso  ->  {order.CustomerName}"
            }
        ));
    }

    private static void Solution2DomainMethodsGuardTransitions()
    {
        //TODO: Talk Step 2 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 2, Domain.Money.Create(19.99m, "USD")) });
        //var shipWhilePlaced = Attempt(() => order.Ship());
        //
        //order.Process();
        //order.Confirm();
        //order.Ship();
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //    "Good: Domain Methods Guard Every Transition",
        //    new[]
        //    {
        //        $"Ship() on a Placed order:  {shipWhilePlaced}",
        //        "",
        //        "Process() -> Confirm() -> Ship()",
        //        $"Status:  {order.Status}",
        //        "",
        //        "order.Status = ..., order.OrderDate = ..., and order.CustomerName = ...",
        //        "no longer compile."
        //    }
        //));
    }

    private static void Problem3LinesChangedBehindTheOrdersBack()
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

    private static void Solution3AddLineKeepsTotalInStep()
    {
        //TODO: Talk Step 3 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 1, Domain.Money.Create(10.00m, "USD")) });
        //var totalBefore = order.Total;
        //
        //order.AddLine(2002, 2, Domain.Money.Create(5.00m, "USD"));
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //    "Good: AddLine Keeps Total in Step",
        //    new[]
        //    {
        //        $"Before AddLine:  Lines: 1   Total: {totalBefore}",
        //        $"After AddLine:   Lines: {order.Lines.Count}   Total: {order.Total}",
        //        "",
        //        "order.Lines.Add(...) and order.Lines.Clear() no longer compile.",
        //        "new OrderLine(...) no longer compiles outside the model."
        //    }
        //));
    }

    private static void Problem4EmptyOrder()
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
                $"Total:       {order.Total}"
            }
        ));
    }

    private static void Solution4PlaceIsTheOnlyWayIn()
    {
        //TODO: Talk Step 4 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 2, Domain.Money.Create(19.99m, "USD")) });
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //    "Good: Order.Place Is the Only Way to Create an Order",
        //    new[]
        //    {
        //        $"Customer:    {order.CustomerName}",
        //        $"Status:      {order.Status}",
        //        $"Order date:  {order.OrderDate:d}",
        //        $"Lines:       {order.Lines.Count}",
        //        $"Total:       {order.Total}",
        //        "",
        //        "new Order() no longer compiles."
        //    }
        //));
    }

    private static void Problem5BadQuantitiesAndDuplicateProducts()
    {
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed" };
        order.Lines.Add(new Anemic.OrderLine(2001, -3, Anemic.Money.Create(19.99m, "USD")));
        order.Lines.Add(new Anemic.OrderLine(0, 1, Anemic.Money.Create(5.00m, "USD")));
        order.Lines.Add(new Anemic.OrderLine(2001, 2, Anemic.Money.Create(19.99m, "USD")));

        var rows = order.Lines
            .Select(l => $"Product {l.ProductId,5}   Quantity {l.Quantity,3}   Line total {l.LineTotal.Amount,8}")
            .ToList();
        rows.Add("");
        rows.Add("Quantity -3, product 0, and product 2001 twice: all accepted.");

        Console.Write(OutputHelpers.BoxedArrayWithTitle("Bad: Every Line Is Accepted", rows.ToArray()));
    }

    private static void Solution5GuardClausesRejectBadInput()
    {
        //TODO: Talk Step 5 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var price = Domain.Money.Create(19.99m, "USD");
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //    "Good: Guard Clauses Reject Bad Input",
        //    new[]
        //    {
        //        $"Quantity -3:        {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, -3, price) }))}",
        //        $"Product 0:          {Attempt(() => Domain.Order.Place("Contoso", new[] { (0, 1, price) }))}",
        //        $"Product 2001 twice: {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, 1, price), (2001, 2, price) }))}",
        //        $"Blank customer:     {Attempt(() => Domain.Order.Place("", new[] { (2001, 1, price) }))}",
        //        $"No lines:           {Attempt(() => Domain.Order.Place("Contoso", Array.Empty<(int, int, Domain.Money)>()))}",
        //        $"Valid order:        {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, 2, price) }))}"
        //    }
        //));
    }

    private static void Problem6NegativeAndMixedCurrencyTotals()
    {
        var order = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Placed" };
        order.Lines.Add(new Anemic.OrderLine(2001, 1, Anemic.Money.Create(19.99m, "USD")));
        order.Lines.Add(new Anemic.OrderLine(2002, 1, Anemic.Money.Create(25.00m, "EUR")));
        var mixedSum = order.Lines.Sum(l => l.LineTotal.Amount);

        order.Total = -42.50m;

        Console.Write(OutputHelpers.BoxedArrayWithTitle(
            "Bad: Total Is Just a Decimal",
            new[]
            {
                "19.99 USD + 25.00 EUR summed into one decimal:",
                $"Sum: {mixedSum}   (dollars? euros?)",
                "",
                "order.Total = -42.50m",
                $"Total: {order.Total}"
            }
        ));
    }

    private static void Solution6MoneyRejectsBadTotals()
    {
        //TODO: Talk Step 6 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var dollars = Domain.Money.Create(19.99m, "USD");
        //var euros = Domain.Money.Create(25.00m, "EUR");
        //var valid = Domain.Order.Place("Contoso", new[] { (2001, 1, dollars), (2002, 2, dollars) });
        //
        //Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //    "Good: Total Is Money",
        //    new[]
        //    {
        //        $"Money.Create(-42.50m):  {Attempt(() => Domain.Money.Create(-42.50m, "USD"))}",
        //        $"USD line + EUR line:    {Attempt(() => Domain.Order.Place("Contoso", new[] { (2001, 1, dollars), (2002, 1, euros) }))}",
        //        "",
        //        $"Valid order total:  {valid.Total.Amount} {valid.Total.Currency}",
        //        "",
        //        "order.Total = -42.50m no longer compiles."
        //    }
        //));
    }

    private static async Task Problem7SaveAndReloadAsync(DbContextOptions<TalkDbContext> options)
    {
        var banana = new Anemic.Order { CustomerName = "Test Customer", OrderDate = DateTime.Now, Status = "Banana", Total = -99.99m };
        var empty = new Anemic.Order();
        var mixed = new Anemic.Order { CustomerName = "Contoso", OrderDate = DateTime.Now, Status = "Shipped", Total = -42.50m };
        mixed.Lines.Add(new Anemic.OrderLine(2001, -3, Anemic.Money.Create(19.99m, "USD")));
        mixed.Lines.Add(new Anemic.OrderLine(2001, 1, Anemic.Money.Create(25.00m, "EUR")));

        await using (var context = new TalkDbContext(options))
        {
            context.AnemicOrders.AddRange(banana, empty, mixed);
            await context.SaveChangesAsync();
        }

        await using (var context = new TalkDbContext(options))
        {
            var reloaded = await context.AnemicOrders.Include(o => o.Lines).OrderBy(o => o.Id).ToListAsync();
            var rows = reloaded
                .Select(o => $"Id {o.Id}   Status '{o.Status}'   Customer '{o.CustomerName}'   Lines {o.Lines.Count}   Total {o.Total}")
                .ToList();
            rows.Add("");
            rows.Add("EF Core saved and reloaded every invalid order.");

            Console.Write(OutputHelpers.BoxedArrayWithTitle("Bad: SQL Server Accepts Every Invalid Order", rows.ToArray()));
        }
    }

    private static async Task Solution7SaveAndReloadAsync(DbContextOptions<TalkDbContext> options)
    {
        //TODO: Talk Step 7 — Delete the next line and uncomment the code below it
        throw new NotImplementedException();
        //var order = Domain.Order.Place("Contoso", new[] { (2001, 2, Domain.Money.Create(19.99m, "USD")), (2002, 1, Domain.Money.Create(5.00m, "USD")) });
        //order.Process();
        //order.Confirm();
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
        //    var statusColumn = await context.Database
        //        .SqlQueryRaw<string>("SELECT Status AS Value FROM DomainOrders WHERE Id = {0}", order.Id)
        //        .SingleAsync();
        //
        //    Console.Write(OutputHelpers.BoxedArrayWithTitle(
        //        "Good: EF Core Saves and Reloads the Good Model",
        //        new[]
        //        {
        //            $"Id {reloaded.Id}   Status {reloaded.Status}   Customer {reloaded.CustomerName}   Lines {reloaded.Lines.Count}",
        //            $"Total: {reloaded.Total.Amount} {reloaded.Total.Currency}",
        //            $"Status column in SQL Server: '{statusColumn}'",
        //            "",
        //            "EF Core used the private constructor and filled _lines directly.",
        //            "The good model contains no EF Core code."
        //        }
        //    ));
        //}
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
