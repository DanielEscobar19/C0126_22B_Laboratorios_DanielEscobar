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
    public void IsCompanyNameAvailable_ReturnsFalse_WhenCreatingNewRepeatedName()
    {
        // arrange
        // insertamos los puestos semilla de prueba
        Helper.InsertarEmpresasSemilla(ref EmpresasSemilla, DbContext, UNIQUEID);
        
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
    public void IsCompanyNameAvailable_ReturnsTrue_WhenCreatingNewNotRepeatedName()
    {
        // arrange
        // insertamos los puestos semilla de prueba
        Helper.InsertarEmpresasSemilla(ref EmpresasSemilla, DbContext, UNIQUEID);

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
    public void IsCompanyNameAvailable_ReturnsTrue_WhenUpdatingAndNotChangingName()
    {
        // arrange
        // insertamos los puestos semilla de prueba
        Helper.InsertarEmpresasSemilla(ref EmpresasSemilla, DbContext, UNIQUEID);

        // como estamos actualizando usamos uno de los modelos que ya estan en la base
        // modifcamos otro atributo que no sea el nombre
        EmpresasSemilla[2].TipoNegocio= $"Negocio de testing {UNIQUEID}";

        // action
        var resultadoJson = CompanyController.IsCompanyNameAvailable(EmpresasSemilla[2].Nombre, EmpresasSemilla[2].Id) as JsonResult;

        // assert
        bool? result = AssertTypeOfJsonResutl(resultadoJson);

        Assert.IsTrue(result, $"Se retorno {result} y debió ser true porque se está actualizando una empresa pero no se modifico el nombre por lo que se permite la actualización.");
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

    [Test]
    public async Task GetEdit_ReturnsEditViewWithCorrespondingModel()
    {
        // arrange
        // insertamos los puestos semilla de prueba
        Helper.InsertarEmpresasSemilla(ref EmpresasSemilla, DbContext, UNIQUEID);

        // copiamos los datos esperados porque el paso de modelos es por referencia
        // por lo tanto si comparamos los modelos siemrpe van a tener mismos datos porque hacen referencia al mismo
        int idEsperado = EmpresasSemilla[2].Id;
        string nombreEsperado = EmpresasSemilla[2].Nombre;
        string tipoEsperado = EmpresasSemilla[2].TipoNegocio;
        string paisEsperado = EmpresasSemilla[2].PaisBase;
        Decimal? valorEsperado = EmpresasSemilla[2].ValorEstimado;
        bool esTransnacionalEsperado = EmpresasSemilla[2].EsTransnacional;

        // action
        var viewResult = await CompanyController.Edit(EmpresasSemilla[2].Id) as ViewResult;

        // assert
        var actualCompany = viewResult.ViewData.Model as CompanyModel;

        AssertComapanyModelType(actualCompany);

        // comparamos que sean el mismo modelo
        Assert.That(actualCompany, Is.Not.Null, "El modelo de la vista es nulo");
        Assert.That(actualCompany.Id, Is.EqualTo(idEsperado), $"El id del modelo no es el esperado. Se esperaba {idEsperado}");
        Assert.That(actualCompany.Nombre, Is.EqualTo(nombreEsperado), $"El nombre del modelo no es el esperado. Se esperaba {nombreEsperado}");
        Assert.That(actualCompany.TipoNegocio, Is.EqualTo(tipoEsperado), $"El TipoNegocio del modelo no es el esperado. Se esperaba {tipoEsperado}");
         Assert.That(actualCompany.PaisBase, Is.EqualTo(paisEsperado), $"El PaisBase del modelo no es el esperado. Se esperaba {paisEsperado}");
         Assert.That(actualCompany.ValorEstimado, Is.EqualTo(valorEsperado), $"El ValorEstimado del modelo no es el esperado. Se esperaba {valorEsperado}");
         Assert.That(actualCompany.EsTransnacional, Is.EqualTo(esTransnacionalEsperado), $"El booleano EsTransnacional del modelo no es el esperado. Se esperaba {esTransnacionalEsperado}");
    }

    [Test]
    public async Task GetEdit_ReturnsNotFound_WithNotRealModelId()
    {
        // arrange
        int idFalso = -1;

        // action
        var result = await CompanyController.Edit(idFalso);

        // assert
        Assert.That(result, Is.TypeOf<NotFoundResult>(), "No se retorno una vista de NotFound cuando se recibio un id inválido");
    }

    #region TypeoOfCompanyAssert
    private void AssertComapanyModelType(Object model)
    {
        Assert.That(model, Is.TypeOf<CompanyModel>(), "No se retorno un modelo de tipo companyModel");
    }
    #endregion TypeoOfCompanyAssert

}