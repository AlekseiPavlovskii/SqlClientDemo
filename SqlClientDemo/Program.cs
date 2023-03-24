using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SqlClientDemo
{
    public class Program
    {
        private const string ConnectionString = 
            "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SqlClientDemo;Pooling=true;Integrated Security=SSPI";

        public static void Main(string[] args)
        {
            Phone newPhone = new Phone
            {
                Manufacturer = "Apple",
                Model = "IPhone",
                Price = 100500
            };

            // Созлаем и открываем подключение к sql серверу
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                
                // Создаем команду
                using (SqlCommand insertCommand = connection.CreateCommand())
                {
                    // Пишем в нее скрипт, который нужно выполнить
                    insertCommand.CommandText = 
                        @"insert into [Phone]
                              ([Model],
                              [Manufacturer],
                              [Price])
                          values
                              (@model,
                              @manufacturer,
                              @price)";
                    // И заполняем плейсхолдеры значениями
                    insertCommand.Parameters.Add("@model", SqlDbType.NVarChar).Value = newPhone.Model;
                    insertCommand.Parameters.Add("@manufacturer", SqlDbType.NVarChar).Value = newPhone.Manufacturer;
                    insertCommand.Parameters.Add("@price", SqlDbType.Int).Value = newPhone.Price;
                    // Запускаем команду
                    insertCommand.ExecuteScalar();
                }

                List<Phone> phones = new List<Phone>();
                // Создаем еще одну команду
                using (SqlCommand readCommand = connection.CreateCommand())
                {
                    // Пишем скрипт
                    readCommand.CommandText = "select [Id], [Model], [Manufacturer], [Price] from [Phone]";
                    // И выполняем ее в режиме чтения
                    using (var reader = readCommand.ExecuteReader())
                    {
                        // Пока считаны не все полученные сущности
                        while (reader.Read())
                        {
                            // Добавляем очередную сущности в массив результатов
                            phones.Add(new Phone
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Model = Convert.ToString(reader["Model"]),
                                Manufacturer = Convert.ToString(reader["Manufacturer"]),
                                Price = Convert.ToInt32(reader["Price"]),
                            });
                        }
                    }
                }
                // И пишем этот массив в консоль
                foreach (var phone in phones)
                {
                    Console.WriteLine($"{phone.Id} {phone.Model}");
                }
            }
        }
    }
}
