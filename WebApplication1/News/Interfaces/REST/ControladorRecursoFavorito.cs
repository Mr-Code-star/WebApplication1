using System.Net.Mime;
using CatchUpPlatform.API.News.Domain.Model.Queries;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebApplication1.News.Domain.Model.queries;
using WebApplication1.News.Domain.Services;
using WebApplication1.News.Interfaces.REST.Resources;
using WebApplication1.News.Interfaces.REST.Transform;

namespace WebApplication1.News.Interfaces.REST;

/// <summary>
/// Controlador de fuentes favoritas.
/// </summary>
/// <param name="servicioComandoFuenteFavorita">El Servicio de Comando de Fuente Favorita</param>
/// <param name="servicioConsultaFuenteFavorita">El Servicio de Consulta de Fuente Favorita</param>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Fuentes Favoritas")]
public class ControladorFuentesFavoritas(
    IServicioComandoFuenteFavorita servicioComandoFuenteFavorita,
    IServicioConsultaFuenteFavorita servicioConsultaFuenteFavorita)
    : ControllerBase
{
    /// <summary>
    /// Crea una fuente favorita.
    /// </summary>
    /// <param name="recurso">Recurso RecursoCrearFuenteFavorita</param>
    /// <returns>
    /// Una respuesta como resultado de acción que contiene la fuente favorita creada, o bad request si no se creó.
    /// </returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Crea una fuente favorita",
        Description = "Crea una fuente favorita con una Clave API de Noticias y un ID de Fuente dados",
        OperationId = "CrearFuenteFavorita")]
    [SwaggerResponse(201, "La fuente favorita fue creada", typeof(RecursoFuenteFavorita))]
    [SwaggerResponse(400, "La fuente favorita no fue creada")]
    public async Task<ActionResult> CrearFuenteFavorita([FromBody] RecursoCrearFuenteFavorita recurso)
    {
        // 🔄 CONVIERTE el recurso del cliente a comando del dominio
        var comandoCrearFuenteFavorita = EnsambladorComandoCrearFuenteFavoritaDesdeRecurso.AComandoDesdeRecurso(recurso);
        
        // 🏗️ EJECUTA el comando en el servicio de dominio
        var resultado = await servicioComandoFuenteFavorita.Manejar(comandoCrearFuenteFavorita);
        
        // ❌ SI falla, devuelve error
        if (resultado is null) return BadRequest();
        
        // ✅ SI éxito, convierte la entidad a recurso y devuelve respuesta 201
        return CreatedAtAction(
            nameof(ObtenerFuenteFavoritaPorId), 
            new { id = resultado.Id }, 
            EnsambladorRecursoFuenteFavoritaDesdeEntidad.ARecursoDesdeEntidad(resultado));
    }
    
    /// <summary>
    /// Obtiene todas las fuentes favoritas por clave API de noticias.
    /// </summary>
    /// <param name="claveApiNoticias">La Clave API de Noticias generada por el proveedor de servicios</param>
    /// <returns>
    /// Una respuesta como resultado de acción que contiene las fuentes favoritas, o no encontrado si no se encontraron fuentes favoritas.
    /// </returns>
    private async Task<ActionResult> ObtenerTodasLasFuentesFavoritasPorClaveApiNoticias(string claveApiNoticias)
    {
        // 🔍 CREA la consulta para buscar todas las fuentes de un usuario
        var consultaObtenerTodasLasFuentesFavoritasPorClaveApi = new ConsultaObtenerFuentesFavoritasPorClaveApi(claveApiNoticias);
        
        // 📊 EJECUTA la consulta en el servicio
        var resultado = await servicioConsultaFuenteFavorita.Manejar(consultaObtenerTodasLasFuentesFavoritasPorClaveApi);
        
        // 🔄 CONVIERTE todas las entidades a recursos
        var recursos = resultado.Select(EnsambladorRecursoFuenteFavoritaDesdeEntidad.ARecursoDesdeEntidad);
        
        // 📤 DEVUELVE la lista de recursos
        return Ok(recursos);
    }
    
    /// <summary>
    /// Obtiene una fuente favorita por clave API de noticias e ID de fuente.
    /// </summary>
    /// <param name="claveApiNoticias">La Clave API de Noticias generada por el proveedor de servicios de noticias</param>
    /// <param name="idFuente">El ID de Fuente del proveedor de servicios de noticias</param>
    /// <returns></returns>
    private async Task<ActionResult> ObtenerFuenteFavoritaPorClaveApiNoticiasYIdFuente(string claveApiNoticias, string idFuente)
    {
        // 🔍 CREA la consulta para buscar una fuente específica
        var consultaObtenerFuenteFavoritaPorClaveApiYIdFuente = new ConsultaObtenerFuenteFavoritaPorClaveApiYIdFuente(claveApiNoticias, idFuente);
        
        // 📊 EJECUTA la consulta en el servicio
        var resultado = await servicioConsultaFuenteFavorita.Manejar(consultaObtenerFuenteFavoritaPorClaveApiYIdFuente);
        
        // ❌ SI no encuentra, devuelve 404
        if (resultado is null) return NotFound();
        
        // 🔄 CONVIERTE la entidad a recurso
        var recurso = EnsambladorRecursoFuenteFavoritaDesdeEntidad.ARecursoDesdeEntidad(resultado);
        
        // 📤 DEVUELVE el recurso
        return Ok(recurso);
    }
    
    /// <summary>
    /// Obtiene una fuente favorita según los parámetros.
    /// </summary>
    /// <param name="claveApiNoticias">La Clave API de Noticias generada por el proveedor de servicios de noticias</param>
    /// <param name="idFuente">El ID de Fuente del proveedor de servicios de noticias</param>
    /// <returns>
    /// Una respuesta como resultado de acción que contiene la fuente favorita si se proporcionan la Clave API de Noticias y el ID de Fuente,
    /// o una respuesta con las fuentes favoritas para la Clave API de Noticias si no se proporciona el ID de Fuente.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Obtiene una fuente favorita según los parámetros",
        Description = "Obtiene una fuente favorita para los parámetros dados",
        OperationId = "ObtenerFuenteFavoritaDesdeConsulta")]
    [SwaggerResponse(200, "Se encontró el/los resultado(s)", typeof(RecursoFuenteFavorita))]
    public async Task<ActionResult> ObtenerFuenteFavoritaDesdeConsulta([FromQuery] string claveApiNoticias, [FromQuery] string idFuente = "")
    {
        // 🎯 DECIDE qué búsqueda hacer según los parámetros:
        // - Si NO hay idFuente: Busca TODAS las fuentes del usuario
        // - Si SÍ hay idFuente: Busca UNA fuente específica
        return string.IsNullOrEmpty(idFuente)
            ? await ObtenerTodasLasFuentesFavoritasPorClaveApiNoticias(claveApiNoticias)
            : await ObtenerFuenteFavoritaPorClaveApiNoticiasYIdFuente(claveApiNoticias, idFuente);
    }
    
    /// <summary>
    /// Obtiene una fuente favorita por ID.
    /// </summary>
    /// <param name="id">El ID de la Fuente Favorita</param>
    /// <returns>
    /// Una respuesta como resultado de acción que contiene la fuente favorita, o no encontrado si no se encontró la fuente favorita.
    /// </returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obtiene una fuente favorita por ID",
        Description = "Obtiene una fuente favorita para un identificador de fuente favorita dado",
        OperationId = "ObtenerFuenteFavoritaPorId")]
    [SwaggerResponse(200, "La fuente favorita fue encontrada", typeof(RecursoFuenteFavorita))]
    public async Task<ActionResult> ObtenerFuenteFavoritaPorId(int id)
    {
        // 🔍 CREA la consulta para buscar por ID numérico
        var consultaObtenerFuenteFavoritaPorId = new ConsultaObtenerFuenteFavoritaPorId(id);
        
        // 📊 EJECUTA la consulta en el servicio
        var resultado = await servicioConsultaFuenteFavorita.Manejar(consultaObtenerFuenteFavoritaPorId);
        
        // ❌ SI no encuentra, devuelve 404
        if (resultado is null) return NotFound();
        
        // 🔄 CONVIERTE la entidad a recurso
        var recurso = EnsambladorRecursoFuenteFavoritaDesdeEntidad.ARecursoDesdeEntidad(resultado);
        
        // 📤 DEVUELVE el recurso
        return Ok(recurso);
    }
}