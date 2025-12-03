using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyLittleRPG_ElGuendouz.Migrations
{
    /// <inheritdoc />
    public partial class AddChasseHistoriqueAndQuestTimerTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChasseHistorique",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    idPersonnage = table.Column<int>(type: "int", nullable: false),
                    idMonstre = table.Column<int>(type: "int", nullable: false),
                    DateChasse = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Vaincu = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChasseHistorique", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChasseHistorique_Character_idPersonnage",
                        column: x => x.idPersonnage,
                        principalTable: "Character",
                        principalColumn: "idPersonnage",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChasseHistorique_Monsters_idMonstre",
                        column: x => x.idMonstre,
                        principalTable: "Monsters",
                        principalColumn: "idMonster",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QuestTimer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NextGenerationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastGenerationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IntervalMinutes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestTimer", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ChasseHistorique_idMonstre",
                table: "ChasseHistorique",
                column: "idMonstre");

            migrationBuilder.CreateIndex(
                name: "IX_ChasseHistorique_idPersonnage",
                table: "ChasseHistorique",
                column: "idPersonnage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Les tables n'ont jamais été créées, donc rien à supprimer
        }
    }
}
