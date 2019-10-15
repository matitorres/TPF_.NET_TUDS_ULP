using Inmobiliaria.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inmobiliaria.Models
{
    public class RepositorioPago : RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Pago p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Pagos (NumeroPago, Fecha, Contrato) " +
                    $"VALUES ('{p.NumeroPago}', SYSDATETIME(), {p.Contrato.Id})";
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
                string sql = $"DELETE FROM Pagos WHERE Id = {id}";
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

        public int Modificacion(Pago p)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Pagos SET NumeroPago={p.NumeroPago}, Fecha='{p.Fecha}', Importe={p.Importe}, Contrato='{p.Contrato.Id}'" +
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

        public IList<Pago> ObtenerTodos()
        {
            IList<Pago> res = new List<Pago>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, NumeroPago, Fecha, Contrato" +
                    $" FROM Pagos";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Pago p = new Pago
                        {
                            Id = reader.GetInt32(0),
                            NumeroPago = reader.GetString(1),
                            Fecha = reader.GetDateTime(2),
                            Contrato = new Contrato
                            {
                                Id = reader.GetInt32(3)
                            }
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Pago ObtenerPorId(int id)
        {
            Pago p = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT p.Id, NumeroPago, Fecha, Contrato, Precio FROM Pagos p, Contratos c WHERE Contrato = c.Id " +
                    $"AND p.Id={id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = new Pago
                        {
                            Id = reader.GetInt32(0),
                            NumeroPago = reader.GetString(1),
                            Fecha = reader.GetDateTime(2),
                            Contrato = new Contrato {
                                Id = reader.GetInt32(3),
                                Precio = reader.GetDecimal(4)
                            }
                        };
                    }
                    connection.Close();
                }
            }
            return p;
        }

        public IList<Pago> BuscarPorContrato(Contrato c)
        {
            IList<Pago> res = new List<Pago>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, NumeroPago, Fecha FROM Pagos" +
                    $" WHERE Contrato={c.Id} ORDER BY NumeroPago DESC";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Pago p = new Pago
                        {
                            Id = reader.GetInt32(0),
                            NumeroPago = reader.GetString(1),
                            Fecha = reader.GetDateTime(2),
                            Contrato = new Contrato
                            {
                                Id = c.Id,
                                Precio = c.Precio
                            }
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
