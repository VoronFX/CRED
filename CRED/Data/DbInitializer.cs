﻿using System.Linq;
using CRED.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CRED.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CREDContext context)
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
