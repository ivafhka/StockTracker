using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticker = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    target_price = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portfolios",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_portfolios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "price_ticks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticker = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    volume = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_ticks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    display_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "positions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    portfolio_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ticker = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    avg_buy_price = table.Column<decimal>(type: "numeric(18,8)", nullable: false),
                    avg_buy_Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    opened_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_positions", x => x.id);
                    table.ForeignKey(
                        name: "FK_positions_portfolios_portfolio_id",
                        column: x => x.portfolio_id,
                        principalTable: "portfolios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_alerts_ticker_active",
                table: "alerts",
                columns: new[] { "ticker", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_alerts_user_id",
                table: "alerts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_portfolios_user_id",
                table: "portfolios",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_positions_portfolio_id",
                table: "positions",
                column: "portfolio_id");

            migrationBuilder.CreateIndex(
                name: "ix_positions_ticker",
                table: "positions",
                column: "ticker");

            migrationBuilder.CreateIndex(
                name: "ix_price_ticks_ticker_recorded_at_desc",
                table: "price_ticks",
                columns: new[] { "ticker", "recorded_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropTable(
                name: "positions");

            migrationBuilder.DropTable(
                name: "price_ticks");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "portfolios");
        }
    }
}
