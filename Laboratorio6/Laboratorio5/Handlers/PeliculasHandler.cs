/*
    Daniel Escobar Giraldo | C02748
*/
using System;
using System.Collections.Generic;
using Laboratorio5.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
namespace Laboratorio5.Handlers
{
    public class PeliculasHandler
    {
        private SqlConnection conexion;
        private string rutaConexion;
        public PeliculasHandler()
        {
            var builder = WebApplication.CreateBuilder();
            rutaConexion = builder.Configuration.GetConnectionString("ContextoDePeliculas");
            conexion = new SqlConnection(rutaConexion);
        }
        private DataTable CrearTablaConsulta(string consulta)
        {
            SqlCommand comandoParaConsulta = new SqlCommand(consulta,
            conexion);
            SqlDataAdapter adaptadorParaTabla = new
            SqlDataAdapter(comandoParaConsulta);
            DataTable consultaFormatoTabla = new DataTable();
            conexion.Open();
            adaptadorParaTabla.Fill(consultaFormatoTabla);
            conexion.Close();
            return consultaFormatoTabla;
        }
        public List<PeliculaModel> ObtenerPeliculas()
        {
            List<PeliculaModel> peliculas = new List<PeliculaModel>();
            string consulta = "SELECT * FROM Pelicula";
            DataTable tablaResultado = CrearTablaConsulta(consulta);
            foreach (DataRow columna in tablaResultado.Rows)
            {
                peliculas.Add(
                new PeliculaModel
                {
                    ID = Convert.ToInt32(columna["Id"]),
                    Nombre = Convert.ToString(columna["Nombre"]),
                    Año = Convert.ToInt32(columna["Año"]),
                });
            }
            return peliculas;
        }
        public bool CrearPelicula(PeliculaModel pelicula)
        {
            var consulta = @"INSERT INTO [dbo].[Pelicula] ([Nombre],[Año]) VALUES(@Nombre, @Año)";

            var comandoParaConsulta = new SqlCommand(consulta, conexion);
            comandoParaConsulta.Parameters.AddWithValue("@Nombre", pelicula.Nombre);
            comandoParaConsulta.Parameters.AddWithValue("@Año", pelicula.Año);
            conexion.Open();

            bool exito = comandoParaConsulta.ExecuteNonQuery() >= 1;
            conexion.Close();
            return exito;
        }
        public bool EditarPelicula(PeliculaModel pelicula)
        {
            var consulta = @"UPDATE [dbo].[Pelicula] SET Nombre = @Nombre, Año = @Año WHERE ID = @ID";

            var comandoParaConsulta = new SqlCommand(consulta, conexion);
            comandoParaConsulta.Parameters.AddWithValue("@Nombre", pelicula.Nombre);
            comandoParaConsulta.Parameters.AddWithValue("@Año", pelicula.Año);
            comandoParaConsulta.Parameters.AddWithValue("@ID", pelicula.ID);
            conexion.Open();

            bool exito = comandoParaConsulta.ExecuteNonQuery() >= 1;
            conexion.Close();
            return exito;
        }
        public bool BorrarPelicula(PeliculaModel pelicula)
        {
            var consulta = @"Delete [dbo].[pelicula] WHERE ID = @ID";

            var comandoParaConsulta = new SqlCommand(consulta, conexion);
            comandoParaConsulta.Parameters.AddWithValue("@ID", pelicula.ID);
            conexion.Open();

            bool exito = comandoParaConsulta.ExecuteNonQuery() >= 1;
            conexion.Close();
            return exito;
        }
        /*
            Daniel Escobar Giraldo | C02748
         */
    }
}
