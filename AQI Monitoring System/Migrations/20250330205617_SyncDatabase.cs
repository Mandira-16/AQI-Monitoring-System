using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AQI_Monitoring_System.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_AqiReadings_SensorId1",
                table: "AqiReadings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Sensors");

            migrationBuilder.DropColumn(
                name: "SensorId1",
                table: "AqiReadings");

            migrationBuilder.AlterColumn<string>(
                name: "SensorId",
                table: "Sensors",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "SensorId",
                table: "AqiReadings",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors",
                column: "SensorId");

            migrationBuilder.CreateIndex(
                name: "IX_AqiReadings_SensorId",
                table: "AqiReadings",
                column: "SensorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AqiReadings_Sensors_SensorId",
                table: "AqiReadings",
                column: "SensorId",
                principalTable: "Sensors",
                principalColumn: "SensorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AqiReadings_Sensors_SensorId",
                table: "AqiReadings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors");

            migrationBuilder.DropIndex(
                name: "IX_AqiReadings_SensorId",
                table: "AqiReadings");

            migrationBuilder.AlterColumn<string>(
                name: "SensorId",
                table: "Sensors",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Sensors",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "SensorId",
                table: "AqiReadings",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SensorId1",
                table: "AqiReadings",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sensors",
                table: "Sensors",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AqiReadings_SensorId1",
                table: "AqiReadings",
                column: "SensorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AqiReadings_Sensors_SensorId1",
                table: "AqiReadings",
                column: "SensorId1",
                principalTable: "Sensors",
                principalColumn: "Id");
        }
    }
}
