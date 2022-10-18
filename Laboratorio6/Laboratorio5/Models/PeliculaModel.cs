/*
    Daniel Escobar Giraldo | C02748 
*/

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio5.Models
{
    public class PeliculaModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Debe ingresar un nombre")]
        [DisplayName("Nombre de la pelicula: ")]

        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar un año")]
        [DisplayName("Año de la película:")]
        [RegularExpression("(18|19|20)[0-9]{2}", ErrorMessage = "Ingrese un año válido")]
        public int Año { get; set; }
    }
}
