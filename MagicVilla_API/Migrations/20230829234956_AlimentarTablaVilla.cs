using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTablaVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenURL", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Detalle de la villa", new DateTime(2023, 8, 29, 20, 49, 56, 409, DateTimeKind.Local).AddTicks(6383), new DateTime(2023, 8, 29, 20, 49, 56, 409, DateTimeKind.Local).AddTicks(6370), "", 50, "VillaReal", 5, 200.0 },
                    { 2, "", "Detalle de la villa", new DateTime(2023, 8, 29, 20, 49, 56, 409, DateTimeKind.Local).AddTicks(6386), new DateTime(2023, 8, 29, 20, 49, 56, 409, DateTimeKind.Local).AddTicks(6386), "", 40, "Premium Vista a la Piscina", 4, 150.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
