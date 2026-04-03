using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIJobMatch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserSubplanUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountsId",
                table: "UserSubscriptions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_AccountsId",
                table: "UserSubscriptions",
                column: "AccountsId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_Accounts_AccountsId",
                table: "UserSubscriptions",
                column: "AccountsId",
                principalTable: "Accounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_Accounts_AccountsId",
                table: "UserSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_AccountsId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "AccountsId",
                table: "UserSubscriptions");
        }
    }
}
