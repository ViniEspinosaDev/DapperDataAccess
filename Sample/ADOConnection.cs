using Microsoft.Data.SqlClient;

namespace AcessoDadosDapper.Sample
{
    public class ADOConnection
    {
        public void GetAllCategories()
        {
            // Caminho de conexão com o servidor do banco de dados
            const string ConnectionString = "Server=localhost,1433;Database=Balta;User ID=sa;Password=1q2w3e4r@#$";

            // Instanciar conexão
            using (var connection = new SqlConnection(ConnectionString))
            {
                // Abrir conexão
                connection.Open();

                // Instanciar comando
                using (var command = new SqlCommand())
                {
                    // Atribuir conexão ao comando
                    command.Connection = connection;
                    // Atribuir tipo do comando
                    command.CommandType = System.Data.CommandType.Text;
                    // Atribuir consulta
                    command.CommandText = "SELECT [Id], [Title] FROM [Category]";

                    // Executar comando
                    var reader = command.ExecuteReader();

                    // Recuperar informações da consulta
                    while (reader.Read())
                    {
                        // Recuperar dado
                        var id = reader.GetGuid(0);
                        string title = reader.GetString(1);

                        System.Console.WriteLine($"{id} - {title}");
                    }
                }
            }
        }
    }
}