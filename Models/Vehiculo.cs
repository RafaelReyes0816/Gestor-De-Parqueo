using System.Text.Json.Serialization;

namespace Control_de_Parqueo.Models;

public class Vehiculo
{
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }
    
    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
    
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; set; }
    
    // Navegación (para cuando se incluye en consultas)
    [JsonPropertyName("registros_parqueo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<RegistroParqueo>? RegistrosParqueo { get; set; }
    
    [JsonPropertyName("recompensas")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Recompensa>? Recompensas { get; set; }
}

