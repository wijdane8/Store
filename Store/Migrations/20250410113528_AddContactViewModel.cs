using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Migrations
{
    /// <inheritdoc />
    public partial class AddContactViewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK__CartItem__CartI__4F7CD00D",
            //    table: "CartItem");

            //migrationBuilder.DropForeignKey(
            //    name: "FK__CartItem__Produ__5070F446",
            //    table: "CartItem");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK__CartItem__3214EC070EF1F339",
            //    table: "CartItem");

            //migrationBuilder.RenameTable(
            //    name: "CartItem",
            //    newName: "CartItems");

            //migrationBuilder.RenameIndex(
            //    name: "UQ__CartItem__9AFC1BDA45E59E96",
            //    table: "CartItems",
            //    newName: "UQ__CartItems__9AFC1BDA45E59E96");

            //migrationBuilder.RenameIndex(
            //    name: "IX_CartItem_ProductId",
            //    table: "CartItems",
            //    newName: "IX_CartItems_ProductId");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK__CartItems__3214EC070EF1F339",
            //    table: "CartItems",
            //    column: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK__CartItems__CartI__4F7CD00D",
            //    table: "CartItems",
            //    column: "CartId",
            //    principalTable: "Carts",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK__CartItems__Produ__5070F446",
            //    table: "CartItems",
            //    column: "ProductId",
            //    principalTable: "Products",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK__CartItems__CartI__4F7CD00D",
            //    table: "CartItems");

            //migrationBuilder.DropForeignKey(
            //    name: "FK__CartItems__Produ__5070F446",
            //    table: "CartItems");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK__CartItems__3214EC070EF1F339",
            //    table: "CartItems");

            //migrationBuilder.RenameTable(
            //    name: "CartItems",
            //    newName: "CartItem");

            //migrationBuilder.RenameIndex(
            //    name: "UQ__CartItems__9AFC1BDA45E59E96",
            //    table: "CartItem",
            //    newName: "UQ__CartItem__9AFC1BDA45E59E96");

            //migrationBuilder.RenameIndex(
            //    name: "IX_CartItems_ProductId",
            //    table: "CartItem",
            //    newName: "IX_CartItem_ProductId");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK__CartItem__3214EC070EF1F339",
            //    table: "CartItem",
            //    column: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK__CartItem__CartI__4F7CD00D",
            //    table: "CartItem",
            //    column: "CartId",
            //    principalTable: "Carts",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK__CartItem__Produ__5070F446",
            //    table: "CartItem",
            //    column: "ProductId",
            //    principalTable: "Products",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
