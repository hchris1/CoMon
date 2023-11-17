using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace CoMon.EntityFrameworkCore
{
    public static class CoMonDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<CoMonDbContext> builder, string connectionString)
        {
            builder.UseNpgsql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<CoMonDbContext> builder, DbConnection connection)
        {
            builder.UseNpgsql(connection);
        }
    }
}
