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
                name: "RequisicoesAlteracaoSenha",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false),
                    Expiracao = table.Column<DateTime>(nullable: false),
                    IdLogin = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisicoesAlteracaoSenha", x => x.Id);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RotasPesquisadas",
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
                    table.PrimaryKey("PK_RotasPesquisadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotasPesquisadas_Logins_IdLogin",
                        column: x => x.IdLogin,
                        principalTable: "Logins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RotasRealizadas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AtualizadoEm = table.Column<DateTime>(nullable: false),
                    AtualizadoPor = table.Column<string>(nullable: true),
                    CriadoEm = table.Column<DateTime>(nullable: false),
                    DuracaoInt = table.Column<int>(nullable: false),
                    DuracaoString = table.Column<string>(nullable: false),
                    IdLogin = table.Column<int>(nullable: false),
                    IdRotaPesquisada = table.Column<int>(nullable: false),
                    Kilometros = table.Column<int>(nullable: false),
                    LatFim = table.Column<double>(nullable: false),
                    LatInicio = table.Column<double>(nullable: false),
                    LngFim = table.Column<double>(nullable: false),
                    LngInicio = table.Column<double>(nullable: false),
                    QtdAtualizacoes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotasRealizadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotasRealizadas_Logins_IdLogin",
                        column: x => x.IdLogin,
                        principalTable: "Logins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RotasRealizadas_RotasPesquisadas_IdRotaPesquisada",
                        column: x => x.IdRotaPesquisada,
                        principalTable: "RotasPesquisadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Avaliacoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AtualizadoEm = table.Column<DateTime>(nullable: false),
                    AtualizadoPor = table.Column<string>(nullable: true),
                    AvSeguranca = table.Column<int>(nullable: false),
                    AvTrajeto = table.Column<int>(nullable: false),
                    CriadoEm = table.Column<DateTime>(nullable: false),
                    IdLogin = table.Column<int>(nullable: false),
                    IdRotaRealizada = table.Column<int>(nullable: false),
                    QtdAtualizacoes = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_Logins_IdLogin",
                        column: x => x.IdLogin,
                        principalTable: "Logins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_RotasRealizadas_IdRotaRealizada",
                        column: x => x.IdRotaRealizada,
                        principalTable: "RotasRealizadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_IdLogin",
                table: "Avaliacoes",
                column: "IdLogin");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_IdRotaRealizada",
                table: "Avaliacoes",
                column: "IdRotaRealizada",
                unique: true);

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
                name: "IX_RotasPesquisadas_IdLogin",
                table: "RotasPesquisadas",
                column: "IdLogin");

            migrationBuilder.CreateIndex(
                name: "IX_RotasRealizadas_IdLogin",
                table: "RotasRealizadas",
                column: "IdLogin");

            migrationBuilder.CreateIndex(
                name: "IX_RotasRealizadas_IdRotaPesquisada",
                table: "RotasRealizadas",
                column: "IdRotaPesquisada",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacoes");

            migrationBuilder.DropTable(
                name: "Perfis");

            migrationBuilder.DropTable(
                name: "RequisicoesAlteracaoSenha");

            migrationBuilder.DropTable(
                name: "RotasRealizadas");

            migrationBuilder.DropTable(
                name: "RotasPesquisadas");

            migrationBuilder.DropTable(
                name: "Logins");
        }
    }
}
