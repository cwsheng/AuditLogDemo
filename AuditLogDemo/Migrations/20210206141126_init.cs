using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuditLogDemo.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Parameters = table.Column<string>(nullable: true),
                    BrowserInfo = table.Column<string>(nullable: true),
                    ClientName = table.Column<string>(nullable: true),
                    ClientIpAddress = table.Column<string>(nullable: true),
                    ExecutionDuration = table.Column<int>(nullable: false),
                    ExecutionTime = table.Column<DateTime>(nullable: false),
                    ReturnValue = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    MethodName = table.Column<string>(nullable: true),
                    ServiceName = table.Column<string>(nullable: true),
                    UserInfo = table.Column<string>(nullable: true),
                    CustomData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditInfos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditInfos");
        }
    }
}
