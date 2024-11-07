using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaApp.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Assunto",
                schema: "dbo",
                columns: table => new
                {
                    CodAs = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assunto", x => x.CodAs);
                });

            migrationBuilder.CreateTable(
                name: "Autor",
                schema: "dbo",
                columns: table => new
                {
                    CodAu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Autor", x => x.CodAu);
                });

            migrationBuilder.CreateTable(
                name: "Livro",
                schema: "dbo",
                columns: table => new
                {
                    Codl = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Editora = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Edicao = table.Column<int>(type: "int", nullable: true),
                    AnoPublicacao = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livro", x => x.Codl);
                });

            migrationBuilder.CreateTable(
                name: "LivroAssunto",
                schema: "dbo",
                columns: table => new
                {
                    LivroCodl = table.Column<int>(type: "int", nullable: false),
                    AssuntoCodAs = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivroAssunto", x => new { x.LivroCodl, x.AssuntoCodAs });
                    table.ForeignKey(
                        name: "FK_LivroAssunto_Assunto_AssuntoCodAs",
                        column: x => x.AssuntoCodAs,
                        principalSchema: "dbo",
                        principalTable: "Assunto",
                        principalColumn: "CodAs",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LivroAssunto_Livro_LivroCodl",
                        column: x => x.LivroCodl,
                        principalSchema: "dbo",
                        principalTable: "Livro",
                        principalColumn: "Codl",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LivroAutor",
                schema: "dbo",
                columns: table => new
                {
                    LivroCodl = table.Column<int>(type: "int", nullable: false),
                    AutorCodAu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivroAutor", x => new { x.LivroCodl, x.AutorCodAu });
                    table.ForeignKey(
                        name: "FK_LivroAutor_Autor_AutorCodAu",
                        column: x => x.AutorCodAu,
                        principalSchema: "dbo",
                        principalTable: "Autor",
                        principalColumn: "CodAu",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LivroAutor_Livro_LivroCodl",
                        column: x => x.LivroCodl,
                        principalSchema: "dbo",
                        principalTable: "Livro",
                        principalColumn: "Codl",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrecoLivro",
                schema: "dbo",
                columns: table => new
                {
                    Codp = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LivroCodl = table.Column<int>(type: "int", nullable: false),
                    TipoCompra = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrecoLivro", x => x.Codp);
                    table.ForeignKey(
                        name: "FK_PrecoLivro_Livro_LivroCodl",
                        column: x => x.LivroCodl,
                        principalSchema: "dbo",
                        principalTable: "Livro",
                        principalColumn: "Codl",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assunto_CodAs",
                schema: "dbo",
                table: "Assunto",
                column: "CodAs",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Autor_CodAu",
                schema: "dbo",
                table: "Autor",
                column: "CodAu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Livro_Codl",
                schema: "dbo",
                table: "Livro",
                column: "Codl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LivroAssunto_AssuntoCodAs",
                schema: "dbo",
                table: "LivroAssunto",
                column: "AssuntoCodAs");

            migrationBuilder.CreateIndex(
                name: "IX_LivroAssunto_LivroCodl",
                schema: "dbo",
                table: "LivroAssunto",
                column: "LivroCodl");

            migrationBuilder.CreateIndex(
                name: "IX_LivroAutor_AutorCodAu",
                schema: "dbo",
                table: "LivroAutor",
                column: "AutorCodAu");

            migrationBuilder.CreateIndex(
                name: "IX_LivroAutor_LivroCodl",
                schema: "dbo",
                table: "LivroAutor",
                column: "LivroCodl");

            migrationBuilder.CreateIndex(
                name: "IX_PrecoLivro_LivroCodl",
                schema: "dbo",
                table: "PrecoLivro",
                column: "LivroCodl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LivroAssunto",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "LivroAutor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PrecoLivro",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Assunto",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Autor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Livro",
                schema: "dbo");
        }
    }
}
