using System.Text.Json.Serialization;

namespace Control_de_Parqueo.Models;

public class Vehiculo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
    
    [JsonPropertyName("fecha_registro")]
    public DateTime FechaRegistro { get; set; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    // Navegación (para cuando se incluye en consultas)
    [JsonPropertyName("registros_parqueo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<RegistroParqueo>? RegistrosParqueo { get; set; }
    
    [JsonPropertyName("recompensas")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Recompensa>? Recompensas { get; set; }
}

