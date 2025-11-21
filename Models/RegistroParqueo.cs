using System.Text.Json.Serialization;

namespace Control_de_Parqueo.Models;

public class RegistroParqueo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("vehiculo_id")]
    public int VehiculoId { get; set; }
    
    [JsonPropertyName("hora_entrada")]
    public DateTime HoraEntrada { get; set; }
    
    [JsonPropertyName("hora_salida")]
    public DateTime? HoraSalida { get; set; }
    
    [JsonPropertyName("horas_totales")]
    public decimal? HorasTotales { get; set; }
    
    [JsonPropertyName("estado")]
    public string Estado { get; set; } = "Dentro";
    
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; set; }
    
    // Navegación (para cuando se incluye en consultas)
    [JsonPropertyName("vehiculos")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Vehiculo? Vehiculo { get; set; }
}

