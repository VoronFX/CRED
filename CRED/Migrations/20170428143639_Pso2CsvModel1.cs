using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace A2SPA.Migrations
{
    public partial class Pso2CsvModel1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pso2CsvFiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pso2CsvFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pso2CsvKeyKey1",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pso2CsvKeyKey1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pso2CsvKeyKey2",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pso2CsvKeyKey2", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pso2CsvKeyKey3",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pso2CsvKeyKey3", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pso2CsvKeys",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key1Id = table.Column<long>(nullable: false),
                    Key2Id = table.Column<long>(nullable: false),
                    Key3Id = table.Column<long>(nullable: false),
                    Key4 = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pso2CsvKeys", x => x.Id);
                    table.UniqueConstraint("AK_Pso2CsvKeys_Key1Id_Key2Id_Key3Id_Key4", x => new { x.Key1Id, x.Key2Id, x.Key3Id, x.Key4 });
                    table.ForeignKey(
                        name: "FK_Pso2CsvKeys_Pso2CsvKeyKey1_Key1Id",
                        column: x => x.Key1Id,
                        principalTable: "Pso2CsvKeyKey1",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pso2CsvKeys_Pso2CsvKeyKey2_Key2Id",
                        column: x => x.Key2Id,
                        principalTable: "Pso2CsvKeyKey2",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pso2CsvKeys_Pso2CsvKeyKey3_Key3Id",
                        column: x => x.Key3Id,
                        principalTable: "Pso2CsvKeyKey3",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pso2CsvFileItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    KeyId = table.Column<long>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    ValueId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pso2CsvFileItems", x => new { x.Id, x.FileId });
                    table.ForeignKey(
                        name: "FK_Pso2CsvFileItems_Pso2CsvFiles_FileId",
                        column: x => x.FileId,
                        principalTable: "Pso2CsvFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pso2CsvFileItems_Pso2CsvKeys_KeyId",
                        column: x => x.KeyId,
                        principalTable: "Pso2CsvKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pso2CsvValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    FileItemId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pso2CsvValues", x => new { x.Id, x.FileItemId, x.FileId });
                    table.ForeignKey(
                        name: "FK_Pso2CsvValues_Pso2CsvFiles_FileId",
                        column: x => x.FileId,
                        principalTable: "Pso2CsvFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pso2CsvValues_Pso2CsvFileItems_FileItemId_FileId",
                        columns: x => new { x.FileItemId, x.FileId },
                        principalTable: "Pso2CsvFileItems",
                        principalColumns: new[] { "Id", "FileId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pso2CsvFileItems_FileId",
                table: "Pso2CsvFileItems",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Pso2CsvFileItems_KeyId",
                table: "Pso2CsvFileItems",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_Pso2CsvKeys_Key2Id",
                table: "Pso2CsvKeys",
                column: "Key2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pso2CsvKeys_Key3Id",
                table: "Pso2CsvKeys",
                column: "Key3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Pso2CsvValues_FileId",
                table: "Pso2CsvValues",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Pso2CsvValues_FileItemId_FileId",
                table: "Pso2CsvValues",
                columns: new[] { "FileItemId", "FileId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pso2CsvValues");

            migrationBuilder.DropTable(
                name: "Pso2CsvFileItems");

            migrationBuilder.DropTable(
                name: "Pso2CsvFiles");

            migrationBuilder.DropTable(
                name: "Pso2CsvKeys");

            migrationBuilder.DropTable(
                name: "Pso2CsvKeyKey1");

            migrationBuilder.DropTable(
                name: "Pso2CsvKeyKey2");

            migrationBuilder.DropTable(
                name: "Pso2CsvKeyKey3");
        }
    }
}
