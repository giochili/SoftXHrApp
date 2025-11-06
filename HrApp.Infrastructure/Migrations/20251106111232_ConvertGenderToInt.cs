using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HrApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertGenderToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Users.Gender: string -> int (enum)
            migrationBuilder.Sql(@"
        UPDATE [Users]
        SET Gender =
            CASE
                WHEN Gender IN ('Male','1') THEN '1'
                WHEN Gender IN ('Female','2') THEN '2'
                ELSE '0'
            END
        WHERE ISNUMERIC(Gender) = 0 OR Gender IS NULL;
    ");
            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Employees.Gender: string -> int (enum)
            migrationBuilder.Sql(@"
        UPDATE [Employees]
        SET Gender =
            CASE
                WHEN Gender IN ('Male','1') THEN '1'
                WHEN Gender IN ('Female','2') THEN '2'
                ELSE '0'
            END
        WHERE ISNUMERIC(Gender) = 0 OR Gender IS NULL;
    ");
            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Employees",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
