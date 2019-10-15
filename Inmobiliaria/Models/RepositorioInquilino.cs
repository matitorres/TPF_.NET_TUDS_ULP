using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inmobiliaria.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorio<Inquilino>
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Inquilino p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Inquilinos (Dni, Nombre, Apellido, Direccion, Telefono, Mail, Trabajo, FechaAlta) " +
                    $"VALUES ('{p.Dni}','{p.Nombre}', '{p.Apellido}','{p.Direccion}','{p.Telefono}','{p.Mail}','{p.Trabajo}', GETDATE())";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    command.CommandText = "SELECT SCOPE_IDENTITY()";
                    p.Id = int.Parse(command.ExecuteScalar().ToString());
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM Inquilinos WHERE Id = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Modificacion(Inquilino p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Inquilinos SET Dni='{p.Dni}', Nombre='{p.Nombre}', Apellido='{p.Apellido}', Direccion='{p.Direccion}', Telefono='{p.Telefono}', Mail='{p.Mail}', Trabajo='{p.Trabajo}' " +
                    $"WHERE Id = {p.Id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inquilino> ObtenerTodos()
        {
            IList<Inquilino> res = new List<Inquilino>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Dni, Nombre, Apellido, Direccion, Telefono, Mail, Trabajo" +
                    $" FROM Inquilinos";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inquilino p = new Inquilino
                        {
                            Id = reader.GetInt32(0),
                            Dni = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Apellido = reader.GetString(3),
                            Direccion = reader.GetString(4),
                            Telefono = reader.GetString(5),
                            Mail = reader.GetString(6),
                            Trabajo = reader.GetString(6),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Inquilino ObtenerPorId(int id)
        {
            Inquilino i = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Dni, Nombre, Apellido, Direccion, Telefono, Mail, Trabajo FROM Inquilinos" +
                    $" WHERE Id={id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        i = new Inquilino
                        {
                            Id = reader.GetInt32(0),
                            Dni = reader.GetString(1),
                            Nombre = reader.GetString(2),
                            Apellido = reader.GetString(3),
                            Direccion = reader.GetString(4),
                            Telefono = reader.GetString(5),
                            Mail = reader.GetString(6),
                            Trabajo = reader.GetString(7),
                        };
                        return i;
                    }
                    connection.Close();
                }
            }
            return i;
        }
    }
}
