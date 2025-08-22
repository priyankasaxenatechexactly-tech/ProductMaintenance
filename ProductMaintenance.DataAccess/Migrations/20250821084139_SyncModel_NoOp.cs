using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductMaintenance.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_NoOp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No schema changes. This migration exists to sync the model snapshot with the current database.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op
        }
    }
}
