﻿using System.Linq;
using A2SPA.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace A2SPA.Data
{
    public static class DbInitializer
    {
        public static void Initialize(A2spaContext context)
        {
            // performsny outstanding migrations.
            context.Database.Migrate();

            // Look for any test data.
            if (context.TestData.Any())
            {
                return;   // DB has been seeded
            }

            var testData = new TestData
            {
                Username = "JaneDoe",
                EmailAddress = "jane.doe@example.com",
                Password = "LetM@In!",
                Currency = 321.45M
            };

            context.TestData.Add(testData);
            context.SaveChanges();
        }
    }
}
