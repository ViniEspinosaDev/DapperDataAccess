using System;
using System.Collections.Generic;
using System.Linq;
using AcessoDadosDapper.Models;
using AcessoDadosDapper.Sample;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AcessoDadosDapper
{
    class Program
    {
        static void Main(string[] args)
        {
            const string ConnectionString = "Server=localhost,1433;Database=Balta;User ID=sa;Password=1q2w3e4r@#$";

            var categories = CreateCategoryModel();

            var category = categories.FirstOrDefault();
            category.Id = new Guid("44de1eea-53e4-453d-9dc5-776accea7e8e");
            category.Title = "Netflix";

            var categoryId = new Guid("09ce0b7b-cfca-497b-92c0-3290ad9d5142"); // 'Backend'

            using (var connection = new SqlConnection(ConnectionString))
            {
                //DapperCommands.ListCategories(connection);
                //int affectedRows = DapperCommands.InsertNewCategory(connection, category);
                //int affectedRows = DapperCommands.InsertManyCategories(connection, categories);
                //int affectedRows = DapperCommands.UpdateCategory(connection, category);
                //int affectedRows = DapperCommands.DeleteCategory(connection, category);
                //category.Id = new Guid("3f8e162f-f613-43a7-a300-d36fd543cef8");
                //affectedRows = DapperCommands.DeleteCategory(connection, category);
                //int affectedRows = DapperCommands.ExecuteDeleteStudentProcedure(connection, new Guid("c4a6a1b1-df18-43df-a565-78d063b0042d"));
                //Console.WriteLine($"{affectedRows} linha(s) afetada(s).");
                //DapperCommands.ListCategories(connection);
                // var courses = DapperCommands.ExecuteGetCourseByCategoryProcedure(connection, categoryId);
                // foreach (var item in courses)
                // {
                //     System.Console.WriteLine($"{item.Id} - {item.Title}");
                // }
                //var guid = DapperCommands.ExecuteScalarInsertCategory(connection, category);
                //System.Console.WriteLine($"Nova item inserido: {guid}");
                //DapperCommands.ReadViewGetAllCourses(connection);
                //DapperCommands.OneToOneCareerItemInnerJoinCourse(connection);
                var careers = DapperCommands.OneToManyGetCareersAndYoursCareerItems(connection);
                foreach (var career in careers)
                {
                    System.Console.WriteLine($"Career: {career.Title}");
                    foreach (var item in career.Items)
                        System.Console.WriteLine($" - Item: {item.Title}");
                }
            }
        }

        static List<Category> CreateCategoryModel()
        {
            var categories = new List<Category>();

            var c1 = new Category()
            {
                Id = Guid.NewGuid(),
                Title = "Amazon AWS",
                Url = "amazon",
                Summary = "AWS Cloud",
                Order = 8,
                Description = "Categoria destinada a serviços do AWS",
                Featured = false
            };

            var c2 = new Category()
            {
                Id = Guid.NewGuid(),
                Title = "Facebook Ads",
                Url = "facebook-ads",
                Summary = "Facebook Ads",
                Order = 9,
                Description = "Categoria destinada a anúncios do Facebook",
                Featured = true
            };

            categories.Add(c1);
            categories.Add(c2);

            return categories;
        }
    }
}
