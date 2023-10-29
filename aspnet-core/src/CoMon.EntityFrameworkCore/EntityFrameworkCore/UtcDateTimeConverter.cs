using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace CoMon.EntityFrameworkCore
{
    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter() : base(x => DateTime.SpecifyKind(x, DateTimeKind.Utc), x => DateTime.SpecifyKind(x, DateTimeKind.Utc))
        {
        }
    }
}