using System;
using System.Collections.Generic;
using System.Linq;
using AcessoDadosDapper.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AcessoDadosDapper.Sample
{
    public static class DapperCommands
    {
        // INSERT
        public static int InsertCategory(SqlConnection connection, Category category)
        {
            // Criando insert com os parametros para o Dapper mapear
            var insertCategory = @"INSERT INTO [Category] VALUES (@Id, @Title, @Url, @Summary, @Order, @Description, @Featured)";

            /* Fazer inserção utilizando Dapper. Esse new é um objeto que irá receber 
                o conteúdo dos parametros do insert criado acima
                OBS: Se criar os parametros com os mesmos nomes dos atributos da Model não é necessário
                     atribuir com =. Por exemplo. Id = category.Id, Title = category.Title...
            */
            int affectedRows = connection.Execute(insertCategory, new
            {
                category.Id,
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });

            return affectedRows;
        }

        // INSERT MANY
        public static int InsertManyCategories(SqlConnection connection, List<Category> categories)
        {
            var insertCategory = $@"
                INSERT INTO [Category] VALUES 
                (
                    @{nameof(Category.Id)}, 
                    @{nameof(Category.Title)}, 
                    @{nameof(Category.Url)}, 
                    @{nameof(Category.Summary)}, 
                    @{nameof(Category.Order)}, 
                    @{nameof(Category.Description)}, 
                    @{nameof(Category.Featured)}
                )";

            int affectedRows = connection.Execute(insertCategory, categories.ToArray());

            return affectedRows;
        }

        // SELECT
        public static void ListCategories(SqlConnection connection)
        {
            var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");

            foreach (var category in categories)
            {
                System.Console.WriteLine($"{category.Id} - {category.Title}");
            }
        }

        // UPDATE
        public static int UpdateCategory(SqlConnection connection, Category category)
        {
            string updateQuery = "UPDATE [Category] SET [Title] = @Title WHERE [Id] = @Id";

            int affectedRows = connection.Execute(updateQuery, new
            {
                category.Title,
                category.Id
            });

            return affectedRows;
        }

        // DELETE
        public static int DeleteCategory(SqlConnection connection, Category category)
        {
            string deleteCategory = "DELETE FROM [Category] WHERE [Id] = @Id";

            int affectedRows = connection.Execute(deleteCategory, new { category.Id });

            return affectedRows;
        }

        // PROCEDURE (Sem retorno)
        public static int ExecuteDeleteStudentProcedure(SqlConnection connection, Guid StudentId)
        {
            string procedure = "[spDeleteStudent]";
            var parameters = new { StudentId };

            // Quando passa o command type = stored procedure não precisa passar EXEC nem o parametro na query
            int affectedRows = connection.Execute(
                sql: procedure,
                param: parameters,
                commandType: System.Data.CommandType.StoredProcedure);

            return affectedRows;
        }

        // PROCEDURE (Com retorno)
        public static IEnumerable<Course> ExecuteGetCourseByCategoryProcedure(SqlConnection connection, Guid categoryId)
        {
            string procedure = "[spGetCoursesByCategory]";
            var parameters = new { categoryId };

            // Quando passa o command type = stored procedure não precisa passar EXEC nem o parametro na query
            var courses = connection.Query<Course>(
                sql: procedure,
                param: parameters,
                commandType: System.Data.CommandType.StoredProcedure);
            return courses;
        }

        // INSERT com retorno do ID que foi gerado
        public static Guid ExecuteScalarInsertCategory(SqlConnection connection, Category category)
        {
            var insertCategory = @"
                                    INSERT INTO 
                                        [Category]
                                    VALUES 
                                    (
                                        NEWID(), 
                                        @Title, 
                                        @Url, 
                                        @Summary, 
                                        @Order, 
                                        @Description, 
                                        @Featured
                                    )";

            var id = connection.ExecuteScalar<Guid>(insertCategory, new
            {
                category.Title,
                category.Url,
                category.Summary,
                category.Order,
                category.Description,
                category.Featured
            });

            return id;
        }

        // VIEW
        public static void ReadViewGetAllCourses(SqlConnection connection)
        {
            var courses = connection.Query("SELECT [Course ID] AS [Id], [Course Title] AS [Title] FROM [vwCourses]");

            foreach (var item in courses)
            {
                System.Console.WriteLine($"{item.Id} - {item.Title}");
            }
        }

        // ONE TO ONE (Pegar os careerItems com o curso dentro)
        public static void OneToOneCareerItemInnerJoinCourse(SqlConnection connection)
        {
            var query = @"
                            SELECT
                                *
                            FROM
                                [CareerItem]
                            INNER JOIN
                                [Course] ON [CareerItem].[CourseId] = [Course].[Id]
                        ";

            var items = connection.Query<CareerItem, Course, CareerItem>(
                query,
                (careerItem, course) =>
                {
                    careerItem.Course = course;
                    return careerItem;
                }, splitOn: "Id");

            foreach (var item in items)
            {
                System.Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
            }
        }

        // ONE TO MANY (Pegar as Career com todos seus CareerItems dentro)
        public static IEnumerable<Career> OneToManyGetCareersAndYoursCareerItems(SqlConnection connection)
        {
            string query = @"
                            SELECT 
                                [Career].[Id],
                                [Career].[Title],
                                [CareerItem].[CareerId],
                                [CareerItem].[Title]
                            FROM
                                [Career]
                            INNER JOIN
                                [CareerItem] ON [CareerItem].[CareerId] = [career].[Id]
                            ORDER BY
                                [Career].[Title]
                            ";

            var careers = new List<Career>();
            // <Career, CareerItem, Career> É objeto pai, objeto filho e resultado final que é o pai
            var items = connection.Query<Career, CareerItem, Career>(query,
            (career, item) =>
            {
                // Verifica se career já existe
                var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
                // Se não existir tem que adicionar pela primeira vez
                if (car == null)
                {
                    car = career;
                    // Adiciona o item na career
                    car.Items.Add(item);
                    // Adiciona career na lista
                    careers.Add(car);
                }
                else
                {
                    // Se não, é pq já existe o car e achou, então só adicionar o item à ela
                    car.Items.Add(item);
                }

                return career;
            }, splitOn: "CareerId");

            return careers;
        }

        // MANY TO MANY
        public static void ManyToMany(SqlConnection connection)
        {
            string query = "SELECT * FROM [Category]; SELECT * FROM [Course]";
        }
    }
}