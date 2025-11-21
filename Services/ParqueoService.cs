using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Control_de_Parqueo.Models;

namespace Control_de_Parqueo.Services;

public interface IParqueoService
{
    Task<Vehiculo?> RegistrarVehiculoAsync(string placa);
    Task<Vehiculo?> ObtenerVehiculoPorPlacaAsync(string placa);
    Task<List<Vehiculo>> ObtenerTodosLosVehiculosAsync();
    Task<RegistroParqueo?> RegistrarEntradaAsync(string placa);
    Task<RegistroParqueo?> RegistrarSalidaAsync(string placa);
    Task<List<RegistroParqueo>> ObtenerRegistrosActivosAsync();
    Task<List<RegistroParqueo>> ObtenerHistorialVehiculoAsync(string placa);
    Task<List<Recompensa>> ObtenerRecompensasVehiculoAsync(string placa);
}

public class ParqueoService : IParqueoService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions;

    public ParqueoService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var supabaseUrl = configuration["Supabase:ProjectUrl"] 
            ?? throw new InvalidOperationException("Supabase ProjectUrl no configurado");
        _apiKey = configuration["Supabase:ApiKey"] 
            ?? throw new InvalidOperationException("Supabase ApiKey no configurado");
        
        _baseUrl = $"{supabaseUrl}/rest/v1";
        
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        _httpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<Vehiculo?> ObtenerVehiculoPorPlacaAsync(string placa)
    {
        try
        {
            placa = placa.ToUpper().Trim();
            var encodedPlaca = Uri.EscapeDataString(placa);
            var response = await _httpClient.GetAsync($"{_baseUrl}/vehiculos?placa=eq.{encodedPlaca}&select=*");
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content) || content == "[]")
            {
                return null;
            }

            var vehiculos = JsonSerializer.Deserialize<List<Vehiculo>>(content, _jsonOptions);
            return vehiculos?.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Vehiculo>> ObtenerTodosLosVehiculosAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/vehiculos?select=*&order=placa.asc");
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<Vehiculo>();
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content) || content == "[]")
            {
                return new List<Vehiculo>();
            }

            var vehiculos = JsonSerializer.Deserialize<List<Vehiculo>>(content, _jsonOptions);
            return vehiculos ?? new List<Vehiculo>();
        }
        catch
        {
            return new List<Vehiculo>();
        }
    }

    public async Task<Vehiculo?> RegistrarVehiculoAsync(string placa)
    {
        placa = placa.ToUpper().Trim();
        
        // Verificar si ya existe
        var existente = await ObtenerVehiculoPorPlacaAsync(placa);
        if (existente != null)
        {
            return existente;
        }

        // Crear nuevo
        var nuevoVehiculo = new
        {
            placa = placa,
            fecha_registro = DateTime.UtcNow,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(nuevoVehiculo, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/vehiculos", content);
            
            // Si hay conflicto, obtener el existente
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                await Task.Delay(500);
                return await ObtenerVehiculoPorPlacaAsync(placa);
            }
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error HTTP {response.StatusCode}: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var vehiculos = JsonSerializer.Deserialize<List<Vehiculo>>(responseContent, _jsonOptions);
            
            if (vehiculos != null && vehiculos.Any())
            {
                return vehiculos.First();
            }
            
            // Si no viene en la respuesta, obtenerlo
            await Task.Delay(500);
            return await ObtenerVehiculoPorPlacaAsync(placa);
        }
        catch
        {
            // Si falla, intentar obtenerlo por si acaso ya existe
            await Task.Delay(500);
            var vehiculo = await ObtenerVehiculoPorPlacaAsync(placa);
            if (vehiculo != null)
            {
                return vehiculo;
            }
            throw;
        }
    }

    public async Task<RegistroParqueo?> RegistrarEntradaAsync(string placa)
    {
        placa = placa.ToUpper().Trim();
        
        // Obtener o crear vehículo
        var vehiculo = await ObtenerVehiculoPorPlacaAsync(placa);
        if (vehiculo == null)
        {
            vehiculo = await RegistrarVehiculoAsync(placa);
        }

        if (vehiculo == null)
        {
            throw new Exception("No se pudo obtener o crear el vehículo");
        }

        // Crear registro de entrada
        var nuevoRegistro = new
        {
            vehiculo_id = vehiculo.Id,
            hora_entrada = DateTime.UtcNow,
            estado = "Dentro",
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(nuevoRegistro, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/registros_parqueo", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al crear registro: {response.StatusCode} - {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var registros = JsonSerializer.Deserialize<List<RegistroParqueo>>(responseContent, _jsonOptions);
        
        return registros?.FirstOrDefault();
    }

    public async Task<RegistroParqueo?> RegistrarSalidaAsync(string placa)
    {
        placa = placa.ToUpper().Trim();
        var vehiculo = await ObtenerVehiculoPorPlacaAsync(placa);
        if (vehiculo == null)
        {
            throw new Exception($"No se encontró el vehículo con placa {placa}");
        }

        // Buscar último registro activo
        var response = await _httpClient.GetAsync(
            $"{_baseUrl}/registros_parqueo?vehiculo_id=eq.{vehiculo.Id}&estado=eq.Dentro&select=*&order=hora_entrada.desc&limit=1");
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error al buscar registro: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var registros = JsonSerializer.Deserialize<List<RegistroParqueo>>(content, _jsonOptions);
        var registroActivo = registros?.FirstOrDefault();
        
        if (registroActivo == null)
        {
            throw new Exception($"No se encontró un registro activo para {placa}");
        }

        // Actualizar con hora de salida
        var updateData = new
        {
            hora_salida = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(updateData, _jsonOptions);
        var updateContent = new StringContent(json, Encoding.UTF8, "application/json");

        var patchResponse = await _httpClient.PatchAsync(
            $"{_baseUrl}/registros_parqueo?id=eq.{registroActivo.Id}", updateContent);
        
        if (!patchResponse.IsSuccessStatusCode)
        {
            var errorContent = await patchResponse.Content.ReadAsStringAsync();
            throw new Exception($"Error al actualizar salida: {patchResponse.StatusCode} - {errorContent}");
        }

        var updatedContent = await patchResponse.Content.ReadAsStringAsync();
        var updatedRegistros = JsonSerializer.Deserialize<List<RegistroParqueo>>(updatedContent, _jsonOptions);
        
        return updatedRegistros?.FirstOrDefault() ?? registroActivo;
    }

    public async Task<List<RegistroParqueo>> ObtenerRegistrosActivosAsync()
    {
        try
        {
            // Obtener registros activos
            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/registros_parqueo?estado=eq.Dentro&select=*&order=hora_entrada.desc");
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<RegistroParqueo>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var registros = JsonSerializer.Deserialize<List<RegistroParqueo>>(content, _jsonOptions);
            
            // Siempre cargar los vehículos manualmente para asegurar que estén disponibles
            if (registros != null && registros.Any())
            {
                foreach (var registro in registros)
                {
                    if (registro.VehiculoId > 0)
                    {
                        registro.Vehiculo = await ObtenerVehiculoPorIdAsync(registro.VehiculoId);
                    }
                }
            }
            
            return registros ?? new List<RegistroParqueo>();
        }
        catch (Exception ex)
        {
            // Log del error si es necesario
            return new List<RegistroParqueo>();
        }
    }
    
    private async Task<Vehiculo?> ObtenerVehiculoPorIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return null;
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/vehiculos?id=eq.{id}&select=*");
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            var vehiculos = JsonSerializer.Deserialize<List<Vehiculo>>(content, _jsonOptions);
            
            return vehiculos?.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<RegistroParqueo>> ObtenerHistorialVehiculoAsync(string placa)
    {
        try
        {
            placa = placa.ToUpper().Trim();
            var vehiculo = await ObtenerVehiculoPorPlacaAsync(placa);
            if (vehiculo == null)
            {
                return new List<RegistroParqueo>();
            }

            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/registros_parqueo?vehiculo_id=eq.{vehiculo.Id}&select=*&order=hora_entrada.desc");
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<RegistroParqueo>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var registros = JsonSerializer.Deserialize<List<RegistroParqueo>>(content, _jsonOptions);
            
            if (registros != null)
            {
                foreach (var registro in registros)
                {
                    registro.Vehiculo = vehiculo;
                }
            }
            
            return registros ?? new List<RegistroParqueo>();
        }
        catch
        {
            return new List<RegistroParqueo>();
        }
    }

    public async Task<List<Recompensa>> ObtenerRecompensasVehiculoAsync(string placa)
    {
        try
        {
            placa = placa.ToUpper().Trim();
            var vehiculo = await ObtenerVehiculoPorPlacaAsync(placa);
            if (vehiculo == null)
            {
                return new List<Recompensa>();
            }

            var response = await _httpClient.GetAsync(
                $"{_baseUrl}/recompensas?vehiculo_id=eq.{vehiculo.Id}&select=*&order=fecha_recompensa.desc");
            
            if (!response.IsSuccessStatusCode)
            {
                return new List<Recompensa>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var recompensas = JsonSerializer.Deserialize<List<Recompensa>>(content, _jsonOptions);
            
            if (recompensas != null)
            {
                foreach (var recompensa in recompensas)
                {
                    recompensa.Vehiculo = vehiculo;
                }
            }
            
            return recompensas ?? new List<Recompensa>();
        }
        catch
        {
            return new List<Recompensa>();
        }
    }
}
