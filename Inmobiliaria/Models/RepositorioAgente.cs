using Inmobiliaria.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inmobiliaria.Models
{
    public class RepositorioAgente : RepositorioBase, IRepositorioAgente
    {
        public RepositorioAgente(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Agente a)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Agentes (Mail, Clave, Salt, Rol, FechaAlta) " +
                    $"VALUES ('{a.Mail}', '{a.Clave}', '{a.Salt}', '{a.Rol}', SYSDATETIME());";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    command.CommandText = "SELECT SCOPE_IDENTITY()";
                    var id = command.ExecuteScalar();
                    a.Id = Convert.ToInt32(id);
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
                string sql = $"DELETE FROM Agentes WHERE Id = {id}";
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

        public int Modificacion(Agente a)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Agentes SET Mail='{a.Mail}' " +
                    $"WHERE Id = {a.Id}";
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

        public IList<Agente> ObtenerTodos()
        {
            IList<Agente> res = new List<Agente>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Mail, Rol" +
                    $" FROM Agentes";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Agente p = new Agente
                        {
                            Id = reader.GetInt32(0),
                            Mail = reader.GetString(1),
                            Rol = reader.GetString(2),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Agente ObtenerPorId(int id)
        {
            Agente p = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Mail FROM Agentes " +
                    $"WHERE Id={id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = new Agente
                        {
                            Id = reader.GetInt32(0),
                            Mail = reader.GetString(1)
                        };
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public Agente GetPass(string mail)
        {
            Agente a = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Clave, Salt, Rol FROM Agentes " +
                    $"WHERE Mail='{mail}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        a = new Agente
                        {
                            Id = reader.GetInt32(0),
                            Mail = mail,
                            Clave = reader.GetString(1),
                            Salt = reader.GetString(2),
                            Rol = reader.GetString(3),
                        };
                    }
                    connection.Close();
                }
            }
            return a;
        }

        public Agente GetPassPorId(int id)
        {
            Agente a = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Clave, Salt FROM Agentes " +
                    $"WHERE Id={id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        a = new Agente
                        {
                            Id = reader.GetInt32(0),
                            Clave = reader.GetString(1),
                            Salt = reader.GetString(2),
                        };
                    }
                    connection.Close();
                }
            }
            return a;
        }

        public int ModificarClave(Agente a)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Agentes SET Clave='{a.Clave}', Salt='{a.Salt}' " +
                    $"WHERE Id = {a.Id}";
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
    }
}
