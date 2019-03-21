using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TriageSystem.Migrations
{
    public partial class addedInfections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientCheckIns_Patients_PPS",
                table: "PatientCheckIns");

            migrationBuilder.RenameColumn(
                name: "Condition",
                table: "PatientCheckIns",
                newName: "Infections");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Time_checked_in",
                table: "PatientWaitingList",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "PPS",
                table: "PatientCheckIns",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Arrival",
                table: "PatientCheckIns",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCheckIns_Patients_PPS",
                table: "PatientCheckIns",
                column: "PPS",
                principalTable: "Patients",
                principalColumn: "PPS",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientCheckIns_Patients_PPS",
                table: "PatientCheckIns");

            migrationBuilder.DropColumn(
                name: "Arrival",
                table: "PatientCheckIns");

            migrationBuilder.RenameColumn(
                name: "Infections",
                table: "PatientCheckIns",
                newName: "Condition");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Time_checked_in",
                table: "PatientWaitingList",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PPS",
                table: "PatientCheckIns",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCheckIns_Patients_PPS",
                table: "PatientCheckIns",
                column: "PPS",
                principalTable: "Patients",
                principalColumn: "PPS",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
