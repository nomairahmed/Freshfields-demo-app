using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;

namespace ExerciseApp.Tests
{
    /// <summary>
    /// Runs the real API against a private in-memory SQLite database and a fixed clock.
    /// </summary>
    public class QuoteApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _connectionString = $"Data Source=quotes-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";

        // An in-memory SQLite database only lives while a connection to it is open.
        private readonly SqliteConnection _keepAlive;

        public QuoteApiFactory()
        {
            _keepAlive = new SqliteConnection(_connectionString);
            _keepAlive.Open();
        }

        public FakeTimeProvider Clock { get; } = new(new DateTimeOffset(2026, 6, 15, 9, 30, 0, TimeSpan.Zero));

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config => config.AddInMemoryCollection(
                new Dictionary<string, string> { ["ConnectionStrings:Quotes"] = _connectionString }));

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<TimeProvider>();
                services.AddSingleton<TimeProvider>(Clock);
            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                _keepAlive.Dispose();
        }
    }
}
