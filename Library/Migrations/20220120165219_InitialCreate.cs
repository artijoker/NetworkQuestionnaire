using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Library.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Patronymic = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesSurveys",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SurveyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesSurveys", x => new { x.EmployeeId, x.SurveyId });
                    table.ForeignKey(
                        name: "FK_EmployeesSurveys_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeesSurveys_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    QuestionTypeId = table.Column<int>(type: "int", nullable: false),
                    SurveyId = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_QuestionTypes_QuestionTypeId",
                        column: x => x.QuestionTypeId,
                        principalTable: "QuestionTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Questions_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FreeAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreeAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FreeAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MultipleAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultipleAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MultipleAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SingleAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SingleAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesFreeAnswers",
                columns: table => new
                {
                    FreeAnswerId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesFreeAnswers", x => new { x.EmployeeId, x.FreeAnswerId });
                    table.ForeignKey(
                        name: "FK_EmployeesFreeAnswers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeesFreeAnswers_FreeAnswers_FreeAnswerId",
                        column: x => x.FreeAnswerId,
                        principalTable: "FreeAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesMultipleAnswers",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    MultipleAnswerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesMultipleAnswers", x => new { x.EmployeeId, x.MultipleAnswerId });
                    table.ForeignKey(
                        name: "FK_EmployeesMultipleAnswers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeesMultipleAnswers_MultipleAnswers_MultipleAnswerId",
                        column: x => x.MultipleAnswerId,
                        principalTable: "MultipleAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesSingleAnswers",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SingleAnswerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesSingleAnswers", x => new { x.EmployeeId, x.SingleAnswerId });
                    table.ForeignKey(
                        name: "FK_EmployeesSingleAnswers_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeesSingleAnswers_SingleAnswers_SingleAnswerId",
                        column: x => x.SingleAnswerId,
                        principalTable: "SingleAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "BirthDate", "Email", "Login", "Name", "Password", "Patronymic", "Surname" },
                values: new object[,]
                {
                    { 1, new DateTime(1989, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "petr1989@yandex.ru", "login", "Петр", "password", null, "Большаков" },
                    { 2, new DateTime(1973, 4, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "semen3401@hotmail.com", "semen3401", "Семен", "8e71c26d7", "Никитович", "Аксенчук" },
                    { 3, new DateTime(1984, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "milan1984@gmail.com", "milan1984", "Милан", "7822cdf91", null, "Яимов" },
                    { 4, new DateTime(1989, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "tamara18031986@gmail.com", "tamara18031989", "Тамара", "3f3af347b", "Алексеевна", "Куксилова" }
                });

            migrationBuilder.InsertData(
                table: "QuestionTypes",
                columns: new[] { "Id", "Type" },
                values: new object[,]
                {
                    { 1, "Single" },
                    { 2, "Multiple" },
                    { 3, "Free" }
                });

            migrationBuilder.InsertData(
                table: "Surveys",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Тест на вовлеченность персонала (Gallup Q12)" },
                    { 2, "Карьерные ожидания сотрудников" },
                    { 3, "Круговая оценка работы персонала (Метод «360 градусов»)" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "IsRequired", "QuestionTypeId", "SurveyId", "Text" },
                values: new object[,]
                {
                    { 1, true, 1, 1, "Знаете ли вы, чего ожидает от вас руководство?" },
                    { 25, false, 1, 3, "Считаете ли вы что рабочая атмосфера в отделе 'токсичная'?" },
                    { 24, true, 1, 3, "Как часто Ваш подчинённый сдаёт работу в 'последнюю минуту'?" },
                    { 23, true, 1, 3, "Насколько коммуникабелен Ваш подчинённый со своими коллегами?" },
                    { 22, true, 1, 3, "Насколько эффективен Ваш подчинённый в работе?" },
                    { 21, true, 1, 3, "Как Вы оцениваете работу Вашего коллеги с клиентами?" },
                    { 20, true, 1, 3, "Насколько трудоспособен Ваш коллега?" },
                    { 19, true, 1, 3, "Как часто Ваш коллега опаздывает на работу?" },
                    { 18, true, 1, 3, "Насколько эффективными являются тренинги Вашего руководителя?" },
                    { 17, true, 1, 3, "Как часто Ваш руководитель даёт оценку Вашей работе?" },
                    { 16, true, 1, 3, "Как часто Ваш руководитель доступен для подчинённых?" },
                    { 15, true, 2, 2, "Уточните: " },
                    { 26, true, 1, 3, "Оцените эффективность работы Вашего отдела?" },
                    { 14, true, 1, 2, "Существуют ли какие-либо препятствия для Вашего карьерного роста?" },
                    { 12, true, 1, 2, "Ожидаете ли Вы повышения? Если да, то как скоро? " },
                    { 11, true, 1, 2, "Насколько важен для Вас карьерный рост? " },
                    { 10, true, 1, 2, "Насколько Вы удовлетворены работой в Компании в целом? " },
                    { 9, true, 1, 2, "Вы работаете в Компании: " },
                    { 8, true, 1, 1, "За последний год были ли у вас на работе возможности для приобретения новых знаний и профессионального роста?" },
                    { 7, true, 1, 1, "За последние полгода кто-нибудь на работе говорил с вами о ваших профессиональных успехах и достижениях?" },
                    { 6, true, 1, 1, "Считают ли коллеги своим долгом качественное выполнение работы?" },
                    { 5, true, 1, 1, "Принимается ли во внимание ваша точка зрения?" },
                    { 4, true, 1, 1, "Получали ли вы за последние семь дней одобрение или похвалу за хорошо выполненную работу?" },
                    { 3, true, 1, 1, "Есть ли у вас возможность ежедневно делать на работе то, что вы делаете лучше всего?" },
                    { 2, true, 1, 1, "Есть ли у вас необходимые инструменты и материалы для качественного выполнения своей работы?" },
                    { 13, true, 2, 2, "Каких изменений Вы ожидаете вместе с повышением? " },
                    { 27, false, 3, 3, "Вы можете внести предложения по улучшению работы Вашего отдела:" }
                });

            migrationBuilder.InsertData(
                table: "FreeAnswers",
                columns: new[] { "Id", "QuestionId" },
                values: new object[] { 1, 27 });

            migrationBuilder.InsertData(
                table: "MultipleAnswers",
                columns: new[] { "Id", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 14, 15, "Другое" },
                    { 13, 15, "Недоброжелательное отношение коллег" },
                    { 11, 15, "Большое количество конкурентов на должность" },
                    { 10, 15, "Отсутствие возможности повысить квалификацию, получить образование" },
                    { 9, 15, "Личные обстоятельства" },
                    { 8, 13, "Другое" },
                    { 7, 13, "Увеличение стрессовости" },
                    { 6, 13, "Рост нагрузки, напряженности" },
                    { 5, 13, "Большую свободу действий" },
                    { 4, 13, "Рост степени ответственности за принимаемые решения" },
                    { 3, 13, "Рост уважения среди колле" },
                    { 2, 13, "Рост количества привилегий и льгот" },
                    { 1, 13, "Повышение заработной платы" },
                    { 12, 15, "Недоброжелательное отношение руководства" }
                });

            migrationBuilder.InsertData(
                table: "SingleAnswers",
                columns: new[] { "Id", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 16, 8, "Нет" },
                    { 59, 21, "Хорошо" },
                    { 58, 21, "Очень хорошо" },
                    { 57, 21, "Отлично" },
                    { 56, 20, "Не удовлетворительно" },
                    { 55, 20, "Удовлетворительно" },
                    { 54, 20, "Достаточно трудоспособен" },
                    { 53, 20, "Очень трудоспособен" },
                    { 60, 21, "Удовлетворительно" },
                    { 52, 19, "Никогда" },
                    { 50, 19, "Часто" },
                    { 49, 19, "Всегда" },
                    { 48, 18, "Не эффективные" },
                    { 47, 18, "Малоэффективные" },
                    { 46, 18, "Эффективные" },
                    { 45, 18, "Очень эффективные" },
                    { 44, 17, "Иногда" },
                    { 51, 19, "Иногда" },
                    { 43, 17, "Редко" },
                    { 61, 21, "Неудовлетворительно" },
                    { 63, 22, "Эффективные" },
                    { 79, 26, "2" },
                    { 78, 26, "3" },
                    { 77, 26, "4" },
                    { 76, 26, "5" },
                    { 75, 25, "Нет" },
                    { 74, 25, "Да" }
                });

            migrationBuilder.InsertData(
                table: "SingleAnswers",
                columns: new[] { "Id", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 73, 24, "Никогда" },
                    { 62, 22, "Очень эффективные" },
                    { 72, 24, "Иногда" },
                    { 70, 24, "Всегда" },
                    { 69, 23, "Скрытный" },
                    { 68, 23, "Не достаточно коммуникабелен" },
                    { 67, 23, "В меру коммуникабелен" },
                    { 66, 23, "Очень коммуникабелен" },
                    { 65, 22, "Не эффективные" },
                    { 64, 22, "Малоэффективные" },
                    { 71, 24, "Часто" },
                    { 42, 17, "Часто" },
                    { 41, 17, "Каждый раз" },
                    { 40, 16, "Никогда" },
                    { 32, 12, "Да, в течение ближайших 3-5 лет" },
                    { 31, 12, "Да, в течение года" },
                    { 30, 12, "Да, в течение ближайших месяцев" },
                    { 29, 11, "Совсем не важен" },
                    { 28, 11, "Скорее не важен" },
                    { 27, 11, "Скорее важен" },
                    { 26, 11, "Крайне важен" },
                    { 33, 12, "Да, не раньше, чем через пять лет" },
                    { 25, 10, "Скорее не удовлетворен(а)" },
                    { 23, 10, "Скорее удовлетворен(а)" },
                    { 22, 10, "Полностью удовлетворен(а)" },
                    { 21, 9, "Более 5 лет" },
                    { 20, 9, "3-5 лет" },
                    { 19, 9, "1-3 года" },
                    { 18, 9, "6-12 месяцев" },
                    { 17, 9, "Менее 6 месяцев" },
                    { 24, 10, "Скорее не удовлетворен(а)" },
                    { 34, 12, "Нет, не ожидаю повышения" },
                    { 14, 7, "Нет" },
                    { 13, 7, "Да" },
                    { 39, 16, "Редко" },
                    { 38, 16, "Часто" },
                    { 37, 16, "Всегда" },
                    { 2, 1, "Нет" },
                    { 3, 2, "Да" },
                    { 80, 26, "1" },
                    { 4, 2, "Нет" },
                    { 5, 3, "Да" }
                });

            migrationBuilder.InsertData(
                table: "SingleAnswers",
                columns: new[] { "Id", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 6, 3, "Нет" },
                    { 36, 14, "Нет" },
                    { 35, 14, "Да" },
                    { 7, 4, "Да" },
                    { 8, 4, "Нет" },
                    { 9, 5, "Да" },
                    { 10, 5, "Нет" },
                    { 11, 6, "Да" },
                    { 12, 6, "Нет" },
                    { 15, 8, "Да" },
                    { 1, 1, "Да" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Login",
                table: "Employees",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesFreeAnswers_FreeAnswerId",
                table: "EmployeesFreeAnswers",
                column: "FreeAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesMultipleAnswers_MultipleAnswerId",
                table: "EmployeesMultipleAnswers",
                column: "MultipleAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesSingleAnswers_SingleAnswerId",
                table: "EmployeesSingleAnswers",
                column: "SingleAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesSurveys_SurveyId",
                table: "EmployeesSurveys",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_FreeAnswers_QuestionId",
                table: "FreeAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_MultipleAnswers_QuestionId",
                table: "MultipleAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionTypeId",
                table: "Questions",
                column: "QuestionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SurveyId",
                table: "Questions",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleAnswers_QuestionId",
                table: "SingleAnswers",
                column: "QuestionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeesFreeAnswers");

            migrationBuilder.DropTable(
                name: "EmployeesMultipleAnswers");

            migrationBuilder.DropTable(
                name: "EmployeesSingleAnswers");

            migrationBuilder.DropTable(
                name: "EmployeesSurveys");

            migrationBuilder.DropTable(
                name: "FreeAnswers");

            migrationBuilder.DropTable(
                name: "MultipleAnswers");

            migrationBuilder.DropTable(
                name: "SingleAnswers");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "QuestionTypes");

            migrationBuilder.DropTable(
                name: "Surveys");
        }
    }
}
