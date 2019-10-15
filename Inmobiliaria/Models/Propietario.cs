using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class Propietario
    {
        public int Id { get; set; }
        [Required]
        public String Nombre { get; set; }
        [Required]
        public String Apellido { get; set; }
        [Required, EmailAddress]
        public String Mail { get; set; }
        [Required, DataType(DataType.Password)]
        public String Clave { get; set; }
        public String Salt { get; set; }
        [Required]
        public String Dni { get; set; }
        [Required]
        public String Telefono { get; set; }
        public DateTime FechaAlta { get; set; }
    }
}
