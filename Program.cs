//Написать программу которая будет принимать у пользователя название БД и создать ее. 
//После чего запрашивать у пользователя Название таблицы и создать ее с полями которые нужны пользователю. 
//После чего дать возможность ему наполнить ее) 

using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ConsoleApp93
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Введите название новой базы данных: ");
            string dbName = Console.ReadLine();

            Console.Write("Введите название таблицы: ");
            string tableName = Console.ReadLine();

            Console.Write("Введите количество полей: ");
            int numFields = int.Parse(Console.ReadLine());

            List<string> fields = new List<string>();

            for (int i = 0; i < numFields; i++)
            {
                Console.Write($"Введите название поля {i + 1}: ");
                string fieldName = Console.ReadLine();

                Console.Write($"Введите тип поля {fieldName} (например, INT, NVARCHAR(50), etc.): ");
                string fieldType = Console.ReadLine();

                fields.Add($"{fieldName} {fieldType}");
            }

            // Подключение к БД master
            string masterConnectionString = @"Data Source=DESKTOP-NBVARLP;Initial Catalog=master;Trusted_Connection=True;TrustServerCertificate=True";

            using (SqlConnection masterConnection = new SqlConnection(masterConnectionString))
            {
                await masterConnection.OpenAsync();
                Console.WriteLine("Подключение к master открыто");

                // Создание БД под именем пользователя
                SqlCommand createDbCommand = new SqlCommand();
                createDbCommand.CommandText = $"CREATE DATABASE {dbName}";
                createDbCommand.Connection = masterConnection;

                await createDbCommand.ExecuteNonQueryAsync();
                Console.WriteLine($"База данных {dbName} создана");

                string dbConnectionString = $@"Data Source=DESKTOP-NBVARLP;Initial Catalog={dbName};Trusted_Connection=True;TrustServerCertificate=True";

                using (SqlConnection dbConnection = new SqlConnection(dbConnectionString))
                {
                    await dbConnection.OpenAsync();
                    Console.WriteLine($"Подключение к базе данных {dbName} открыто");

                    // Создание таблицы с заданными полями
                    SqlCommand createTableCommand = new SqlCommand();
                    createTableCommand.CommandText = $"CREATE TABLE {tableName} ({string.Join(", ", fields)})";
                    createTableCommand.Connection = dbConnection;

                    await createTableCommand.ExecuteNonQueryAsync();
                    Console.WriteLine($"Таблица {tableName} создана");

                    // Наполнение таблицы данными
                    Console.WriteLine($"Введите данные для таблицы {tableName}:");
                    while (true)
                    {
                        List<string> values = new List<string>();
                        foreach (string field in fields)
                        {
                            Console.Write($"Введите значение для поля {field}: ");
                            string value = Console.ReadLine();
                            values.Add(value);
                        }

                        SqlCommand insertCommand = new SqlCommand();
                        insertCommand.CommandText = $"INSERT INTO {tableName} VALUES ('{string.Join("', '", values)}')";
                        insertCommand.Connection = dbConnection;

                        await insertCommand.ExecuteNonQueryAsync();
                        Console.WriteLine("Книга успешно добавлена в таблицу.");
                        Console.Write("Продолжить добавление данных? (y/n): ");
                        string continueAdding = Console.ReadLine();
                        if (continueAdding.ToLower() != "y")
                            break;
                    }

                    Console.WriteLine("Подключение к базе данных закрыто");
                }
            }

            Console.WriteLine("Подключение к master закрыто");
            Console.WriteLine("End Main");
            Console.Read();
        }
    }
}