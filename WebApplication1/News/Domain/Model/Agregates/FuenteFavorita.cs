using WebApplication1.News.Domain.Model.commands;

namespace WebApplication1.News.Domain.Model.Agregates;

/// <summary>
///     Agregando FuenteFavorita
///     Esta clase representa la ficha de una fuente favorita guardada
/// </summary>
 
public partial class FuenteFavorita
{
    // Constructor vacio protegido (Para Entity FrameWork)
    
    protected FuenteFavorita()
    {
        ClaveApiNoticias = string.Empty;
        IdFuente = string.Empty;
    }

    /// <summary>
    /// Constructor principal para crear una fuentefavorita
    /// (ua el "papelito de pedido" para crear la ficha)
    /// </summary>
    /// <para name="comando"> El comando para crear fuentes favoritas</para>
    
    public FuenteFavorita(ComandoCrearFuenteFavorita comando)
    {
        ClaveApiNoticias = comando.ClaveApiNoticias;
        IdFuente = comando.IdFuente;
    }
    
    public int Id { get; } // Número único automático (como ID de base de datos)
    public string IdFuente { get; private set; } // Tu llave de acceso

    public string ClaveApiNoticias { get; private set; } // Codigo de la fuente (ej: "bbc-news")
    
}