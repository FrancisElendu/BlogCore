using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedmanytomanyconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostCategory");

            migrationBuilder.DropTable(
                name: "BlogPostTag");

            migrationBuilder.CreateTable(
                name: "BlogPostCategories",
                columns: table => new
                {
                    BlogPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostCategories", x => new { x.BlogPostId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_BlogPostCategories_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostTags",
                columns: table => new
                {
                    BlogPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostTags", x => new { x.BlogPostId, x.TagId });
                    table.ForeignKey(
                        name: "FK_BlogPostTags_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostCategories_BlogPostId",
                table: "BlogPostCategories",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostCategories_CategoryId",
                table: "BlogPostCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTags_BlogPostId",
                table: "BlogPostTags",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTags_TagId",
                table: "BlogPostTags",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogPostCategories");

            migrationBuilder.DropTable(
                name: "BlogPostTags");

            migrationBuilder.CreateTable(
                name: "BlogPostCategory",
                columns: table => new
                {
                    BlogPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostCategory", x => new { x.BlogPostId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_BlogPostCategory_BlogPosts_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogPostTag",
                columns: table => new
                {
                    BlogPostsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPostTag", x => new { x.BlogPostsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_BlogPostTag_BlogPosts_BlogPostsId",
                        column: x => x.BlogPostsId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogPostTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostCategory_CategoryId",
                table: "BlogPostCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPostTag_TagsId",
                table: "BlogPostTag",
                column: "TagsId");
        }
    }
}
