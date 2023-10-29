using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CoMon.EntityFrameworkCore
{
    public static class CoMonDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<CoMonDbContext> builder, string connectionString)
        {
            builder.UseSqlite(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<CoMonDbContext> builder, DbConnection connection)
        {
            builder.UseSqlite(connection);
        }
    }
}
