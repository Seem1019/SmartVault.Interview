using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartVault.Library;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace SmartVault.DataGeneration
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            

            SQLiteConnection.CreateFile(configuration["DatabaseFileName"]);
            var connectionString = string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", configuration?["DatabaseFileName"]);
            Console.WriteLine(connectionString);
            File.WriteAllText("TestDoc.txt", $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}");
            Stopwatch stopwatch = Stopwatch.StartNew();
            using (var connection = new SQLiteConnection(connectionString))
            {

                var files = Directory.GetFiles(@"..\..\..\..\BusinessObjectSchema");
                for (int i = 0; i <= 2; i++)
                {
                    var serializer = new XmlSerializer(typeof(BusinessObject));
                    using (var reader = new StreamReader(files[i]))
                    {
                        var businessObject = serializer.Deserialize(reader) as BusinessObject;
                        if (businessObject?.Script != null)
                        {
                            connection.Execute(businessObject.Script);
                        }
                    }
                }
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    connection.Query("ALTER TABLE User ADD COLUMN CreatedOn DATETIME DEFAULT CURRENT_TIMESTAMP;");
                    connection.Query("ALTER TABLE Account ADD COLUMN CreatedOn DATETIME DEFAULT CURRENT_TIMESTAMP;");
                    connection.Query("ALTER TABLE Document ADD COLUMN CreatedOn DATETIME DEFAULT CURRENT_TIMESTAMP;");
                    // Listas para acumular las sentencias INSERT
                    var insertUserStatements = new List<string>();
                    var insertAccountStatements = new List<string>();
                    var insertDocumentStatements = new List<string>();

                    // Configuración inicial
                    int batchSize = 10; // Tamaño del lote para ejecutar inserciones
                    int documentNumber = 0;

                    for (int i = 0; i < 100; i++)
                    {

                        // Generar datos aleatorios para el día
                        var randomDayIterator = RandomDay().GetEnumerator();
                        randomDayIterator.MoveNext();


                        // Agregar la sentencia INSERT a la lista para la tabla User
                        insertUserStatements.Add($"('{i}', 'FName{i}', 'LName{i}', '{randomDayIterator.Current.ToString("yyyy-MM-dd")}', '{i}', 'UserName{i}', 'password-hash')");

                        // Agregar la sentencia INSERT para la tabla Account
                        insertAccountStatements.Add($"('{i}', 'Account{i}')");

                        for (int d = 0; d < 10000; d++, documentNumber++)
                        {
                            // Suponiendo que TestDoc.txt está en la misma ubicación que el ejecutable
                            var documentPath = new FileInfo("TestDoc.txt").FullName;
                            insertDocumentStatements.Add($"('{documentNumber}', 'Document{i}-{d}.txt', '{documentPath}', '{new FileInfo(documentPath).Length}', '{i}')");

                            // Ejecutar cada 1000 documentos para evitar una lista demasiado grande
                            if (insertDocumentStatements.Count >= 1000)
                            {
                                var documentCommand = connection.CreateCommand();
                                documentCommand.Transaction = transaction;
                                documentCommand.CommandText = $"INSERT INTO Document (Id, Name, FilePath, Length, AccountId) VALUES {string.Join(",", insertDocumentStatements)}";
                                documentCommand.ExecuteNonQuery();
                                insertDocumentStatements.Clear();
                            }
                        }

                        // Ejecutar el lote de inserciones para User y Account cada 'batchSize' veces
                        if (i % batchSize == 0 || i == 99)
                        {
                            // Ejecutar INSERT para User
                            if (insertUserStatements.Any())
                            {
                                var userCommand = connection.CreateCommand();
                                userCommand.Transaction = transaction;
                                userCommand.CommandText = $"INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password) VALUES {string.Join(",", insertUserStatements)}";
                                userCommand.ExecuteNonQuery();
                                insertUserStatements.Clear();
                            }

                            // Ejecutar INSERT para Account
                            if (insertAccountStatements.Any())
                            {
                                var accountCommand = connection.CreateCommand();
                                accountCommand.Transaction = transaction;
                                accountCommand.CommandText = $"INSERT INTO Account (Id, Name) VALUES {string.Join(",", insertAccountStatements)}";
                                accountCommand.ExecuteNonQuery();
                                insertAccountStatements.Clear();
                            }
                        }
                    }

                    // Asegurarse de ejecutar cualquier sentencia restante para Document
                    if (insertDocumentStatements.Any())
                    {
                        var documentCommand = connection.CreateCommand();
                        documentCommand.Transaction = transaction;
                        documentCommand.CommandText = $"INSERT INTO Document (Id, Name, FilePath, Length, AccountId) VALUES {string.Join(",", insertDocumentStatements)}";
                        documentCommand.ExecuteNonQuery();
                        insertDocumentStatements.Clear();
                    }

                    // Confirmar la transacción al final
                    transaction.Commit();
                }
                stopwatch.Stop();
                Console.WriteLine($"Tiempo de ejecución: {stopwatch.ElapsedMilliseconds} milisegundos");
                Console.WriteLine($"Tiempo de ejecución: {stopwatch.Elapsed.TotalSeconds} segundos");
                var accountData = connection.Query("SELECT id FROM Account LIMIT 1;");
                Console.WriteLine($"AccountCount: {JsonConvert.SerializeObject(accountData)}");
                var documentData = connection.Query("SELECT COUNT(*) FROM Document;");
                Console.WriteLine($"DocumentCount: {JsonConvert.SerializeObject(documentData)}");
                var userData = connection.Query("SELECT COUNT(*) FROM User;");
                Console.WriteLine($"UserCount: {JsonConvert.SerializeObject(userData)}");
            }
        }

        static IEnumerable<DateTime> RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }
    }
}