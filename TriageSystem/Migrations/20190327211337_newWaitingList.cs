using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TriageSystem.Migrations
{
    public partial class newWaitingList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PPS",
                table: "PatientCheckIns");

            migrationBuilder.RenameColumn(
                name: "PPS",
                table: "PatientWaitingList",
                newName: "Infections");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Time_checked_in",
                table: "PatientWaitingList",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Arrival",
                table: "PatientWaitingList",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Arrival",
                table: "PatientWaitingList");

            migrationBuilder.RenameColumn(
                name: "Infections",
                table: "PatientWaitingList",
                newName: "PPS");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Time_checked_in",
                table: "PatientWaitingList",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "PPS",
                table: "PatientCheckIns",
                nullable: true);
        }
    }
}
