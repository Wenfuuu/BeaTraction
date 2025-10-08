using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeaTraction.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    row_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "user"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    row_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "attractions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    row_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attractions", x => x.id);
                    table.ForeignKey(
                        name: "FK_attractions_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attraction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    registered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    row_version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registrations", x => x.id);
                    table.ForeignKey(
                        name: "FK_registrations_attractions_attraction_id",
                        column: x => x.attraction_id,
                        principalTable: "attractions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registrations_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_attractions_schedule_id",
                table: "attractions",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_registrations_attraction_id",
                table: "registrations",
                column: "attraction_id");

            migrationBuilder.CreateIndex(
                name: "uq_user_attraction",
                table: "registrations",
                columns: new[] { "user_id", "attraction_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION update_row_version()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW.row_version := OLD.row_version + 1;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_users_rowversion
                BEFORE UPDATE ON users
                FOR EACH ROW EXECUTE FUNCTION update_row_version();
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_schedules_rowversion
                BEFORE UPDATE ON schedules
                FOR EACH ROW EXECUTE FUNCTION update_row_version();
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_attractions_rowversion
                BEFORE UPDATE ON attractions
                FOR EACH ROW EXECUTE FUNCTION update_row_version();
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER trg_registrations_rowversion
                BEFORE UPDATE ON registrations
                FOR EACH ROW EXECUTE FUNCTION update_row_version();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_registrations_rowversion ON registrations;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_attractions_rowversion ON attractions;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_schedules_rowversion ON schedules;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS trg_users_rowversion ON users;");
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS update_row_version();");

            migrationBuilder.DropTable(
                name: "registrations");

            migrationBuilder.DropTable(
                name: "attractions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "schedules");
        }
    }
}
