using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Contrato
    {
        public int Id { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }
        [Required]
        public Decimal Precio { get; set; }
        [Required]
        public String NombreGarante { get; set; }
        [Required]
        public String DniGarante { get; set; }
        [Required]
        public String TelefonoGarante { get; set; }
        public DateTime FechaAlta { get; set; }
        public Inmueble Inmueble { get; set; }
        public Inquilino Inquilino { get; set; }
        public Agente Agente { get; set; }
    }
}
