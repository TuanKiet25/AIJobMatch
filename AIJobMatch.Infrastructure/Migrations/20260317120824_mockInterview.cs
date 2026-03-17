using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIJobMatch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mockInterview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MockInterview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CandidateId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomTargetJobTitle = table.Column<string>(type: "text", nullable: true),
                    OverallScore = table.Column<double>(type: "double precision", nullable: true),
                    OverallFeedback = table.Column<string>(type: "text", nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CvSnapshot = table.Column<string>(type: "text", nullable: false),
                    InterviewDifficulty = table.Column<int>(type: "integer", nullable: false),
                    InterviewStatus = table.Column<int>(type: "integer", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockInterview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockInterview_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MockInterviewDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MockInterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionOrder = table.Column<int>(type: "integer", nullable: false),
                    QuestionText = table.Column<string>(type: "text", nullable: false),
                    CandidateAnswer = table.Column<string>(type: "text", nullable: true),
                    QuestionScore = table.Column<double>(type: "double precision", nullable: true),
                    AIFeedback = table.Column<string>(type: "text", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MockInterviewDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MockInterviewDetail_MockInterview_MockInterviewId",
                        column: x => x.MockInterviewId,
                        principalTable: "MockInterview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MockInterview_CandidateId",
                table: "MockInterview",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_MockInterviewDetail_MockInterviewId",
                table: "MockInterviewDetail",
                column: "MockInterviewId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MockInterviewDetail");

            migrationBuilder.DropTable(
                name: "MockInterview");
        }
    }
}
