using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store.Migrations
{
    /// <inheritdoc />
    public partial class megaChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ProductReviews");

            // Try to rename the CartItems index, with existence check
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'UQ__CartItems__9AFC1BDA45E59E96' AND object_id = OBJECT_ID('CartItems'))
                BEGIN
                    EXEC sp_rename N'CartItems.UQ__CartItems__9AFC1BDA45E59E96', N'UQ__CartItem__9AFC1BDA45E59E96', N'INDEX';
                END
            ");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Remove the AddPrimaryKey for CartItems
            // migrationBuilder.AddPrimaryKey(...);

            // IMPORTANT: Remove the creation of the new ApplicationUser table.
            // migrationBuilder.CreateTable( ... );

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "([NormalizedUserName] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "([NormalizedName] IS NOT NULL)");

            // Robustly drop the old foreign key if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Carts_ApplicationUser_UserId')
                BEGIN
                    ALTER TABLE Carts DROP CONSTRAINT FK_Carts_ApplicationUser_UserId;
                END
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // **SECOND APPROACH: Update Orphaned Product Reviews**
            // **IMPORTANT:** Replace these example UPDATE statements with your actual mapping logic.
            // **You need to identify the old UserIds (potentially from the removed ApplicationUser table)
            // **and the corresponding new UserIds in the AspNetUsers table.**

            migrationBuilder.Sql(@"
                -- Example: Update reviews with old UserId 'old-user-1' to new UserId 'new-user-a'
                UPDATE ProductReviews
                SET UserId = 'your-new-user-id-1'
                WHERE UserId = 'your-old-user-id-1';
            ");

            migrationBuilder.Sql(@"
                -- Example: Update reviews with old UserId 'old-user-2' to new UserId 'new-user-b'
                UPDATE ProductReviews
                SET UserId = 'your-new-user-id-2'
                WHERE UserId = 'your-old-user-id-2';
            ");

            // Add more UPDATE statements as needed for all your orphaned records.
            // It's crucial to have a correct mapping here.

            // Finally, add the foreign key constraint AFTER updating the orphaned records
            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_AspNetUsers_UserId",
                table: "ProductReviews",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Robustly drop the old wishlist foreign key if it exists
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK__Wishlists__AspNetUsers')
                BEGIN
                    ALTER TABLE Wishlists DROP CONSTRAINT FK__Wishlists__AspNetUsers;
                END
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlists_AspNetUsers_UserId",
                table: "Wishlists",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The Down method should reverse the Up method.
            // Adjust it based on the changes you actually made in Up().

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_ApplicationUser_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_AspNetUsers_UserId",
                table: "ProductReviews");

            // You might need to reverse the data updates here if possible/necessary
            // This is often complex and might not be fully reversible.

            migrationBuilder.DropForeignKey(
                name: "FK_Wishlists_AspNetUsers_UserId",
                table: "Wishlists");

            migrationBuilder.AddForeignKey(
                name: "FK__Wishlists__AspNetUsers",
                table: "Wishlists",
                column: "UserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ProductReviews",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK__CartItem__3214EC070EF1F339",
                table: "CartItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");
        }
    }
}