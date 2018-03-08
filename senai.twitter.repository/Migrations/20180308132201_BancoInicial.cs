using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace senai.twitter.repository.Migrations
{
    public partial class BancoInicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AtualizadoEm = table.Column<DateTime>(nullable: false),
                    AtualizadoPor = table.Column<string>(nullable: true),
                    CriadoEm = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: false),
                    NomeUsuario = table.Column<string>(maxLength: 50, nullable: false),
                    QtdAtualizacoes = table.Column<int>(nullable: false),
                    Senha = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AtualizadoEm = table.Column<DateTime>(nullable: false),
                    AtualizadoPor = table.Column<string>(nullable: true),
                    AvatarUrl = table.Column<string>(maxLength: 200, nullable: true),
                    Bio = table.Column<string>(maxLength: 200, nullable: true),
                    Cidade = table.Column<string>(maxLength: 50, nullable: false),
                    CriadoEm = table.Column<DateTime>(nullable: false),
                    DataNascimento = table.Column<DateTime>(nullable: false),
                    Estado = table.Column<string>(maxLength: 50, nullable: false),
                    IdLogin = table.Column<int>(nullable: false),
                    Nome = table.Column<string>(maxLength: 50, nullable: false),
                    QtdAtualizacoes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Perfis_Logins_IdLogin",
                        column: x => x.IdLogin,
                        principalTable: "Logins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RotasPesquisas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AtualizadoEm = table.Column<DateTime>(nullable: false),
                    AtualizadoPor = table.Column<string>(nullable: true),
                    CriadoEm = table.Column<DateTime>(nullable: false),
                    DestinoEnd = table.Column<string>(nullable: false),
                    DestinoLat = table.Column<double>(nullable: false),
                    DestinoLng = table.Column<double>(nullable: false),
                    Distancia = table.Column<int>(nullable: false),
                    Duracao = table.Column<string>(nullable: false),
                    IdLogin = table.Column<int>(nullable: false),
                    OrigemEnd = table.Column<string>(nullable: false),
                    OrigemLat = table.Column<double>(nullable: false),
                    OrigemLng = table.Column<double>(nullable: false),
                    PolylinePoints = table.Column<string>(nullable: false),
                    QtdAtualizacoes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotasPesquisas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotasPesquisas_Logins_IdLogin",
                        column: x => x.IdLogin,
                        principalTable: "Logins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logins_Email",
                table: "Logins",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logins_NomeUsuario",
                table: "Logins",
                column: "NomeUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Perfis_IdLogin",
                table: "Perfis",
                column: "IdLogin",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RotasPesquisas_IdLogin",
                table: "RotasPesquisas",
                column: "IdLogin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Perfis");

            migrationBuilder.DropTable(
                name: "RotasPesquisas");

            migrationBuilder.DropTable(
                name: "Logins");
        }
    }
}
