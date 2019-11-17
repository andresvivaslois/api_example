using GNB.Infrastructure.Models;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace GNB.Infrastructure.Factories
{
    public class BankContextFactory : IDesignTimeDbContextFactory<BankContext>
    {
        public BankContext CreateDbContext(string[] args)
        {
            BankContext context = new BankContext();
            return context;
        }
    }
}
