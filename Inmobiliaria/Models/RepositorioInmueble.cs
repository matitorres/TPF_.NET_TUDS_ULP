using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Inmobiliaria.Models
{
    public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
    {
        public RepositorioInmueble(IConfiguration configuration) : base(configuration)
        {

        }

        public int Alta(Inmueble entidad)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"INSERT INTO Inmuebles (Latitud, Longitud, Tipo, Uso, Ambientes, Direccion, Precio, Estado, FechaAlta, Propietario) " +
                    $"VALUES ('{entidad.Latitud}', '{entidad.Longitud}', '{entidad.Tipo}', '{entidad.Uso}', {entidad.Ambientes}, " +
                    $"'{entidad.Direccion}', {entidad.Precio.ToString().Replace(",", ".")}, 1, SYSDATETIME(), {entidad.Propietario.Id}); " +
                    $"SELECT SCOPE_IDENTITY() AS [IdGenerado]";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
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
                string sql = $"DELETE FROM Inmuebles WHERE Id = {id}";
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

        public int Modificacion(Inmueble inmueble)
        {
            int res = -1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"UPDATE Inmuebles SET Latitud='{inmueble.Latitud}', Longitud='{inmueble.Longitud}', Tipo='{inmueble.Tipo}', " +
                    $"Uso='{inmueble.Uso}', Ambientes={inmueble.Ambientes}, Direccion='{inmueble.Direccion}', " +
                    $"Precio = {inmueble.Precio.ToString().Replace(",", ".")}, Estado={Convert.ToInt32(inmueble.Estado)} " +
                    $"WHERE Id = {inmueble.Id};";
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

        public IList<Inmueble> ObtenerTodos()
        {
            IList<Inmueble> res = new List<Inmueble>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT i.Id, Latitud, Longitud, Tipo, Uso, Ambientes, Direccion, Precio, Estado, Propietario, p.Nombre," +
                    $" p.Apellido FROM Inmuebles i INNER JOIN Propietarios p ON i.Propietario = p.Id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble entidad = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Latitud = reader.GetString(1),
                            Longitud = reader.GetString(2),
                            Tipo = reader.GetString(3),
                            Uso = reader.GetString(4),
                            Ambientes = reader.GetInt32(5),
                            Direccion = reader.GetString(6),
                            Precio = reader.GetDecimal(7),
                            Estado = reader.GetBoolean(8),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(9),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public Inmueble ObtenerPorId(int id)
        {
            Inmueble entidad = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT i.Id, Latitud, Longitud, Tipo, Uso, Ambientes, Direccion, Precio, Estado, Propietario, p.Nombre," +
                    $" p.Apellido FROM Inmuebles i INNER JOIN Propietarios p ON i.Propietario = p.Id WHERE i.Id={id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        entidad = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Latitud = reader.GetString(1),
                            Longitud = reader.GetString(2),
                            Tipo = reader.GetString(3),
                            Uso = reader.GetString(4),
                            Ambientes = reader.GetInt32(5),
                            Direccion = reader.GetString(6),
                            Precio = reader.GetDecimal(7),
                            Estado = reader.GetBoolean(8),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(9),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            }
                        };
                    };
                    connection.Close();
                }
            }
            return entidad;
        }

        public IList<Inmueble> BuscarPorPropietario(int idPropietario)
        {
            List<Inmueble> res = new List<Inmueble>();
            Inmueble entidad = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT i.Id, Latitud, Longitud, Tipo, Uso, Ambientes, Direccion, Precio, Estado, Propietario, p.Nombre," +
                    $" p.Apellido FROM Inmuebles i INNER JOIN Propietarios p ON i.Propietario = p.Id WHERE Propietario={idPropietario}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        entidad = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Latitud = reader.GetString(1),
                            Longitud = reader.GetString(2),
                            Tipo = reader.GetString(3),
                            Uso = reader.GetString(4),
                            Ambientes = reader.GetInt32(5),
                            Direccion = reader.GetString(6),
                            Precio = reader.GetDecimal(7),
                            Estado = reader.GetBoolean(8),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(9),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inmueble> ObtenerDisponibles()
        {
            List<Inmueble> res = new List<Inmueble>();
            Inmueble entidad = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT i.Id, Latitud, Longitud, Tipo, Uso, Ambientes, Direccion, Precio, Estado, Propietario, p.Nombre, p.Apellido " +
                    $"FROM (SELECT * FROM Inmuebles WHERE Estado = 1) i " +
                    $"LEFT JOIN (SELECT MAX(Id) AS Id, Inmueble, MAX(FechaFin) AS FechaFin FROM Contratos GROUP BY Inmueble) c ON i.Id = c.Inmueble " +
                    $"LEFT JOIN Propietarios p ON p.Id = i.Propietario " +
                    $"WHERE c.FechaFin < SYSDATETIME() " +
                    $"OR c.Id IS NULL " +
                    $"GROUP BY i.Id, Latitud, Longitud, Tipo, Uso, Ambientes, Direccion, Precio, Estado, Propietario, p.Nombre, p.Apellido;";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        entidad = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Latitud = reader.GetString(1),
                            Longitud = reader.GetString(2),
                            Tipo = reader.GetString(3),
                            Uso = reader.GetString(4),
                            Ambientes = reader.GetInt32(5),
                            Direccion = reader.GetString(6),
                            Precio = reader.GetDecimal(7),
                            Estado = reader.GetBoolean(8),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(9),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public IList<Inmueble> Buscar(Inmueble inmueble)
        {
            List<Inmueble> res = new List<Inmueble>();
            Inmueble entidad = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT i.Id, Latitud, Longitud, Tipo, Uso, Ambientes, Direccion, Precio, Estado, Propietario, p.Nombre," +
                    $" p.Apellido FROM Inmuebles i INNER JOIN Propietarios p ON i.Propietario = p.Id ";

                if (inmueble.Tipo != "" || inmueble.Uso != "" || inmueble.Ambientes != 0 || inmueble.Precio != 0)
                {
                    sql += "WHERE ";

                    String strTipo = $" Tipo LIKE '%{inmueble.Tipo}%' ";
                    String strUso = $" Uso LIKE '%{inmueble.Uso}%' ";
                    String strAmbientes = $" Ambientes = {inmueble.Ambientes} ";
                    String strPrecio = $" Precio BETWEEN {(inmueble.Precio - 2000).ToString().Replace(",", ".")} AND {(inmueble.Precio + 2000).ToString().Replace(",", ".")} ";

                    List<String> partes = new List<String>();

                    if (inmueble.Tipo != "") partes.Add(strTipo);
                    if (inmueble.Uso!= "") partes.Add(strUso);
                    if (inmueble.Ambientes != 0) partes.Add(strAmbientes);
                    if (inmueble.Precio != 0) partes.Add(strPrecio);

                    sql += String.Join("AND", partes);
                }


                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        entidad = new Inmueble
                        {
                            Id = reader.GetInt32(0),
                            Latitud = reader.GetString(1),
                            Longitud = reader.GetString(2),
                            Tipo = reader.GetString(3),
                            Uso = reader.GetString(4),
                            Ambientes = reader.GetInt32(5),
                            Direccion = reader.GetString(6),
                            Precio = reader.GetDecimal(7),
                            Estado = reader.GetBoolean(8),
                            Propietario = new Propietario
                            {
                                Id = reader.GetInt32(9),
                                Nombre = reader.GetString(10),
                                Apellido = reader.GetString(11),
                            }
                        };
                        res.Add(entidad);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}
