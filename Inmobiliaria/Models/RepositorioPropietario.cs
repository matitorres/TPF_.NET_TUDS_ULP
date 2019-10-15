using Inmobiliaria.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Inmobiliaria.Models
{
    public class RepositorioPropietario : RepositorioBase, IRepositorioPropietario
    {
        public RepositorioPropietario(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Propietario p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Propietarios (Nombre, Apellido, Mail, Clave, Salt, Dni, Telefono, FechaAlta) " +
                    $"VALUES ('{p.Nombre}', '{p.Apellido}', '{p.Mail}', '{p.Clave}', '{p.Salt}', '{p.Dni}', '{p.Telefono}', SYSDATETIME());";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    command.CommandText = "SELECT SCOPE_IDENTITY()";
                    var id = command.ExecuteScalar();
                    p.Id = Convert.ToInt32(id);
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
                string sql = $"DELETE FROM Propietarios WHERE Id = {id}";
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

        public int Modificacion(Propietario p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Propietarios SET Nombre='{p.Nombre}', Apellido='{p.Apellido}', Mail='{p.Mail}', Dni='{p.Dni}', Telefono='{p.Telefono}' " +
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

        public IList<Propietario> ObtenerTodos()
        {
            IList<Propietario> res = new List<Propietario>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Mail, Dni, Telefono, FechaAlta" +
                    $" FROM Propietarios";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Propietario p = new Propietario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Mail = reader.GetString(3),
                            Dni = reader.GetString(4),
                            Telefono = reader.GetString(5),
                            FechaAlta = reader.GetDateTime(6),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Propietario ObtenerPorId(int id)
        {
            Propietario p = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Mail, Dni, Telefono FROM Propietarios" +
                    $" WHERE Id={id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = new Propietario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Mail = reader.GetString(3),
                            Dni = reader.GetString(4),
                            Telefono = reader.GetString(5),
                        };
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public Propietario GetPass(string mail)
        {
            Propietario p = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Clave, Salt FROM Propietarios" +
                    $" WHERE Mail={mail}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = new Propietario
                        {
                            Id = reader.GetInt32(0),
                            Clave = reader.GetString(1),
                            Salt = reader.GetString(2),
                        };
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public IList<Propietario> BuscarPorNombre(string nombre)
        {
            List<Propietario> res = new List<Propietario>();
            Propietario p = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Mail, Dni, Telefono FROM Propietarios" +
                    $" WHERE Nombre LIKE %{nombre}% OR Apellido LIKE %{nombre}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p = new Propietario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Mail = reader.GetString(3),
                            Dni = reader.GetString(4),
                            Telefono = reader.GetString(5),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}
