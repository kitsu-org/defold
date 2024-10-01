using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Defold.Migrations
{
    /// <inheritdoc />
    public partial class uploaded_files_and_file_chunks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    Bucket = table.Column<string>(type: "text", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => new { x.Bucket, x.Key });
                });

            migrationBuilder.CreateTable(
                name: "FileChunks",
                columns: table => new
                {
                    Bucket = table.Column<string>(type: "text", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    ChunkIndex = table.Column<long>(type: "bigint", nullable: false),
                    ChunkHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileChunks", x => new { x.Bucket, x.Key, x.ChunkIndex });
                    table.ForeignKey(
                        name: "FK_FileChunks_UploadedFiles_Bucket_Key",
                        columns: x => new { x.Bucket, x.Key },
                        principalTable: "UploadedFiles",
                        principalColumns: new[] { "Bucket", "Key" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileChunks_Bucket_Key",
                table: "FileChunks",
                columns: new[] { "Bucket", "Key" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileChunks");

            migrationBuilder.DropTable(
                name: "UploadedFiles");
        }
    }
}
