using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleService.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientUid);
                });

            migrationBuilder.CreateTable(
                name: "Providers",
                columns: table => new
                {
                    ProviderUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Providers", x => x.ProviderUid);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentSlots",
                columns: table => new
                {
                    AppointmentSlotUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSlots", x => x.AppointmentSlotUid);
                    table.ForeignKey(
                        name: "FK_AppointmentSlots_Providers_ProviderUid",
                        column: x => x.ProviderUid,
                        principalTable: "Providers",
                        principalColumn: "ProviderUid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    AppointmentUID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentSlotUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookedDateTimeUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Confirmed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.AppointmentUID);
                    table.ForeignKey(
                        name: "FK_Appointments_AppointmentSlots_AppointmentSlotUid",
                        column: x => x.AppointmentSlotUid,
                        principalTable: "AppointmentSlots",
                        principalColumn: "AppointmentSlotUid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Clients_ClientUid",
                        column: x => x.ClientUid,
                        principalTable: "Clients",
                        principalColumn: "ClientUid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppointmentSlotUid",
                table: "Appointments",
                column: "AppointmentSlotUid");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClientUid",
                table: "Appointments",
                column: "ClientUid");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_ProviderUid",
                table: "AppointmentSlots",
                column: "ProviderUid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "AppointmentSlots");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Providers");
        }
    }
}
