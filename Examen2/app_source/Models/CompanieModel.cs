namespace app_source.Models
{
    public class CompanieModel
    {
        // nombre de la empresa
        public string? Nombre { get; set; }

        // el tipo de negocio es muy variado
        // puede ser distribuidor, contruccion, restaurante, etc.
        public string? TipoNegocio { get; set; }

        // pais donde esta la sede principal de la empresa
        public string?  PaisBase { get; set; }

        // valor estimado total de la empresa
        // puede ser un numero real
        public decimal ValorEstimado { get; set; }    
    }
}
