using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PRN232.Student.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.StudentId);
                });

            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "StudentId", "DateOfBirth", "Email", "FullName" },
                values: new object[,]
                {
                    { 1, new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "student01@lms.local", "Nguyen An 01" },
                    { 2, new DateTime(2002, 2, 2, 0, 0, 0, 0, DateTimeKind.Utc), "student02@lms.local", "Tran Binh 02" },
                    { 3, new DateTime(2003, 3, 3, 0, 0, 0, 0, DateTimeKind.Utc), "student03@lms.local", "Le Chi 03" },
                    { 4, new DateTime(2004, 4, 4, 0, 0, 0, 0, DateTimeKind.Utc), "student04@lms.local", "Pham Dung 04" },
                    { 5, new DateTime(2000, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "student05@lms.local", "Hoang Hanh 05" },
                    { 6, new DateTime(2001, 6, 6, 0, 0, 0, 0, DateTimeKind.Utc), "student06@lms.local", "Phan Khoa 06" },
                    { 7, new DateTime(2002, 7, 7, 0, 0, 0, 0, DateTimeKind.Utc), "student07@lms.local", "Vu Linh 07" },
                    { 8, new DateTime(2003, 8, 8, 0, 0, 0, 0, DateTimeKind.Utc), "student08@lms.local", "Dang Minh 08" },
                    { 9, new DateTime(2004, 9, 9, 0, 0, 0, 0, DateTimeKind.Utc), "student09@lms.local", "Bui Nam 09" },
                    { 10, new DateTime(2000, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc), "student10@lms.local", "Do Quyen 10" },
                    { 11, new DateTime(2001, 11, 11, 0, 0, 0, 0, DateTimeKind.Utc), "student11@lms.local", "Nguyen An 11" },
                    { 12, new DateTime(2002, 12, 12, 0, 0, 0, 0, DateTimeKind.Utc), "student12@lms.local", "Tran Binh 12" },
                    { 13, new DateTime(2003, 1, 13, 0, 0, 0, 0, DateTimeKind.Utc), "student13@lms.local", "Le Chi 13" },
                    { 14, new DateTime(2004, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), "student14@lms.local", "Pham Dung 14" },
                    { 15, new DateTime(2000, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "student15@lms.local", "Hoang Hanh 15" },
                    { 16, new DateTime(2001, 4, 16, 0, 0, 0, 0, DateTimeKind.Utc), "student16@lms.local", "Phan Khoa 16" },
                    { 17, new DateTime(2002, 5, 17, 0, 0, 0, 0, DateTimeKind.Utc), "student17@lms.local", "Vu Linh 17" },
                    { 18, new DateTime(2003, 6, 18, 0, 0, 0, 0, DateTimeKind.Utc), "student18@lms.local", "Dang Minh 18" },
                    { 19, new DateTime(2004, 7, 19, 0, 0, 0, 0, DateTimeKind.Utc), "student19@lms.local", "Bui Nam 19" },
                    { 20, new DateTime(2000, 8, 20, 0, 0, 0, 0, DateTimeKind.Utc), "student20@lms.local", "Do Quyen 20" },
                    { 21, new DateTime(2001, 9, 21, 0, 0, 0, 0, DateTimeKind.Utc), "student21@lms.local", "Nguyen An 21" },
                    { 22, new DateTime(2002, 10, 22, 0, 0, 0, 0, DateTimeKind.Utc), "student22@lms.local", "Tran Binh 22" },
                    { 23, new DateTime(2003, 11, 23, 0, 0, 0, 0, DateTimeKind.Utc), "student23@lms.local", "Le Chi 23" },
                    { 24, new DateTime(2004, 12, 24, 0, 0, 0, 0, DateTimeKind.Utc), "student24@lms.local", "Pham Dung 24" },
                    { 25, new DateTime(2000, 1, 25, 0, 0, 0, 0, DateTimeKind.Utc), "student25@lms.local", "Hoang Hanh 25" },
                    { 26, new DateTime(2001, 2, 26, 0, 0, 0, 0, DateTimeKind.Utc), "student26@lms.local", "Phan Khoa 26" },
                    { 27, new DateTime(2002, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), "student27@lms.local", "Vu Linh 27" },
                    { 28, new DateTime(2003, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "student28@lms.local", "Dang Minh 28" },
                    { 29, new DateTime(2004, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), "student29@lms.local", "Bui Nam 29" },
                    { 30, new DateTime(2000, 6, 3, 0, 0, 0, 0, DateTimeKind.Utc), "student30@lms.local", "Do Quyen 30" },
                    { 31, new DateTime(2001, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "student31@lms.local", "Nguyen An 31" },
                    { 32, new DateTime(2002, 8, 5, 0, 0, 0, 0, DateTimeKind.Utc), "student32@lms.local", "Tran Binh 32" },
                    { 33, new DateTime(2003, 9, 6, 0, 0, 0, 0, DateTimeKind.Utc), "student33@lms.local", "Le Chi 33" },
                    { 34, new DateTime(2004, 10, 7, 0, 0, 0, 0, DateTimeKind.Utc), "student34@lms.local", "Pham Dung 34" },
                    { 35, new DateTime(2000, 11, 8, 0, 0, 0, 0, DateTimeKind.Utc), "student35@lms.local", "Hoang Hanh 35" },
                    { 36, new DateTime(2001, 12, 9, 0, 0, 0, 0, DateTimeKind.Utc), "student36@lms.local", "Phan Khoa 36" },
                    { 37, new DateTime(2002, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "student37@lms.local", "Vu Linh 37" },
                    { 38, new DateTime(2003, 2, 11, 0, 0, 0, 0, DateTimeKind.Utc), "student38@lms.local", "Dang Minh 38" },
                    { 39, new DateTime(2004, 3, 12, 0, 0, 0, 0, DateTimeKind.Utc), "student39@lms.local", "Bui Nam 39" },
                    { 40, new DateTime(2000, 4, 13, 0, 0, 0, 0, DateTimeKind.Utc), "student40@lms.local", "Do Quyen 40" },
                    { 41, new DateTime(2001, 5, 14, 0, 0, 0, 0, DateTimeKind.Utc), "student41@lms.local", "Nguyen An 41" },
                    { 42, new DateTime(2002, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), "student42@lms.local", "Tran Binh 42" },
                    { 43, new DateTime(2003, 7, 16, 0, 0, 0, 0, DateTimeKind.Utc), "student43@lms.local", "Le Chi 43" },
                    { 44, new DateTime(2004, 8, 17, 0, 0, 0, 0, DateTimeKind.Utc), "student44@lms.local", "Pham Dung 44" },
                    { 45, new DateTime(2000, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "student45@lms.local", "Hoang Hanh 45" },
                    { 46, new DateTime(2001, 10, 19, 0, 0, 0, 0, DateTimeKind.Utc), "student46@lms.local", "Phan Khoa 46" },
                    { 47, new DateTime(2002, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc), "student47@lms.local", "Vu Linh 47" },
                    { 48, new DateTime(2003, 12, 21, 0, 0, 0, 0, DateTimeKind.Utc), "student48@lms.local", "Dang Minh 48" },
                    { 49, new DateTime(2004, 1, 22, 0, 0, 0, 0, DateTimeKind.Utc), "student49@lms.local", "Bui Nam 49" },
                    { 50, new DateTime(2000, 2, 23, 0, 0, 0, 0, DateTimeKind.Utc), "student50@lms.local", "Do Quyen 50" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Student_Email",
                table: "Student",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Student");
        }
    }
}
