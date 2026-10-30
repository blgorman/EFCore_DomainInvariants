using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TalkApp;
using TalkApp.Data;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var options = new DbContextOptionsBuilder<TalkDbContext>()
    .UseSqlServer(config.GetConnectionString("TalkDbConnection"))
    .Options;

await using (var context = new TalkDbContext(options))
{
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
}

await TalkDemo.RunAsync(options);
