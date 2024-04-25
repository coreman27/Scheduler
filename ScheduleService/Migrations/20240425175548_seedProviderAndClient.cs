using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleService.Migrations
{
    /// <inheritdoc />
    public partial class seedProviderAndClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientUid", "Name", "TimeZoneId" },
                values: new object[] { new Guid("a0b2a246-7532-4940-acb1-951099d75df1"), "client", "Eastern Standard Time" });

            migrationBuilder.InsertData(
                table: "Providers",
                columns: new[] { "ProviderUid", "Name", "TimeZoneId" },
                values: new object[] { new Guid("4a63037a-c0d8-4c6b-a1ca-ca9cdd9501a3"), "provider", "Central Standard Time" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ClientUid",
                keyValue: new Guid("a0b2a246-7532-4940-acb1-951099d75df1"));

            migrationBuilder.DeleteData(
                table: "Providers",
                keyColumn: "ProviderUid",
                keyValue: new Guid("4a63037a-c0d8-4c6b-a1ca-ca9cdd9501a3"));
        }
    }
}
