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
        [Required(ErrorMessage = "Debe indicar el tipo de empresa")]
        [StringLength(256, ErrorMessage = "El tipo de negocio debe tener menos de 256 carácteres")]
        [DisplayName("Tipo de negocio:")]
        [DefaultValue("Desconocido")]
        public string? TipoNegocio { get; set; }

        // pais donde esta la sede principal de la empresa
        [Required(ErrorMessage = "Debe indicar el país de origen de la empresa")]
        [StringLength(256, ErrorMessage = "El nombre del país base debe tener menos de 256 carácteres")]
        [DisplayName("País base de la empresa:")]
        [DefaultValue("Desconocido")]
        public string? PaisBase { get; set; }

        // valor estimado total de la empresa
        // puede ser un numero real
        [DisplayName("Valor estimado de la empresa:")]
        [DefaultValue(0)]
        public decimal ValorEstimado { get; set; }

        // true si es transacional y false en caso contrario
        [Required(ErrorMessage = "Debe indicar si la empresa es trasnacional")]
        public bool EsTransacional { get; set; }
    }
}
