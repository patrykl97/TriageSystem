using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TriageSystem.Migrations
{
    public partial class demoDay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Time_admitted",
                table: "PatientAdmitted",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "Time_admitted",
                table: "PatientAdmitted",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
