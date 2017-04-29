using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace A2SPA.Migrations
{
    public partial class Pso2CsvModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Pso2CsvKeyKey3",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Pso2CsvKeyKey2",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Pso2CsvKeyKey1",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Pso2CsvKeyKey3_Value",
                table: "Pso2CsvKeyKey3",
                column: "Value");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Pso2CsvKeyKey2_Value",
                table: "Pso2CsvKeyKey2",
                column: "Value");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Pso2CsvKeyKey1_Value",
                table: "Pso2CsvKeyKey1",
                column: "Value");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Pso2CsvKeyKey3_Value",
                table: "Pso2CsvKeyKey3");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Pso2CsvKeyKey2_Value",
                table: "Pso2CsvKeyKey2");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Pso2CsvKeyKey1_Value",
                table: "Pso2CsvKeyKey1");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Pso2CsvKeyKey3",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Pso2CsvKeyKey2",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Pso2CsvKeyKey1",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
