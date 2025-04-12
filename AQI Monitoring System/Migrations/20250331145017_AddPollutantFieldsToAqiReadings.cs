using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AQI_Monitoring_System.Migrations
{
    /// <inheritdoc />
    public partial class AddPollutantFieldsToAqiReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Co",
                table: "AqiReadings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "No2",
                table: "AqiReadings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "O3",
                table: "AqiReadings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Pm10",
                table: "AqiReadings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Pm25",
                table: "AqiReadings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "So2",
                table: "AqiReadings",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Co",
                table: "AqiReadings");

            migrationBuilder.DropColumn(
                name: "No2",
                table: "AqiReadings");

            migrationBuilder.DropColumn(
                name: "O3",
                table: "AqiReadings");

            migrationBuilder.DropColumn(
                name: "Pm10",
                table: "AqiReadings");

            migrationBuilder.DropColumn(
                name: "Pm25",
                table: "AqiReadings");

            migrationBuilder.DropColumn(
                name: "So2",
                table: "AqiReadings");
        }
    }
}
