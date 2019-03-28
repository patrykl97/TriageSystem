using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TriageSystem.Migrations
{
    public partial class updatedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Condition",
                table: "PatientWaitingList",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Time_triaged",
                table: "PatientWaitingList",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Time_triaged",
                table: "PatientCheckIns",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time_triaged",
                table: "PatientWaitingList");

            migrationBuilder.DropColumn(
                name: "Time_triaged",
                table: "PatientCheckIns");

            migrationBuilder.AlterColumn<string>(
                name: "Condition",
                table: "PatientWaitingList",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
