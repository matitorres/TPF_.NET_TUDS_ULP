using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria.Models
{
    public class LoginView
    {
        [DataType(DataType.EmailAddress)]
        public String Mail { get; set; }
        [DataType(DataType.Password)]
        public String Clave { get; set; }
    }
}
