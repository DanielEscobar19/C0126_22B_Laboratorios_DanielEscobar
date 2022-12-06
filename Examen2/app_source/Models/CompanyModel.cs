using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace app_source.Models
{
    public class CompanyModel
    {
        // nombre de la empresa
        [Required(ErrorMessage = "Debe indicar el nombre de la empresa")]
        [StringLength(256, ErrorMessage = "El nombre de la empresa debe tener menos de 256 carácteres")]
        [DisplayName("Nombre de la empresa:")]
        [Key] // el nombre sera la llave en la base de datos
        public string? Nombre { get; set; }

        // el tipo de negocio es muy variado
        // puede ser distribuidor, contruccion, restaurante, etc.
        [StringLength(256, ErrorMessage = "El tipo de negocio debe tener menos de 256 carácteres")]
        [DisplayName("Tipo de negocio:")]
        public string? TipoNegocio { get; set; }

        // pais donde esta la sede principal de la empresa
        [StringLength(256, ErrorMessage = "El nombre del país base debe tener menos de 256 carácteres")]
        [DisplayName("País base de la empresa:")]
        public string? PaisBase { get; set; }

        // valor estimado total de la empresa
        // puede ser un numero real
        [DisplayName("Valor estimado de la empresa:")]
        public decimal? ValorEstimado { get; set; }

        // true si es transacional y false en caso contrario
        [DisplayName("¿La empresa es trasnacional?")]
        [Required(ErrorMessage = "Debe indicar si la empresa es trasnacional")]
        public bool EsTransnacional { get; set; }
    }
}
