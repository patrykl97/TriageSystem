using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TriageSystem.Migrations
{
    public partial class AddedIdToPatient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientCheckIns_Patients_PPS",
                table: "PatientCheckIns");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientWaitingList_Patients_PPS",
                table: "PatientWaitingList");

            migrationBuilder.DropIndex(
                name: "IX_PatientWaitingList_PPS",
                table: "PatientWaitingList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_PatientCheckIns_PPS",
                table: "PatientCheckIns");

            migrationBuilder.AlterColumn<string>(
                name: "PPS",
                table: "PatientWaitingList",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "PatientWaitingList",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "PPS",
                table: "Patients",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Patients",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "PPS",
                table: "PatientCheckIns",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "PatientCheckIns",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_PatientWaitingList_PatientId",
                table: "PatientWaitingList",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCheckIns_PatientId",
                table: "PatientCheckIns",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCheckIns_Patients_PatientId",
                table: "PatientCheckIns",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientWaitingList_Patients_PatientId",
                table: "PatientWaitingList",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientCheckIns_Patients_PatientId",
                table: "PatientCheckIns");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientWaitingList_Patients_PatientId",
                table: "PatientWaitingList");

            migrationBuilder.DropIndex(
                name: "IX_PatientWaitingList_PatientId",
                table: "PatientWaitingList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_PatientCheckIns_PatientId",
                table: "PatientCheckIns");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "PatientWaitingList");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "PatientCheckIns");

            migrationBuilder.AlterColumn<string>(
                name: "PPS",
                table: "PatientWaitingList",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PPS",
                table: "Patients",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PPS",
                table: "PatientCheckIns",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "PPS");

            migrationBuilder.CreateIndex(
                name: "IX_PatientWaitingList_PPS",
                table: "PatientWaitingList",
                column: "PPS");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCheckIns_PPS",
                table: "PatientCheckIns",
                column: "PPS");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCheckIns_Patients_PPS",
                table: "PatientCheckIns",
                column: "PPS",
                principalTable: "Patients",
                principalColumn: "PPS",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientWaitingList_Patients_PPS",
                table: "PatientWaitingList",
                column: "PPS",
                principalTable: "Patients",
                principalColumn: "PPS",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
