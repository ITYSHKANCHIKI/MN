using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoralNavigator.API.Migrations
{
    public partial class AddPasswordSaltToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Сначала конвертируем существующий текст в bytea:
            migrationBuilder.Sql(
                @"ALTER TABLE ""Users"" 
                ALTER COLUMN ""PasswordHash"" 
                TYPE bytea 
                USING ""PasswordHash""::bytea;");

            // 2) Затем добавляем новое поле PasswordSalt
            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Users",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // откат добавления PasswordSalt
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");

            // конвертируем назад в текст (если вам это нужно)
            migrationBuilder.Sql(
                @"ALTER TABLE ""Users"" 
                ALTER COLUMN ""PasswordHash"" 
                TYPE text 
                USING encode(""PasswordHash"", 'hex');");
        }
    }
}
