using app_source.Controllers;
using app_source.Data;
using app_source.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace unit_tests;

public class CompanyControllerTest
{
    private CompanyContext DbContext;
    private List<CompanyModel> EmpresasSemilla = new();
    private CompanyController? CompanyController = null;
    private const string UNIQUEID = "FXc21c02748";

    [SetUp]
    public void Setup()
    {
        DbContext = Helper.DbContext;
        CompanyController = new(DbContext);
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var item in EmpresasSemilla) {
            DbContext.Remove(item);
        }
        DbContext.SaveChanges();
        EmpresasSemilla.Clear();
        CompanyController = null;
    }

    [Test]
    public async Task IsCompanyNameAvailable_ReturnsFalse_WhenCreatingNewRepeatedName()
    {
        // arrange
        // ingresamos empresas cualquiera
        // creamos lista con empresas de prueba
        for (int i = 0; i < 4; ++i)
        {
            CompanyModel tempCompany = new CompanyModel
            {
                // solo asignamos un nombre porque nos interesa probrar si se peude repetir nombres
                Nombre = $"Testing {i} {UNIQUEID}",
            };
            // ingresamos en la base y y actualizamos el modelo actual
            // la actualizacion es para recibir el numero de id que asigna la base
            tempCompany = DbContext.Add(tempCompany).Entity;

            EmpresasSemilla.Add(tempCompany);
        }

        // guardamos en la base los cambios realizados
        await DbContext.SaveChangesAsync();

        // creamos una empresa que tiene el nombre repetido y que no esta en la base de datos
        // esto significa que es la situacion de la vista de creacion
        // el modelo no tiene id porque no ha sido ingresado a la base
        CompanyModel nombreRepetido = new()
        {
            Nombre = EmpresasSemilla[0].Nombre
        };

        // action
        var resultadoJson = CompanyController.IsCompanyNameAvailable(nombreRepetido.Nombre, nombreRepetido.Id) as JsonResult;

        // assert
        bool? result = AssertTypeOfJsonResutl(resultadoJson);

        Assert.IsFalse(result, $"Se retorno {result} cuando el nombre no estaba disponible porque ya existe una empresa con el mismo nombre. Se espera false como resultado");
    }

    [Test]
    public async Task IsCompanyNameAvailable_ReturnsTrue_WhenCreatingNewNotRepeatedName()
    {
        // arrange
        // ingresamos empresas cualquiera
        // creamos lista con empresas de prueba
        for (int i = 0; i < 4; ++i)
        {
            CompanyModel tempCompany = new CompanyModel
            {
                // solo asignamos un nombre porque nos interesa probrar si se peude repetir nombres
                Nombre = $"Testing {i} {UNIQUEID}",
            };
            // ingresamos en la base y y actualizamos el modelo actual
            // la actualizacion es para recibir el numero de id que asigna la base
            tempCompany = DbContext.Add(tempCompany).Entity;

            EmpresasSemilla.Add(tempCompany);
        }

        // guardamos en la base los cambios realizados
        await DbContext.SaveChangesAsync();

        // creamos una empresa que tiene el nombre repetido y que no esta en la base de datos
        // esto significa que es la situacion de la vista de creacion
        // el modelo no tiene id porque no ha sido ingresado a la base
        CompanyModel nombreRepetido = new()
        {
            Nombre = "Nombre no repetido 7895"
        };

        // action
        var resultadoJson = CompanyController.IsCompanyNameAvailable(nombreRepetido.Nombre, nombreRepetido.Id) as JsonResult;

        // assert
        bool? result = AssertTypeOfJsonResutl(resultadoJson);

        Assert.IsTrue(result, $"Se retorno {result} cuando el nombre no estaba disponible porque ya existe una empresa con el mismo nombre. Se espera false como resultado");
    }

    [Test]
    public async Task IsCompanyNameAvailable_ReturnsTrue_WhenUpdatingAndNotChangingName()
    {
        // arrange
        // ingresamos empresas cualquiera
        // creamos lista con empresas de prueba
        for (int i = 0; i < 4; ++i)
        {
            CompanyModel tempCompany = new CompanyModel
            {
                // solo asignamos un nombre porque nos interesa probrar si se peude repetir nombres
                Nombre = $"Testing {i} {UNIQUEID}",
            };
            // ingresamos en la base y y actualizamos el modelo actual
            // la actualizacion es para recibir el numero de id que asigna la base
            tempCompany = DbContext.Add(tempCompany).Entity;

            EmpresasSemilla.Add(tempCompany);
        }

        // guardamos en la base los cambios realizados
        await DbContext.SaveChangesAsync();

        // como estamos actualizando usamos uno de los modelos que ya estan en la base
        // modifcamos otro atributo que no sea el nombre
        EmpresasSemilla[2].TipoNegocio= $"Negocio de testing {UNIQUEID}";

        // action
        var resultadoJson = CompanyController.IsCompanyNameAvailable(EmpresasSemilla[2].Nombre, EmpresasSemilla[2].Id) as JsonResult;

        // assert
        bool? result = AssertTypeOfJsonResutl(resultadoJson);

        Assert.IsTrue(result, $"Se retorno {result} y debió ser true porque se está actuaizando una empresa pero no se modifico el nombre por lo que se permite la actualización.");
    }

    private static bool? AssertTypeOfJsonResutl(JsonResult controllerResult)
    {
        bool? result = null;
        try
        {
            result = Convert.ToBoolean(controllerResult.Value);
        }
        catch
        {
            Assert.Fail($"Se esperaba un booleano como respuesta");
        }
        return result;
    }
}