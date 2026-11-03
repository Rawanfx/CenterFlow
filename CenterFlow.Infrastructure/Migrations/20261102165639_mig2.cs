using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CenterFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Centers_CenterId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Centers_CenterId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Centers");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_CenterId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "StudentBookings");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "GradeLevel",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "CenterId",
                table: "AspNetUsers",
                newName: "GradeLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_CenterId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_GradeLevelId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "TeacherAvailabilities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "HoldExpiresAt",
                table: "StudentBookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StudentBookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RecurringBookingId",
                table: "Books",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReminderSentAt",
                table: "Books",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherSubjectAssignId",
                table: "Books",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ExperienceYear",
                table: "AspNetUsers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GradeLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecurringBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId1 = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherSubjectAssignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    From = table.Column<TimeSpan>(type: "time", nullable: false),
                    To = table.Column<TimeSpan>(type: "time", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringBookings_AspNetUsers_TeacherId1",
                        column: x => x.TeacherId1,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecurringBookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSubjectAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GradeLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSubjectAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignment_AspNetUsers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignment_GradeLevels_GradeLevelId",
                        column: x => x.GradeLevelId,
                        principalTable: "GradeLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherSubjectAssignment_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeacherGradeLevelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rates_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Rates_StudentBookings_StudentBookingId",
                        column: x => x.StudentBookingId,
                        principalTable: "StudentBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rates_TeacherSubjectAssignment_TeacherGradeLevelId",
                        column: x => x.TeacherGradeLevelId,
                        principalTable: "TeacherSubjectAssignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Books_RecurringBookingId",
                table: "Books",
                column: "RecurringBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_TeacherSubjectAssignId",
                table: "Books",
                column: "TeacherSubjectAssignId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_StudentBookingId",
                table: "Rates",
                column: "StudentBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_StudentId_TeacherGradeLevelId",
                table: "Rates",
                columns: new[] { "StudentId", "TeacherGradeLevelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rates_TeacherGradeLevelId",
                table: "Rates",
                column: "TeacherGradeLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBookings_RoomId",
                table: "RecurringBookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringBookings_TeacherId1",
                table: "RecurringBookings",
                column: "TeacherId1");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignment_GradeLevelId",
                table: "TeacherSubjectAssignment",
                column: "GradeLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignment_SubjectId",
                table: "TeacherSubjectAssignment",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignment_TeacherId",
                table: "TeacherSubjectAssignment",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_GradeLevels_GradeLevelId",
                table: "AspNetUsers",
                column: "GradeLevelId",
                principalTable: "GradeLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_RecurringBookings_RecurringBookingId",
                table: "Books",
                column: "RecurringBookingId",
                principalTable: "RecurringBookings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_TeacherSubjectAssignment_TeacherSubjectAssignId",
                table: "Books",
                column: "TeacherSubjectAssignId",
                principalTable: "TeacherSubjectAssignment",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_GradeLevels_GradeLevelId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_RecurringBookings_RecurringBookingId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_TeacherSubjectAssignment_TeacherSubjectAssignId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Rates");

            migrationBuilder.DropTable(
                name: "RecurringBookings");

            migrationBuilder.DropTable(
                name: "TeacherSubjectAssignment");

            migrationBuilder.DropTable(
                name: "GradeLevels");

            migrationBuilder.DropIndex(
                name: "IX_Books_RecurringBookingId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_TeacherSubjectAssignId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "TeacherAvailabilities");

            migrationBuilder.DropColumn(
                name: "HoldExpiresAt",
                table: "StudentBookings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentBookings");

            migrationBuilder.DropColumn(
                name: "RecurringBookingId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ReminderSentAt",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TeacherSubjectAssignId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ExperienceYear",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "GradeLevelId",
                table: "AspNetUsers",
                newName: "CenterId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_GradeLevelId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_CenterId");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "StudentBookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "CenterId",
                table: "Rooms",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "GradeLevel",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Centers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CenterId",
                table: "Rooms",
                column: "CenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Centers_CenterId",
                table: "AspNetUsers",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Centers_CenterId",
                table: "Rooms",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
