using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TriageSystem.Migrations
{
    public partial class updatedModels3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlowchartName",
                table: "PatientWaitingList",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientAdmitted",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PatientId = table.Column<int>(nullable: false),
                    Infections = table.Column<string>(nullable: true),
                    Arrival = table.Column<string>(nullable: true),
                    Condition = table.Column<string>(nullable: false),
                    FinalCondition = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false),
                    Time_checked_in = table.Column<DateTime>(nullable: false),
                    Time_triaged = table.Column<DateTime>(nullable: false),
                    Time_admitted = table.Column<DateTime>(nullable: false),
                    Time_released = table.Column<DateTime>(nullable: true),
                    HospitalID = table.Column<int>(nullable: false),
                    FlowchartName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAdmitted", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientAdmitted");

            migrationBuilder.DropColumn(
                name: "FlowchartName",
                table: "PatientWaitingList");
        }
    }
}
