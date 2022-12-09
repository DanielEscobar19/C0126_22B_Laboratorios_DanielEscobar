using app_source.Controllers;
using app_source.Data;
using app_source.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
    public async Task TearDown()
    {
        foreach (var item in EmpresasSemilla) {
            DbContext.Remove(item);
        }
        await DbContext.SaveChangesAsync();
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


    [Test]
    public async Task DeleteConfirmed_ReturnsRedirectionToIndex_WhenDeleted()
    {
        // arrange
        Helper.InsertarEmpresasSemilla(ref EmpresasSemilla, DbContext, UNIQUEID);

        // action
        var viewResult = await CompanyController.DeleteConfirmed(EmpresasSemilla[3].Id);

        // eliminamos la empresa para que el teardown elimine el resto
        EmpresasSemilla.Remove(EmpresasSemilla[3]);

        // assert
        Assert.That(viewResult, Is.TypeOf<RedirectToActionResult>());
        var redirectResult = viewResult as RedirectToActionResult;
        Assert.That(redirectResult.ActionName, Is.EqualTo("Index"), "Se esperaba la action Index");
    }

    [Test]
    public async Task DeleteConfirmed_DeletesCompanyInBase()
    {
        // arrange
        Helper.InsertarEmpresasSemilla(ref EmpresasSemilla, DbContext, UNIQUEID);

        string nombreModeloBorrado = EmpresasSemilla[1].Nombre;

        // action
        var viewResult = await CompanyController.DeleteConfirmed(EmpresasSemilla[1].Id);
        
        // eliminamos la empresa para que el teardown elimine el resto
        EmpresasSemilla.Remove(EmpresasSemilla[1]);

        // assert
        List<CompanyModel> empresasEnBase = await DbContext.CompanyModel.ToListAsync();

        // revisamos que no este en la base el que acabamos de eliminar
        Assert.IsFalse(empresasEnBase.Any(x => x.Nombre == nombreModeloBorrado));
    }

    [Test]
    public async Task PostCreate_ReturnsCreateView_WhenInvalidModel()
    {
        // arrange
        // ingresamos un puesto semilla en la base
        CompanyModel semilla = new()
        {
            Nombre  = $"Semilla Testing {UNIQUEID}"
        };

        // ingresamos datos a EmpresasSemilla para un buen teardown
        EmpresasSemilla.Add(DbContext.Add(semilla).Entity);
        // guardamos cambios en la base
        await DbContext.SaveChangesAsync();

        CompanyModel nombreRepetido = new()
        {
            Nombre = semilla.Nombre
        };

        // action
        var viewResult = await CompanyController.Create(nombreRepetido);

        // custom teardown
        List<CompanyModel> empresasEnBase = await DbContext.CompanyModel.ToListAsync();
        nombreRepetido = empresasEnBase.Find(x => x.Nombre == nombreRepetido.Nombre);
        if (nombreRepetido is not null)
        {
            EmpresasSemilla.Add(nombreRepetido);
        }

        // assert
        Assert.That(viewResult, Is.TypeOf<ViewResult>());
        var view = (ViewResult) viewResult;
        Assert.That(view.ViewData.Model, Is.TypeOf<CompanyModel>(), "Se esperaba la vista Create");
    }

    [Test]
    public async Task PostCreate_AddsCorrespondingModelToBase()
    {
        // arrange
        // copiamos los datos esperados porque el paso de modelos es por referencia
        // por lo tanto si comparamos los modelos siemrpe van a tener mismos datos porque hacen referencia al mismo
        string nombreEsperado = $"Semilla Testing {UNIQUEID}";
        string tipoEsperado = $"Negocio de testing {UNIQUEID}";
        string paisEsperado = $"Pais de testing  {UNIQUEID}";
        Decimal? valorEsperado = 45678.56m;
        bool esTransnacionalEsperado = true;

        // ingresamos un puesto semilla en la base
        CompanyModel newCompany = new()
        {
            Nombre = nombreEsperado,
            TipoNegocio = tipoEsperado,
            PaisBase = paisEsperado,
            ValorEstimado = valorEsperado,
            EsTransnacional = esTransnacionalEsperado
        };

        // action
        await CompanyController.Create(newCompany);

        // assert
        List<CompanyModel> empresasEnBase = await DbContext.CompanyModel.ToListAsync();
        CompanyModel nuevoEnBase = empresasEnBase.Find(x => x.Nombre == newCompany.Nombre);

        // custom teardown
        if (nuevoEnBase is not null)
        {
            EmpresasSemilla.Add(nuevoEnBase);
        }


        Assert.That(nuevoEnBase.Nombre, Is.EqualTo(nombreEsperado), "El nombre en la base no es el esperado");
        Assert.That(nuevoEnBase.TipoNegocio, Is.EqualTo(tipoEsperado), "El TipoNegocio en la base no es el esperado");
        Assert.That(nuevoEnBase.PaisBase, Is.EqualTo(paisEsperado), "El PaisBase en la base no es el esperado");
        Assert.That(nuevoEnBase.ValorEstimado, Is.EqualTo(valorEsperado), "El ValorEstimado en la base no es el esperado");
        Assert.That(nuevoEnBase.EsTransnacional, Is.EqualTo(esTransnacionalEsperado), "El booleano EsTransnacional en la base no es el esperado");
    }

}