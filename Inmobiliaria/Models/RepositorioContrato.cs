using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inmobiliaria.Models
{
    public class RepositorioContrato : RepositorioBase, IRepositorioContrato
    {
        public RepositorioContrato(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Contrato entidad)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Contratos (FechaInicio, FechaFin, Precio, NombreGarante, DniGarante, TelefonoGarante," +
                    $"FechaAlta, Inmueble, Inquilino, Agente) " +
                    $"VALUES (@fecha_inicio, @fecha_fin, {entidad.Inmueble.Precio.ToString().Replace(",", ".")}, '{entidad.NombreGarante}', '{entidad.DniGarante}', " +
                    $"'{entidad.TelefonoGarante}', SYSDATETIME(), {entidad.Inmueble.Id}, {entidad.Inquilino.Id}, {entidad.Agente.Id}); " +
                    $"SELECT SCOPE_IDENTITY() AS [IdGenerado];";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@fecha_inicio", SqlDbType.DateTime).Value = entidad.FechaInicio;
                    command.Parameters.Add("@fecha_fin", SqlDbType.DateTime).Value = entidad.FechaFin;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    int idGenerado = Convert.ToInt32(command.ExecuteScalar());
                    entidad.Id = idGenerado;
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
                string sql = $"DELETE FROM Contratos WHERE Id = {id}";
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

        public int Modificacion(Contrato contrato)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Contratos SET FechaInicio = @fecha_inicio, FechaFin = @fecha_fin, " +
                    $"Precio = {contrato.Precio.ToString().Replace(",", ".")}, NombreGarante = '{contrato.NombreGarante}', " +
                    $"DniGarante = '{contrato.DniGarante}', TelefonoGarante = '{contrato.TelefonoGarante}' WHERE Id = {contrato.Id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@fecha_inicio", SqlDbType.DateTime).Value = contrato.FechaInicio;
                    command.Parameters.Add("@fecha_fin", SqlDbType.DateTime).Value = contrato.FechaFin;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> ObtenerTodos()
        {
            IList<Contrato> res = new List<Contrato>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, c.FechaInicio, c.FechaFin, c.Precio, c.NombreGarante, c.DniGarante, c.TelefonoGarante, " +
                    $"c.Inquilino, i.Nombre, i.Apellido, p.Direccion FROM Inmuebles p, Inquilinos i, Contratos c " +
                    $"WHERE c.Inmueble = p.Id AND c.Inquilino = i.Id ORDER BY c.FechaFin DESC;";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Precio = reader.GetDecimal(3),
                            NombreGarante = reader.GetString(4),
                            DniGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(8),
                                Apellido = reader.GetString(9),
                            },
                            Inmueble = new Inmueble
                            {
                                Direccion = reader.GetString(10),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Contrato ObtenerPorId(int id)
        {
            Contrato entidad = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, c.FechaInicio, c.FechaFin, c.Precio, c.NombreGarante, c.DniGarante, c.TelefonoGarante, " +
                    $"i.Id, i.Nombre, i.Apellido, p.Id, p.Direccion FROM Inmuebles p, Inquilinos i, Contratos c WHERE c.Inmueble = p.Id " +
                    $"AND c.Inquilino = i.Id AND c.Id = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Precio = reader.GetDecimal(3),
                            NombreGarante = reader.GetString(4),
                            DniGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(8),
                                Apellido = reader.GetString(9),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(10),
                                Direccion = reader.GetString(11),
                            }
                        };
                    };
                    connection.Close();
                }
            }
            return entidad;
        }

        public IList<Contrato> BuscarPorInmueble(int idInmueble)
        {
            List<Contrato> res = new List<Contrato>();
            Contrato entidad = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, c.FechaInicio, c.FechaFin, c.Precio, c.NombreGarante, c.DniGarante, c.TelefonoGarante, " +
                    $"i.Id, i.Nombre, i.Apellido, p.Direccion FROM Inmuebles p, Inquilinos i, Contratos c WHERE c.Inmueble = p.Id " +
                    $"AND c.Inquilino = i.Id AND p.Id = {idInmueble}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Precio = reader.GetDecimal(3),
                            NombreGarante = reader.GetString(4),
                            DniGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(8),
                                Apellido = reader.GetString(9),
                            },
                            Inmueble = new Inmueble
                            {
                                Direccion = reader.GetString(10),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> BuscarPorInquilino(int idInquilino)
        {
            List<Contrato> res = new List<Contrato>();
            Contrato entidad = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, c.FechaInicio, c.FechaFin, c.Precio, c.NombreGarante, c.DniGarante, c.TelefonoGarante, " +
                    $"i.Id, i.Nombre, i.Apellido, p.Direccion FROM Inmuebles p, Inquilinos i, Contratos c WHERE c.Inmueble = p.Id " +
                    $"AND c.Inquilino = i.Id AND i.Id = {idInquilino}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Precio = reader.GetDecimal(3),
                            NombreGarante = reader.GetString(4),
                            DniGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(8),
                                Apellido = reader.GetString(9),
                            },
                            Inmueble = new Inmueble
                            {
                                Direccion = reader.GetString(10),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Contrato> BuscarVigentes()
        {
            IList<Contrato> res = new List<Contrato>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT c.Id, c.FechaInicio, c.FechaFin, c.Precio, c.NombreGarante, c.DniGarante, c.TelefonoGarante, " +
                    $"i.Id, i.Nombre, i.Apellido, p.Direccion FROM Inmuebles p, Inquilinos i, Contratos c WHERE c.Inmueble = p.Id " +
                    $"AND c.Inquilino = i.Id AND c.FechaFin > SYSDATETIME() AND c.FechaInicio < SYSDATETIME();";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Contrato entidad = new Contrato
                        {
                            Id = reader.GetInt32(0),
                            FechaInicio = reader.GetDateTime(1),
                            FechaFin = reader.GetDateTime(2),
                            Precio = reader.GetDecimal(3),
                            NombreGarante = reader.GetString(4),
                            DniGarante = reader.GetString(5),
                            TelefonoGarante = reader.GetString(6),
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(7),
                                Nombre = reader.GetString(8),
                                Apellido = reader.GetString(9),
                            },
                            Inmueble = new Inmueble
                            {
                                Direccion = reader.GetString(10),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Contrato ObtenerPorIdRenovar(int id)
        {
            Contrato entidad = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Inquilino, Inmueble FROM Contratos WHERE Id = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = new Contrato
                        {
                            Inquilino = new Inquilino
                            {
                                Id = reader.GetInt32(0),
                            },
                            Inmueble = new Inmueble
                            {
                                Id = reader.GetInt32(1),
                            }
                        };
                    };
                    connection.Close();
                }
            }
            return entidad;
        }
    }
}
