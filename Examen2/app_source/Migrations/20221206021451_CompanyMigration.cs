﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace appsource.Migrations
{
    /// <inheritdoc />
    public partial class CompanyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyModel",
                columns: table => new
                {
                    Nombre = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TipoNegocio = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PaisBase = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ValorEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EsTransnacional = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyModel", x => x.Nombre);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyModel");
        }
    }
}
