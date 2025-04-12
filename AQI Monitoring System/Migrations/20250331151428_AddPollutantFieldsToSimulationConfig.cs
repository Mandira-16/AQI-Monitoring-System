using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AQI_Monitoring_System.Migrations
{
    /// <inheritdoc />
    public partial class AddPollutantFieldsToSimulationConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BaselineCo",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BaselineNo2",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BaselineO3",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BaselinePm10",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BaselinePm25",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BaselineSo2",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VariationCo",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VariationNo2",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VariationO3",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VariationPm10",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VariationPm25",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VariationSo2",
                table: "SimulationConfigs",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaselineCo",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "BaselineNo2",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "BaselineO3",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "BaselinePm10",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "BaselinePm25",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "BaselineSo2",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "VariationCo",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "VariationNo2",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "VariationO3",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "VariationPm10",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "VariationPm25",
                table: "SimulationConfigs");

            migrationBuilder.DropColumn(
                name: "VariationSo2",
                table: "SimulationConfigs");
        }
    }
}
