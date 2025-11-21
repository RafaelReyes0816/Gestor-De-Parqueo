using System.Text.Json.Serialization;

namespace Control_de_Parqueo.Models;

public class Recompensa
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("vehiculo_id")]
    public int VehiculoId { get; set; }
    
    [JsonPropertyName("registro_parqueo_id")]
    public int RegistroParqueoId { get; set; }
    
    [JsonPropertyName("tipo_recompensa")]
    public string TipoRecompensa { get; set; } = string.Empty;
    
    [JsonPropertyName("horas_acumuladas")]
    public decimal HorasAcumuladas { get; set; }
    
    [JsonPropertyName("fecha_recompensa")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? FechaRecompensa { get; set; }
    
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; set; }
    
    // Navegación (para cuando se incluye en consultas)
    [JsonPropertyName("vehiculos")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Vehiculo? Vehiculo { get; set; }
    
    [JsonPropertyName("registros_parqueo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public RegistroParqueo? RegistroParqueo { get; set; }
}

