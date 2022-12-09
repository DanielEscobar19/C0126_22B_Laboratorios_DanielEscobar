using app_source.Controllers;
using app_source.Data;
using app_source.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace unit_tests;

/// <summary>
/// Clase que testea el funcionamiento de los metodos del controlador Company
/// </summary>
public class CompanyControllerTest
{

    /// <summary>
    /// instancia para manejar la base de datos
    /// </summary>
    private CompanyContext DbContext;

    /// <summary>
    /// empresas que se ingresan en la base para teastear
    /// </summary>
    private List<CompanyModel> EmpresasSemilla = new();

    /// <summary>
    /// controlador que deseamos testear en todas las pruebas
    /// </summary>
    private CompanyController? CompanyController = null;

    /// <summary>
    /// en los test se crean empresas y para que tenga nombres unicos se les agrega este string
    /// asi los datos de testeo no chocan con otros datos que puedan haber en la base
    /// </summary>
    private const string UNIQUEID = "FXc21c02748";

    /// <summary>
    /// inicializamos la conexion con la base de datos
    /// </summary>
    [SetUp]
    public void Setup()
    {
        DbContext = Helper.DbContext;
        CompanyController = new(DbContext);
    }

    /// <summary>
    /// eliminamos las empresas semilla y cerramos los conexion con la base de datos
    /// </summary>
    [TearDown]
    public async Task TearDown()
    {
        // eliminacion de datos semilla

        foreach (var item in EmpresasSemilla) {
            DbContext.Remove(item);
        }
        await DbContext.SaveChangesAsync();
        EmpresasSemilla.Clear();

        // cerramos los conexion con la base de datos
        CompanyController = null;
    }


    /// <summary>
    /// test del metodo JsonResult IsCompanyNameAvailable(string nombre, int id)
    /// </summary>
    /// <objetivo>
    /// la prueba verifica que retorne false (nombre no disponible) cuando se esta creando una empresa con un nombre que ya existe
    /// envia un modelo con un nombre repetido
    /// </objetivo>
    /// <resultado>
    /// se retorna false porque el nombre esta repetido
    /// </resultado>
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
        // verificamos que si haya retornado un booleano el metodo
        bool? result = AssertTypeOfJsonResutl(resultadoJson);

        // verificamos que si sea false el resultado
        Assert.IsFalse(result, $"Se retorno {result} cuando el nombre no estaba disponible porque ya existe una empresa con el mismo nombre. Se espera false como resultado");
    }

    /// <summary>
    /// test del metodo JsonResult IsCompanyNameAvailable(string nombre, int id)
    /// </summary>
    /// <objetivo>
    /// la prueba verifica que retorne true (nombre disponible) cuando se esta creando una empresa con un nombre que no existe aun
    /// envia un modelo con un nombre que no esta repetido
    /// </objetivo>
    /// <resultado>
    /// se retorna true porque el nombre no existe en la base aun
    /// </resultado>
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
        // verificamos que si haya retornado un booleano el metodo
        bool? result = AssertTypeOfJsonResutl(resultadoJson);

        // verificamos que si sea true el resultado
        Assert.IsTrue(result, $"Se retorno {result} cuando el nombre no estaba disponible porque ya existe una empresa con el mismo nombre. Se espera false como resultado");
    }

    /// <summary>
    /// test del metodo JsonResult IsCompanyNameAvailable(string nombre, int id)
    /// </summary>
    /// <objetivo>
    /// la prueba verifica que retorne true (nombre disponible) cuando se esta actualizando una empresa y el nombre no es modificado pero si otro atributo
    /// envia un modelo que ya existe en la base y se actualiza el tipo de negocio pero no el nombre
    /// </objetivo>
    /// <resultado>
    /// se retorna true porque como el nombre no fue modificado si se debe pemritir la actualizacion
    /// </resultado>
    /// <remarks>   
    /// esta prueba se creo porque note que cuando se actualiza un empresa se actualiza todos los datos inclusive si no fueron modificados
    /// por lo que si no se actualizaba el nombre de la empresa, el sistema creia que no estaba disponible porque el nombre ya estaba en la base de datos
    /// se modifico el metodo para que en caso de actualizacion si permita el nombre si no se modifico y en caso de ser modificado revisa que no exista otro con ese nombre.
    /// se creo este test verifica que si se permite actualiza una empresa inclusive si no se modifica el nombre pero si otros atributos
    /// </remarks>
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
        // verificamos que si haya retornado un booleano el metodo
        bool? result = AssertTypeOfJsonResutl(resultadoJson);
        
        // verificamos que si sea true el resultado
        Assert.IsTrue(result, $"Se retorno {result} y debió ser true porque se está actualizando una empresa pero no se modifico el nombre por lo que se permite la actualización.");
    }

    /// <summary>
    /// metodo que hace assert del tipo de dato que viene en el jsonresult
    /// </summary>
    /// <remarks>
    /// si no se logra el casting a booleano el assert falla
    /// </remarks>
    /// <param name="controllerResult"></param>
    /// <returns>el booleano en cuestion</returns>
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

    /// <summary>
    /// test del metodo async Task<IActionResult> Edit(int? id)
    /// </summary>
    /// <objetivo>
    /// test que verifica que el metodo que retorna la vista de edicion y que esta contenga el modelo correpondiente del que se pidio la actualizacion
    /// se envia un modelo valido con un id que si existe en la base de datos para "atualizarlo"
    /// </objetivo>
    /// <resultado>
    /// se retorna la vista de edicion con el modelo correspiendte. El modelo que contiene la vista debe tener los mismos datos que el modelo que se envio
    /// </resultado>
    [Test]
    public async Task GetEdit_ReturnsEditViewWithCorrespondingModel()
    {
        // arrange
        // insertamos los puestos semilla de prueba
        Helper.InsertarEmpresasSemilla(ref EmpresasSemilla, DbContext, UNIQUEID);

        // copiamos los datos esperados porque el paso de modelos es por referencia
        // por lo tanto si comparamos los modelos siempre van a tener mismos datos porque hacen referencia al mismo
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

        // verificamos que el modelo si sea de tipo CompanyModel
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

    /// <summary>
    /// test del metodo async Task<IActionResult> Edit(int? id)
    /// </summary>
    /// <objetivo>
    /// test que verifica que el metodo que retorna la vista de notfound en caso de que se trate de editar un metodo que no existe
    /// se envia un id que no existe en la base. Puede ser null o -1 ya que los id siempre son mayores a 0
    /// </objetivo>
    /// <resultado>
    /// se retorna una vista de tipo NotFound porque se trato de editar un modelo que no existe
    /// </resultado>
    [Test]
    public async Task GetEdit_ReturnsNotFound_WithNotRealModelId()
    {
        // arrange
        int? idFalso = null;

        // action
        var result = await CompanyController.Edit(idFalso);
        // assert
        Assert.That(result, Is.TypeOf<ViewResult>(), "No se retorno una vista de ViewResult");
        var view = result as ViewResult;
        Assert.That(view.ViewName, Is.EqualTo("NotFound"), "No se recibió una vista de error 404");
    }
    /// <summary>
    /// metodo que permite hacer assert de un modelo.
    /// revisa que el objeto sea un CompanyModel
    /// </summary>
    /// <param name="model"></param>
    #region TypeoOfCompanyAssert
    private void AssertComapanyModelType(Object model)
    {
        Assert.That(model, Is.TypeOf<CompanyModel>(), "No se retorno un modelo de tipo companyModel");
    }
    #endregion TypeoOfCompanyAssert

    /// <summary>
    /// test del metodo async Task<IActionResult> DeleteConfirmed(int id)
    /// </summary>
    /// <objetivo>
    /// test que verifica que el metodo retorna una redireccion a la accion index
    /// se envia el id de unmodelo valido a eliminar para que se ejecute el metodo correctamente
    /// </objetivo>
    /// <resultado>
    /// se retorna una vista de tipo RedirectToActionResult y que la redicreccion sea hacia Index
    /// </resultado>
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

    /// <summary>
    /// test del metodo async Task<IActionResult> DeleteConfirmed(int id)
    /// </summary>
    /// <objetivo>
    /// test que verifica que el metodo elimine corectamente en la base el modelo
    /// se envia el id de un modelo valido a eliminar para que se ejecute el metodo correctamente
    /// </objetivo>
    /// <resultado>
    /// elmodelo eliminado ya no esta en la base y la cantidad de elementos en la base se reduce de 1
    /// </resultado>
    [Test]
    public async Task DeleteConfirmed_DeletesCompanyInBase()
    {
        // arrange
        Helper.InsertarEmpresasSemilla(ref EmpresasSemilla, DbContext, UNIQUEID);

        string nombreModeloBorrado = EmpresasSemilla[1].Nombre;

        List<CompanyModel> empresasEnBase = await DbContext.CompanyModel.ToListAsync();

        // alamacenamos la cantidad de elementos antes de eliminar el nuevo
        int cantidadPreEliminacion = empresasEnBase.Count;
        // action
        var viewResult = await CompanyController.DeleteConfirmed(EmpresasSemilla[1].Id);
        
        // eliminamos la empresa para que el teardown elimine el resto
        EmpresasSemilla.Remove(EmpresasSemilla[1]);

        // assert
        empresasEnBase = await DbContext.CompanyModel.ToListAsync();

        // revisamos que no este en la base el que acabamos de eliminar
        Assert.IsFalse(empresasEnBase.Any(x => x.Nombre == nombreModeloBorrado));

        // revisamos que la cantidad de elementos en la base sea cantidadPreEliminacion - 1 ya que se elimino un solo elemento
        Assert.That(empresasEnBase.Count, Is.EqualTo(cantidadPreEliminacion - 1), "Se eliminó más de una empresa en la base");
    }

    /// <summary>
    /// test del metodo async Task<IActionResult> Create([Bind("Id,Nombre,TipoNegocio,PaisBase,ValorEstimado,EsTransnacional")] CompanyModel companyModel)
    /// </summary>
    /// <objetivo>
    /// test verifica que si se ingresa algun valor erroneo al crear una empresa se devuelve la misma vista y no el index
    /// se envia una empresa con un nombre que ya existe en la base para que el modelo sea invalido
    /// </objetivo>
    /// <resultado>
    /// se espera que se retorne la misma vista de create con el modelo y no otra
    /// </resultado>
    [Test]
    public async Task PostCreate_ReturnsCreateView_WhenInvalidModel()
    {
        // arrange
        // ingresamos una empresa semilla en la base
        CompanyModel semilla = new()
        {
            Nombre  = $"Semilla Testing {UNIQUEID}"
        };

        // ingresamos datos a EmpresasSemilla para un buen teardown
        EmpresasSemilla.Add(DbContext.Add(semilla).Entity);
        // guardamos cambios en la base
        await DbContext.SaveChangesAsync();

        // creamos un modelo con el nombre repetido para que sea invalido
        CompanyModel nombreRepetido = new()
        {
            Nombre = semilla.Nombre
        };

        // action
        var viewResult = await CompanyController.Create(nombreRepetido);

        // assert
        // custom teardown
        // agregamos a EmpresasSemilla el modelo nombreRepetido en caso de que si sea haya creado
        // asi el teardown se encarga de eliminarlo
        List<CompanyModel> empresasEnBase = await DbContext.CompanyModel.ToListAsync();
        nombreRepetido = empresasEnBase.Find(x => x.Nombre == nombreRepetido.Nombre);
        if (nombreRepetido is not null)
        {
            EmpresasSemilla.Add(nombreRepetido);
        }
        // fin custom teardown


        // revisamos que sea una bvista lo que retorna
        Assert.That(viewResult, Is.TypeOf<ViewResult>());
        var view = (ViewResult) viewResult;
        // por ser una operacion post la vista no retorna el nombre del cshtml utilizado
        // revisamos que la vista tenga el modelo CompanyModel
        // asi sabemos que es la vista de crear
        Assert.That(view.ViewData.Model, Is.TypeOf<CompanyModel>(), "Se esperaba la vista Create");
    }

    /// <summary>
    /// test del metodo async Task<IActionResult> Create([Bind("Id,Nombre,TipoNegocio,PaisBase,ValorEstimado,EsTransnacional")] CompanyModel companyModel)
    /// </summary>
    /// <objetivo>
    /// test verifica que al ingresar el modelo si se ingrese en la base de datos correctamente
    /// se envia un modelo valido
    /// </objetivo>
    /// <resultado>
    /// se espera que en la base se encuntre el modelo ingresado con los mismos datos
    /// </resultado>
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

        // creamos modelo valido a ingresar en la base
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
        // fin custom teardown

        // revismas que el modelo en base sea el mismo que el ingresado
        Assert.That(nuevoEnBase.Nombre, Is.EqualTo(nombreEsperado), "El nombre en la base no es el esperado");
        Assert.That(nuevoEnBase.TipoNegocio, Is.EqualTo(tipoEsperado), "El TipoNegocio en la base no es el esperado");
        Assert.That(nuevoEnBase.PaisBase, Is.EqualTo(paisEsperado), "El PaisBase en la base no es el esperado");
        Assert.That(nuevoEnBase.ValorEstimado, Is.EqualTo(valorEsperado), "El ValorEstimado en la base no es el esperado");
        Assert.That(nuevoEnBase.EsTransnacional, Is.EqualTo(esTransnacionalEsperado), "El booleano EsTransnacional en la base no es el esperado");
    }

}