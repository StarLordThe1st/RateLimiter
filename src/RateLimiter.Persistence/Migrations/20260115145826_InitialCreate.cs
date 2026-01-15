using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RateLimiter.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rate_limit_policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectPattern = table.Column<string>(type: "text", nullable: false),
                    ResourcePattern = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<double>(type: "double precision", nullable: false),
                    RefillRatePerSecond = table.Column<double>(type: "double precision", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rate_limit_policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "token_bucket_states",
                columns: table => new
                {
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Resource = table.Column<string>(type: "text", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tokens = table.Column<double>(type: "double precision", nullable: false),
                    LastRefillUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_token_bucket_states", x => new { x.SubjectId, x.Resource, x.PolicyId });
                });

            migrationBuilder.CreateIndex(
                name: "IX_rate_limit_policies_IsActive",
                table: "rate_limit_policies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_rate_limit_policies_Priority",
                table: "rate_limit_policies",
                column: "Priority");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rate_limit_policies");

            migrationBuilder.DropTable(
                name: "token_bucket_states");
        }
    }
}
