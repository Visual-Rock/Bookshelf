using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookshelf.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_Books : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_author", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "publishers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_publisher", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    isbn = table.Column<string>(type: "text", nullable: false),
                    publish_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    publisher_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pages = table.Column<int>(type: "integer", nullable: false),
                    language = table.Column<string>(type: "text", nullable: false),
                    external_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book", x => x.id);
                    table.ForeignKey(
                        name: "fk_book_publisher",
                        column: x => x.publisher_id,
                        principalTable: "publishers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "book_author_relation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    author = table.Column<Guid>(type: "uuid", nullable: false),
                    book = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_author_relation", x => x.id);
                    table.ForeignKey(
                        name: "fk_author_book",
                        column: x => x.book,
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_book_author",
                        column: x => x.author,
                        principalTable: "authors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "book_category_relation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    book = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_category_relation", x => x.id);
                    table.ForeignKey(
                        name: "fk_book_category",
                        column: x => x.category,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_category_book",
                        column: x => x.book,
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_book_relations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false),
                    book_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_book_relation", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_book_relation_book",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_book_relation_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_book_author_relation_author",
                table: "book_author_relation",
                column: "author");

            migrationBuilder.CreateIndex(
                name: "ix_book_author_relation_book",
                table: "book_author_relation",
                column: "book");

            migrationBuilder.CreateIndex(
                name: "ix_book_category_relation_book",
                table: "book_category_relation",
                column: "book");

            migrationBuilder.CreateIndex(
                name: "ix_book_category_relation_category",
                table: "book_category_relation",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_books_publisher_id",
                table: "books",
                column: "publisher_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_book_relations_book_id",
                table: "user_book_relations",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_book_relations_user_id",
                table: "user_book_relations",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "book_author_relation");

            migrationBuilder.DropTable(
                name: "book_category_relation");

            migrationBuilder.DropTable(
                name: "user_book_relations");

            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "books");

            migrationBuilder.DropTable(
                name: "publishers");
        }
    }
}
