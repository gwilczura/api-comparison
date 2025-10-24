using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Wilczura.Demo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "demo");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "countries",
                schema: "demo",
                columns: table => new
                {
                    country_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "citext", nullable: false),
                    name = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_countries", x => x.country_id);
                });

            migrationBuilder.CreateTable(
                name: "company_locations",
                schema: "demo",
                columns: table => new
                {
                    company_location_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    country_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "citext", nullable: false),
                    description = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_company_locations", x => x.company_location_id);
                    table.ForeignKey(
                        name: "fk_company_locations_countries_country_id",
                        column: x => x.country_id,
                        principalSchema: "demo",
                        principalTable: "countries",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                schema: "demo",
                columns: table => new
                {
                    employee_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_location_id = table.Column<long>(type: "bigint", nullable: true),
                    email = table.Column<string>(type: "citext", nullable: false),
                    name = table.Column<string>(type: "citext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_employees", x => x.employee_id);
                    table.ForeignKey(
                        name: "fk_employees_company_locations_company_location_id",
                        column: x => x.company_location_id,
                        principalSchema: "demo",
                        principalTable: "company_locations",
                        principalColumn: "company_location_id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_company_locations_country_id",
                schema: "demo",
                table: "company_locations",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_employees_company_location_id",
                schema: "demo",
                table: "employees",
                column: "company_location_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "employees",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "company_locations",
                schema: "demo");

            migrationBuilder.DropTable(
                name: "countries",
                schema: "demo");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:citext", ",,");
        }
    }
}
