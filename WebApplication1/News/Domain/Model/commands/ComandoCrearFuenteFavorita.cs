// Define el espacio de nombres para los comandos del dominio de noticias

namespace WebApplication1.News.Domain.Model.commands;

/// <summary>
///     Comando para crear una fuente de noticias favorita
///     (Como un "papelito de pedido" para guardar tus fuentes favoritas)
/// </summary>
/// <param name="ClaveApiNoticias">La clave de API obtenida del proveedor de noticias (como tu llave de acceso)</param>
/// <param name="IdFuente">El identificador único de la fuente de noticias que quieres guardar como favorita</param>

public record ComandoCrearFuenteFavorita(
    string ClaveApiNoticias, 
    string IdFuente
);