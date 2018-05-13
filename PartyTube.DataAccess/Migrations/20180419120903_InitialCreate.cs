using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PartyTube.DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Playlist",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("Sqlite:Autoincrement", true),
                    PlaylistName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("Sqlite:Autoincrement", true),
                    DurationInSeconds = table.Column<int>(nullable: false),
                    ThumbnailUrl = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    VideoIdentifier = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrentPlaylist",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("Sqlite:Autoincrement", true),
                    Order = table.Column<int>(nullable: false),
                    VideoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentPlaylist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrentPlaylist_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("Sqlite:Autoincrement", true),
                    PlayedDateTime = table.Column<DateTime>(nullable: false),
                    VideoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_History_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NowPlaying",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                              .Annotation("Sqlite:Autoincrement", true),
                    IsPlaying = table.Column<bool>(nullable: false),
                    VideoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NowPlaying", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NowPlaying_Video_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistVideoItem",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(nullable: false),
                    VideoItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistVideoItem", x => new {x.PlaylistId, x.VideoItemId});
                    table.ForeignKey(
                        name: "FK_PlaylistVideoItem_Playlist_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistVideoItem_Video_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Video",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrentPlaylist_VideoId",
                table: "CurrentPlaylist",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_History_VideoId",
                table: "History",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_NowPlaying_VideoId",
                table: "NowPlaying",
                column: "VideoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Playlist_PlaylistName",
                table: "Playlist",
                column: "PlaylistName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Video_VideoIdentifier",
                table: "Video",
                column: "VideoIdentifier",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentPlaylist");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "NowPlaying");

            migrationBuilder.DropTable(
                name: "PlaylistVideoItem");

            migrationBuilder.DropTable(
                name: "Playlist");

            migrationBuilder.DropTable(
                name: "Video");
        }
    }
}