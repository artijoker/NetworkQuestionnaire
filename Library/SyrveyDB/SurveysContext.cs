using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Library {
    public partial class SurveysContext : DbContext {
        public SurveysContext() {
        }

        public SurveysContext(DbContextOptions<SurveysContext> options)
            : base(options) {
        }

        public DbSet<FreeAnswer> FreeAnswers { get; set; }
        public DbSet<EmployeeFreeAnswer> EmployeesFreeAnswers { get; set; }
        public DbSet<MultipleAnswer> MultipleAnswers { get; set; }
        public DbSet<SingleAnswer> SingleAnswers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlServer("Server=localhost;Database=Surveys;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<MultipleAnswer>(answer => {
                answer.ToTable("MultipleAnswers");

                answer.Property(a => a.Text)
                    .IsRequired()
                    .HasMaxLength(150);

                answer.Property(a => a.QuestionId)
                   .IsRequired();
            });

            modelBuilder.Entity<FreeAnswer>(answer => {
                answer.ToTable("FreeAnswers");

                answer.Property(a => a.QuestionId)
                   .IsRequired();
            });

            modelBuilder.Entity<SingleAnswer>(answer => {
                answer.ToTable("SingleAnswers");

                answer.Property(a => a.Text)
                    .IsRequired()
                    .HasMaxLength(150);

                answer.Property(a => a.QuestionId)
                   .IsRequired();

            });

            modelBuilder.Entity<QuestionType>(type => {
                type.Property(t => t.Type)
                    .IsRequired()
                    .HasMaxLength(20);

                type.HasMany(t => t.Questions)
                    .WithOne(q => q.Type)
                    .HasForeignKey(q => q.QuestionTypeId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Question>(question => {
                question.Property(q => q.Text)
                    .IsRequired()
                    .HasMaxLength(150);

                question.Property(q => q.QuestionTypeId)
                   .IsRequired();


                question.Property(q => q.SurveyId)
                    .IsRequired();

                question.Property(q => q.IsRequired)
                    .IsRequired();

                question.HasMany(q => q.MultipleAnswers)
                    .WithOne(a => a.Question)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);

                question.HasMany(q => q.FreeAnswers)
                    .WithOne(a => a.Question)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);

                question.HasMany(q => q.SingleAnswers)
                    .WithOne(a => a.Question)
                    .HasForeignKey(a => a.QuestionId)
                    .OnDelete(DeleteBehavior.Cascade);


            });

            modelBuilder.Entity<Survey>(survey => {
                survey.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                survey.HasMany(s => s.Questions)
                    .WithOne(q => q.Survey)
                    .HasForeignKey(q => q.SurveyId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Employee>(employee => {

                employee.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50);

                employee.HasIndex(e => e.Login)
                    .IsUnique();

                employee.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                employee.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                employee.HasIndex(e => e.Email)
                    .IsUnique();

                employee.Property(e => e.BirthDate)
                    .IsRequired();

                employee.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                employee.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50);

                employee.Property(e => e.Patronymic)
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<EmployeeFreeAnswer>(employeeFreeAnswer => {
                employeeFreeAnswer.Property(e => e.FreeAnswerId)
                    .IsRequired();

                employeeFreeAnswer.Property(e => e.EmployeeId)
                    .IsRequired();

                employeeFreeAnswer.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(150);

                employeeFreeAnswer.HasOne(e => e.FreeAnswer)
                   .WithMany(a => a.EmployeeFreeAnswers)
                   .HasForeignKey(a => a.FreeAnswerId)
                   .OnDelete(DeleteBehavior.Cascade);

                employeeFreeAnswer.HasOne(e => e.Employee)
                    .WithMany(e => e.EmployeeFreeAnswers)
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);


                employeeFreeAnswer.HasKey(c => new { c.EmployeeId, c.FreeAnswerId });


            });

            modelBuilder.Entity<Employee>()
                .HasMany(employee => employee.Surveys)
                .WithMany(survey => survey.Employees)
                .UsingEntity<Dictionary<string, object>>(
                "EmployeesSurveys",
                table => table.HasOne<Survey>().WithMany().HasForeignKey("SurveyId"),
                table => table.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId")
                );

            modelBuilder.Entity<Employee>()
                .HasMany(employee => employee.SingleAnswers)
                .WithMany(answer => answer.Employees)
                .UsingEntity<Dictionary<string, object>>(
                "EmployeesSingleAnswers",
                table => table.HasOne<SingleAnswer>().WithMany().HasForeignKey("SingleAnswerId"),
                table => table.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId")
                );

            modelBuilder.Entity<Employee>()
                .HasMany(employee => employee.MultipleAnswers)
                .WithMany(answer => answer.Employees)
                .UsingEntity<Dictionary<string, object>>(
                "EmployeesMultipleAnswers",
                table => table.HasOne<MultipleAnswer>().WithMany().HasForeignKey("MultipleAnswerId"),
                table => table.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId")
                );

            FillWithData(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        void FillWithData(ModelBuilder modelBuilder) {
            modelBuilder.Entity<QuestionType>().HasData(
                new[] {
                    new QuestionType() {Id = 1, Type = "Single"},
                    new QuestionType() {Id = 2, Type = "Multiple"},
                    new QuestionType() {Id = 3, Type = "Free"}
                });

            modelBuilder.Entity<Survey>().HasData(new[] {
                new Survey() {
                    Id = 1,
                    Name = "Тест на вовлеченность персонала (Gallup Q12)"
                },
                new Survey() {
                    Id = 2,
                    Name = "Карьерные ожидания сотрудников"
                },
                new Survey() {
                    Id = 3,
                    Name = "Круговая оценка работы персонала (Метод «360 градусов»)"
                }
            });

            modelBuilder.Entity<Question>().HasData(
                new[] {
                    new Question() {
                        Id = 1,
                        SurveyId = 1,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Знаете ли вы, чего ожидает от вас руководство?"
                    },
                    new Question() {
                        Id = 2,
                        SurveyId = 1,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Есть ли у вас необходимые инструменты и материалы для качественного выполнения своей работы?"
                    },
                    new Question() {
                        Id = 3,
                        SurveyId = 1,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Есть ли у вас возможность ежедневно делать на работе то, что вы делаете лучше всего?"
                    },
                    new Question() {
                        Id = 4,
                        SurveyId = 1,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Получали ли вы за последние семь дней одобрение или похвалу за хорошо выполненную работу?"
                    },
                    new Question() {
                        Id = 5,
                        SurveyId = 1,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Принимается ли во внимание ваша точка зрения?"
                    },
                    new Question() {
                        Id = 6,
                        SurveyId = 1,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Считают ли коллеги своим долгом качественное выполнение работы?"
                    },
                    new Question() {
                        Id = 7,
                        SurveyId = 1,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "За последние полгода кто-нибудь на работе говорил с вами о ваших профессиональных успехах и достижениях?"
                    },
                    new Question() {
                        Id = 8,
                        SurveyId = 1,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "За последний год были ли у вас на работе возможности для приобретения новых знаний и профессионального роста?"
                    },



                    new Question() {
                        Id = 9,
                        SurveyId = 2,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Вы работаете в Компании: "
                    },
                    new Question() {
                        Id = 10,
                        SurveyId = 2,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Насколько Вы удовлетворены работой в Компании в целом? "
                    },
                    new Question() {
                        Id = 11,
                        SurveyId = 2,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Насколько важен для Вас карьерный рост? "
                    },
                    new Question() {
                        Id = 12,
                        SurveyId = 2,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Ожидаете ли Вы повышения? Если да, то как скоро? "
                    },
                    new Question() {
                        Id = 13,
                        SurveyId = 2,
                        QuestionTypeId = 2,
                        IsRequired = true,
                        Text = "Каких изменений Вы ожидаете вместе с повышением? "
                    },
                    new Question() {
                        Id = 14,
                        SurveyId = 2,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Существуют ли какие-либо препятствия для Вашего карьерного роста?"
                    },
                    new Question() {
                        Id = 15,
                        SurveyId = 2,
                        QuestionTypeId = 2,
                        IsRequired = true,
                        Text = "Уточните: "
                    },


                    new Question() {
                        Id = 16,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Как часто Ваш руководитель доступен для подчинённых?"
                    },
                    new Question() {
                        Id = 17,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Как часто Ваш руководитель даёт оценку Вашей работе?"
                    },
                    new Question() {
                        Id = 18,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Насколько эффективными являются тренинги Вашего руководителя?"
                    },
                    new Question() {
                        Id = 19,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Как часто Ваш коллега опаздывает на работу?"
                    },
                    new Question() {
                        Id = 20,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Насколько трудоспособен Ваш коллега?"
                    },
                    new Question() {
                        Id = 21,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Как Вы оцениваете работу Вашего коллеги с клиентами?"
                    },
                    new Question() {
                        Id = 22,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Насколько эффективен Ваш подчинённый в работе?"
                    },
                    new Question() {
                        Id = 23,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Насколько коммуникабелен Ваш подчинённый со своими коллегами?"
                    },
                    new Question() {
                        Id = 24,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Как часто Ваш подчинённый сдаёт работу в 'последнюю минуту'?"
                    },
                    new Question() {
                        Id = 25,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = false,
                        Text = "Считаете ли вы что рабочая атмосфера в отделе 'токсичная'?"
                    },
                    new Question() {
                        Id = 26,
                        SurveyId = 3,
                        QuestionTypeId = 1,
                        IsRequired = true,
                        Text = "Оцените эффективность работы Вашего отдела?"
                    },
                    new Question() {
                        Id = 27,
                        SurveyId = 3,
                        QuestionTypeId = 3,
                        IsRequired = false,
                        Text = "Вы можете внести предложения по улучшению работы Вашего отдела:"
                    }

                    
                });
            modelBuilder.Entity<SingleAnswer>().HasData(
                new[] {
                    new SingleAnswer() {Id = 1, QuestionId = 1, Text = "Да" },
                    new SingleAnswer() {Id = 2, QuestionId = 1, Text = "Нет" },

                    new SingleAnswer() {Id = 3, QuestionId = 2, Text = "Да" },
                    new SingleAnswer() {Id = 4, QuestionId = 2, Text = "Нет" },

                    new SingleAnswer() {Id = 5, QuestionId = 3, Text = "Да" },
                    new SingleAnswer() {Id = 6, QuestionId = 3, Text = "Нет" },

                    new SingleAnswer() {Id = 7, QuestionId = 4, Text = "Да" },
                    new SingleAnswer() {Id = 8, QuestionId = 4, Text = "Нет" },

                    new SingleAnswer() {Id = 9, QuestionId = 5, Text = "Да" },
                    new SingleAnswer() {Id = 10, QuestionId = 5, Text = "Нет" },

                    new SingleAnswer() {Id = 11, QuestionId = 6, Text = "Да" },
                    new SingleAnswer() {Id = 12, QuestionId = 6, Text = "Нет" },

                    new SingleAnswer() {Id = 13, QuestionId = 7, Text = "Да" },
                    new SingleAnswer() {Id = 14, QuestionId = 7, Text = "Нет" },

                    new SingleAnswer() {Id = 15, QuestionId = 8, Text = "Да" },
                    new SingleAnswer() {Id = 16, QuestionId = 8, Text = "Нет" },




                    new SingleAnswer() {Id = 17, QuestionId = 9, Text = "Менее 6 месяцев" },
                    new SingleAnswer() {Id = 18, QuestionId = 9, Text = "6-12 месяцев" },
                    new SingleAnswer() {Id = 19, QuestionId = 9, Text = "1-3 года" },
                    new SingleAnswer() {Id = 20, QuestionId = 9, Text = "3-5 лет" },
                    new SingleAnswer() {Id = 21, QuestionId = 9, Text = "Более 5 лет" },

                    new SingleAnswer() {Id = 22, QuestionId = 10, Text = "Полностью удовлетворен(а)" },
                    new SingleAnswer() {Id = 23, QuestionId = 10, Text = "Скорее удовлетворен(а)" },
                    new SingleAnswer() {Id = 24, QuestionId = 10, Text = "Скорее не удовлетворен(а)" },
                    new SingleAnswer() {Id = 25, QuestionId = 10, Text = "Скорее не удовлетворен(а)" },

                    new SingleAnswer() {Id = 26, QuestionId = 11, Text = "Крайне важен" },
                    new SingleAnswer() {Id = 27, QuestionId = 11, Text = "Скорее важен" },
                    new SingleAnswer() {Id = 28, QuestionId = 11, Text = "Скорее не важен" },
                    new SingleAnswer() {Id = 29, QuestionId = 11, Text = "Совсем не важен" },

                    new SingleAnswer() {Id = 30, QuestionId = 12, Text = "Да, в течение ближайших месяцев" },
                    new SingleAnswer() {Id = 31, QuestionId = 12, Text = "Да, в течение года" },
                    new SingleAnswer() {Id = 32, QuestionId = 12, Text = "Да, в течение ближайших 3-5 лет" },
                    new SingleAnswer() {Id = 33, QuestionId = 12, Text = "Да, не раньше, чем через пять лет" },
                    new SingleAnswer() {Id = 34, QuestionId = 12, Text = "Нет, не ожидаю повышения" },

                    new SingleAnswer() {Id = 35, QuestionId = 14, Text = "Да" },
                    new SingleAnswer() {Id = 36, QuestionId = 14, Text = "Нет" },



                    new SingleAnswer() {Id = 37, QuestionId = 16, Text = "Всегда" },
                    new SingleAnswer() {Id = 38, QuestionId = 16, Text = "Часто" },
                    new SingleAnswer() {Id = 39, QuestionId = 16, Text = "Редко" },
                    new SingleAnswer() {Id = 40, QuestionId = 16, Text = "Никогда" },

                    new SingleAnswer() {Id = 41, QuestionId = 17, Text = "Каждый раз" },
                    new SingleAnswer() {Id = 42, QuestionId = 17, Text = "Часто" },
                    new SingleAnswer() {Id = 43, QuestionId = 17, Text = "Редко" },
                    new SingleAnswer() {Id = 44, QuestionId = 17, Text = "Иногда" },

                    new SingleAnswer() {Id = 45, QuestionId = 18, Text = "Очень эффективные" },
                    new SingleAnswer() {Id = 46, QuestionId = 18, Text = "Эффективные" },
                    new SingleAnswer() {Id = 47, QuestionId = 18, Text = "Малоэффективные" },
                    new SingleAnswer() {Id = 48, QuestionId = 18, Text = "Не эффективные" },

                    new SingleAnswer() {Id = 49, QuestionId = 19, Text = "Всегда" },
                    new SingleAnswer() {Id = 50, QuestionId = 19, Text = "Часто" },
                    new SingleAnswer() {Id = 51, QuestionId = 19, Text = "Иногда" },
                    new SingleAnswer() {Id = 52, QuestionId = 19, Text = "Никогда" },

                    new SingleAnswer() {Id = 53, QuestionId = 20, Text = "Очень трудоспособен" },
                    new SingleAnswer() {Id = 54, QuestionId = 20, Text = "Достаточно трудоспособен" },
                    new SingleAnswer() {Id = 55, QuestionId = 20, Text = "Удовлетворительно" },
                    new SingleAnswer() {Id = 56, QuestionId = 20, Text = "Не удовлетворительно" },

                    new SingleAnswer() {Id = 57, QuestionId = 21, Text = "Отлично" },
                    new SingleAnswer() {Id = 58, QuestionId = 21, Text = "Очень хорошо" },
                    new SingleAnswer() {Id = 59, QuestionId = 21, Text = "Хорошо" },
                    new SingleAnswer() {Id = 60, QuestionId = 21, Text = "Удовлетворительно" },
                    new SingleAnswer() {Id = 61, QuestionId = 21, Text = "Неудовлетворительно" },

                    new SingleAnswer() {Id = 62, QuestionId = 22, Text = "Очень эффективные" },
                    new SingleAnswer() {Id = 63, QuestionId = 22, Text = "Эффективные" },
                    new SingleAnswer() {Id = 64, QuestionId = 22, Text = "Малоэффективные" },
                    new SingleAnswer() {Id = 65, QuestionId = 22, Text = "Не эффективные" },

                    new SingleAnswer() {Id = 66, QuestionId = 23, Text = "Очень коммуникабелен" },
                    new SingleAnswer() {Id = 67, QuestionId = 23, Text = "В меру коммуникабелен" },
                    new SingleAnswer() {Id = 68, QuestionId = 23, Text = "Не достаточно коммуникабелен" },
                    new SingleAnswer() {Id = 69, QuestionId = 23, Text = "Скрытный" },

                    new SingleAnswer() {Id = 70, QuestionId = 24, Text = "Всегда" },
                    new SingleAnswer() {Id = 71, QuestionId = 24, Text = "Часто" },
                    new SingleAnswer() {Id = 72, QuestionId = 24, Text = "Иногда" },
                    new SingleAnswer() {Id = 73, QuestionId = 24, Text = "Никогда" },

                    new SingleAnswer() {Id = 74, QuestionId = 25, Text = "Да" },
                    new SingleAnswer() {Id = 75, QuestionId = 25, Text = "Нет" },

                    new SingleAnswer() {Id = 76, QuestionId = 26, Text = "5" },
                    new SingleAnswer() {Id = 77, QuestionId = 26, Text = "4" },
                    new SingleAnswer() {Id = 78, QuestionId = 26, Text = "3" },
                    new SingleAnswer() {Id = 79, QuestionId = 26, Text = "2" },
                    new SingleAnswer() {Id = 80, QuestionId = 26, Text = "1" },
                });

            modelBuilder.Entity<MultipleAnswer>().HasData(
                new[] {
                    new MultipleAnswer() { Id = 1, QuestionId = 13, Text = "Повышение заработной платы" },
                    new MultipleAnswer() { Id = 2, QuestionId = 13, Text = "Рост количества привилегий и льгот" },
                    new MultipleAnswer() { Id = 3, QuestionId = 13, Text = "Рост уважения среди колле" },
                    new MultipleAnswer() { Id = 4, QuestionId = 13, Text = "Рост степени ответственности за принимаемые решения" },
                    new MultipleAnswer() { Id = 5, QuestionId = 13, Text = "Большую свободу действий" },
                    new MultipleAnswer() { Id = 6, QuestionId = 13, Text = "Рост нагрузки, напряженности" },
                    new MultipleAnswer() { Id = 7, QuestionId = 13, Text = "Увеличение стрессовости" },
                    new MultipleAnswer() { Id = 8, QuestionId = 13, Text = "Другое" },

                    new MultipleAnswer() { Id = 9, QuestionId = 15, Text = "Личные обстоятельства" },
                    new MultipleAnswer() { Id = 10, QuestionId = 15, Text = "Отсутствие возможности повысить квалификацию, получить образование" },
                    new MultipleAnswer() { Id = 11, QuestionId = 15, Text = "Большое количество конкурентов на должность" },
                    new MultipleAnswer() { Id = 12, QuestionId = 15, Text = "Недоброжелательное отношение руководства" },
                    new MultipleAnswer() { Id = 13, QuestionId = 15, Text = "Недоброжелательное отношение коллег" },
                    new MultipleAnswer() { Id = 14, QuestionId = 15, Text = "Другое" },
                });

            modelBuilder.Entity<FreeAnswer>().HasData(new FreeAnswer() { Id = 1, QuestionId = 27 });

            modelBuilder.Entity<Employee>().HasData(
                new[] {
                    new Employee() {
                        Id = 1,
                        Surname = "Большаков",
                        Name = "Петр",
                        BirthDate = DateTime.Parse("01.07.1989"),
                        Email = "petr1989@yandex.ru",
                        Login = "login",
                        Password = "password"
                    },
                    new Employee() {
                        Id = 2,
                        Surname = "Аксенчук",
                        Name = "Семен",
                        Patronymic = "Никитович",
                        BirthDate = DateTime.Parse("22.04.1973"),
                        Email = "semen3401@hotmail.com",
                        Login = "semen3401",
                        Password = "8e71c26d7"
                        
                    },
                     new Employee() {
                        Id = 3,
                        Surname = "Яимов",
                        Name = "Милан",
                        BirthDate = DateTime.Parse("05.11.1984"),
                        Email = "milan1984@gmail.com",
                        Login = "milan1984",
                        Password = "7822cdf91"
                    },
                      new Employee() {
                        Id = 4,
                        Surname = "Куксилова",
                        Name = "Тамара",
                        Patronymic = "Алексеевна",
                        BirthDate = DateTime.Parse("18.03.1989"),
                        Email = "tamara18031986@gmail.com",
                        Login = "tamara18031989",
                        Password = "3f3af347b"
                    },
                });
        }



    }



}
