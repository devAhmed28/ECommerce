using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Modules.Identity.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenFamilyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FamilyId",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("""
            UPDATE RefreshTokens
            SET FamilyId = NEWID()
            WHERE FamilyId IS NULL;
            """);

            migrationBuilder.AlterColumn<Guid>(
                name: "FamilyId",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FamilyId",
                table: "RefreshTokens");
        }
    }
}
