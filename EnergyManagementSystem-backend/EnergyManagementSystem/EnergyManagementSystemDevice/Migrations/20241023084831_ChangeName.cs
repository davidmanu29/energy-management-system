using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnergyManagementSystemDevice.Migrations
{
    /// <inheritdoc />
    public partial class ChangeName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "maxConsumptionPerHour",
                table: "Devices",
                newName: "MaxConsumptionPerHour");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxConsumptionPerHour",
                table: "Devices",
                newName: "maxConsumptionPerHour");
        }
    }
}
