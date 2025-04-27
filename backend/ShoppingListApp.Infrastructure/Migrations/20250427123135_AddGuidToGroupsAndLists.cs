using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingListApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidToGroupsAndLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingItems_ShoppingLists_ShoppingListId",
                table: "ShoppingItems");

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "ShoppingLists",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("UPDATE Groups SET Guid = NEWID()");
            migrationBuilder.Sql("UPDATE ShoppingLists SET Guid = NEWID()");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Guid",
                table: "Groups",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_Guid",
                table: "ShoppingLists",
                column: "Guid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingItems_ShoppingLists_ShoppingListId",
                table: "ShoppingItems",
                column: "ShoppingListId",
                principalTable: "ShoppingLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingItems_ShoppingLists_ShoppingListId",
                table: "ShoppingItems");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Groups");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingItems_ShoppingLists_ShoppingListId",
                table: "ShoppingItems",
                column: "ShoppingListId",
                principalTable: "ShoppingLists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
