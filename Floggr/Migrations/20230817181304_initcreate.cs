using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Floggr.Migrations
{
    /// <inheritdoc />
    public partial class initcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodCategories",
                columns: table => new
                {
                    foodCategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategories", x => x.foodCategoryID);
                });

            migrationBuilder.CreateTable(
                name: "FoodNutrientSource",
                columns: table => new
                {
                    foodNutrientSourceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodNutrientSource", x => x.foodNutrientSourceID);
                });

            migrationBuilder.CreateTable(
                name: "MeasureUnits",
                columns: table => new
                {
                    measureUnitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasureUnits", x => x.measureUnitID);
                });

            migrationBuilder.CreateTable(
                name: "Nutrients",
                columns: table => new
                {
                    nutrientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rank = table.Column<int>(type: "int", nullable: false),
                    unitName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nutrients", x => x.nutrientID);
                });

            migrationBuilder.CreateTable(
                name: "RootFoundationFoods",
                columns: table => new
                {
                    rootID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RootFoundationFoods", x => x.rootID);
                });

            migrationBuilder.CreateTable(
                name: "InputFoods2",
                columns: table => new
                {
                    inputFoods2ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    foodClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    foodCategoryID = table.Column<int>(type: "int", nullable: true),
                    fdcId = table.Column<int>(type: "int", nullable: true),
                    publicationDate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputFoods2", x => x.inputFoods2ID);
                    table.ForeignKey(
                        name: "FK_InputFoods2_FoodCategories_foodCategoryID",
                        column: x => x.foodCategoryID,
                        principalTable: "FoodCategories",
                        principalColumn: "foodCategoryID");
                });

            migrationBuilder.CreateTable(
                name: "FoodNutrientDerivations",
                columns: table => new
                {
                    foodNutrientDerivationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    foodNutrientSourceID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodNutrientDerivations", x => x.foodNutrientDerivationID);
                    table.ForeignKey(
                        name: "FK_FoodNutrientDerivations_FoodNutrientSource_foodNutrientSourceID",
                        column: x => x.foodNutrientSourceID,
                        principalTable: "FoodNutrientSource",
                        principalColumn: "foodNutrientSourceID");
                });

            migrationBuilder.CreateTable(
                name: "FoundationFoods",
                columns: table => new
                {
                    foundationFoodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    foodClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isHistoricalReference = table.Column<bool>(type: "bit", nullable: false),
                    ndbNumber = table.Column<int>(type: "int", nullable: true),
                    dataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    foodCategoryID = table.Column<int>(type: "int", nullable: false),
                    fdcId = table.Column<int>(type: "int", nullable: false),
                    publicationDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    scientificName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rootID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundationFoods", x => x.foundationFoodID);
                    table.ForeignKey(
                        name: "FK_FoundationFoods_FoodCategories_foodCategoryID",
                        column: x => x.foodCategoryID,
                        principalTable: "FoodCategories",
                        principalColumn: "foodCategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoundationFoods_RootFoundationFoods_rootID",
                        column: x => x.rootID,
                        principalTable: "RootFoundationFoods",
                        principalColumn: "rootID");
                });

            migrationBuilder.CreateTable(
                name: "FoodNutrients",
                columns: table => new
                {
                    foodNutrientID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    nutrientID = table.Column<int>(type: "int", nullable: true),
                    dataPoints = table.Column<int>(type: "int", nullable: false),
                    foodNutrientDerivationID = table.Column<int>(type: "int", nullable: true),
                    median = table.Column<double>(type: "float", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    max = table.Column<double>(type: "float", nullable: true),
                    min = table.Column<double>(type: "float", nullable: true),
                    foundationFoodID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodNutrients", x => x.foodNutrientID);
                    table.ForeignKey(
                        name: "FK_FoodNutrients_FoodNutrientDerivations_foodNutrientDerivationID",
                        column: x => x.foodNutrientDerivationID,
                        principalTable: "FoodNutrientDerivations",
                        principalColumn: "foodNutrientDerivationID");
                    table.ForeignKey(
                        name: "FK_FoodNutrients_FoundationFoods_foundationFoodID",
                        column: x => x.foundationFoodID,
                        principalTable: "FoundationFoods",
                        principalColumn: "foundationFoodID");
                    table.ForeignKey(
                        name: "FK_FoodNutrients_Nutrients_nutrientID",
                        column: x => x.nutrientID,
                        principalTable: "Nutrients",
                        principalColumn: "nutrientID");
                });

            migrationBuilder.CreateTable(
                name: "FoodPortions",
                columns: table => new
                {
                    foodPortionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<double>(type: "float", nullable: false),
                    measureUnitID = table.Column<int>(type: "int", nullable: true),
                    modifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    gramWeight = table.Column<double>(type: "float", nullable: false),
                    sequenceNumber = table.Column<int>(type: "int", nullable: false),
                    minYearAcquired = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<double>(type: "float", nullable: false),
                    foundationFoodID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPortions", x => x.foodPortionID);
                    table.ForeignKey(
                        name: "FK_FoodPortions_FoundationFoods_foundationFoodID",
                        column: x => x.foundationFoodID,
                        principalTable: "FoundationFoods",
                        principalColumn: "foundationFoodID");
                    table.ForeignKey(
                        name: "FK_FoodPortions_MeasureUnits_measureUnitID",
                        column: x => x.measureUnitID,
                        principalTable: "MeasureUnits",
                        principalColumn: "measureUnitID");
                });

            migrationBuilder.CreateTable(
                name: "InputFoods",
                columns: table => new
                {
                    inputFoodsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    foodDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    inputFoods2ID = table.Column<int>(type: "int", nullable: true),
                    foundationFoodID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputFoods", x => x.inputFoodsID);
                    table.ForeignKey(
                        name: "FK_InputFoods_FoundationFoods_foundationFoodID",
                        column: x => x.foundationFoodID,
                        principalTable: "FoundationFoods",
                        principalColumn: "foundationFoodID");
                    table.ForeignKey(
                        name: "FK_InputFoods_InputFoods2_inputFoods2ID",
                        column: x => x.inputFoods2ID,
                        principalTable: "InputFoods2",
                        principalColumn: "inputFoods2ID");
                });

            migrationBuilder.CreateTable(
                name: "NutrientConversionFactors",
                columns: table => new
                {
                    nutrientConversionFactorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    proteinValue = table.Column<double>(type: "float", nullable: false),
                    fatValue = table.Column<double>(type: "float", nullable: false),
                    carbohydrateValue = table.Column<double>(type: "float", nullable: false),
                    value = table.Column<double>(type: "float", nullable: true),
                    foundationFoodID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutrientConversionFactors", x => x.nutrientConversionFactorID);
                    table.ForeignKey(
                        name: "FK_NutrientConversionFactors_FoundationFoods_foundationFoodID",
                        column: x => x.foundationFoodID,
                        principalTable: "FoundationFoods",
                        principalColumn: "foundationFoodID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutrientDerivations_foodNutrientSourceID",
                table: "FoodNutrientDerivations",
                column: "foodNutrientSourceID");

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutrients_foodNutrientDerivationID",
                table: "FoodNutrients",
                column: "foodNutrientDerivationID");

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutrients_foundationFoodID",
                table: "FoodNutrients",
                column: "foundationFoodID");

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutrients_nutrientID",
                table: "FoodNutrients",
                column: "nutrientID");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPortions_foundationFoodID",
                table: "FoodPortions",
                column: "foundationFoodID");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPortions_measureUnitID",
                table: "FoodPortions",
                column: "measureUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_FoundationFoods_foodCategoryID",
                table: "FoundationFoods",
                column: "foodCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_FoundationFoods_rootID",
                table: "FoundationFoods",
                column: "rootID");

            migrationBuilder.CreateIndex(
                name: "IX_InputFoods_foundationFoodID",
                table: "InputFoods",
                column: "foundationFoodID");

            migrationBuilder.CreateIndex(
                name: "IX_InputFoods_inputFoods2ID",
                table: "InputFoods",
                column: "inputFoods2ID");

            migrationBuilder.CreateIndex(
                name: "IX_InputFoods2_foodCategoryID",
                table: "InputFoods2",
                column: "foodCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_NutrientConversionFactors_foundationFoodID",
                table: "NutrientConversionFactors",
                column: "foundationFoodID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodNutrients");

            migrationBuilder.DropTable(
                name: "FoodPortions");

            migrationBuilder.DropTable(
                name: "InputFoods");

            migrationBuilder.DropTable(
                name: "NutrientConversionFactors");

            migrationBuilder.DropTable(
                name: "FoodNutrientDerivations");

            migrationBuilder.DropTable(
                name: "Nutrients");

            migrationBuilder.DropTable(
                name: "MeasureUnits");

            migrationBuilder.DropTable(
                name: "InputFoods2");

            migrationBuilder.DropTable(
                name: "FoundationFoods");

            migrationBuilder.DropTable(
                name: "FoodNutrientSource");

            migrationBuilder.DropTable(
                name: "FoodCategories");

            migrationBuilder.DropTable(
                name: "RootFoundationFoods");
        }
    }
}
