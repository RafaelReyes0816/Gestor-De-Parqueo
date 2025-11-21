using Microsoft.AspNetCore.Mvc;
using Control_de_Parqueo.Models;
using Control_de_Parqueo.Services;

namespace Control_de_Parqueo.Controllers;

public class ParqueoController : Controller
{
    private readonly IParqueoService _parqueoService;
    private readonly ILogger<ParqueoController> _logger;

    public ParqueoController(IParqueoService parqueoService, ILogger<ParqueoController> logger)
    {
        _parqueoService = parqueoService;
        _logger = logger;
    }

    // GET: Parqueo/Entrada
    public IActionResult Entrada()
    {
        return View();
    }

    // POST: Parqueo/Entrada
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Entrada(string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
        {
            TempData["Error"] = "Por favor, ingrese un número de placa válido.";
            return View();
        }

        try
        {
            var registro = await _parqueoService.RegistrarEntradaAsync(placa);
            
            if (registro != null)
            {
                TempData["Success"] = $"Vehículo con placa {placa.ToUpper()} registrado con entrada exitosamente.";
                TempData["HoraEntrada"] = registro.HoraEntrada.ToString("dd/MM/yyyy HH:mm:ss");
            }
            else
            {
                TempData["Error"] = "No se pudo registrar la entrada del vehículo.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar entrada: {Message}", ex.Message);
            TempData["Error"] = $"Error al registrar la entrada: {ex.Message}";
            
            // En desarrollo, mostrar más detalles
            if (HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                TempData["ErrorDetails"] = ex.ToString();
            }
        }

        return View();
    }

    // GET: Parqueo/Salida
    public async Task<IActionResult> Salida()
    {
        try
        {
            var registrosActivos = await _parqueoService.ObtenerRegistrosActivosAsync();
            return View(registrosActivos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener registros activos");
            return View(new List<RegistroParqueo>());
        }
    }

    // POST: Parqueo/Salida
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salida(string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
        {
            TempData["Error"] = "Por favor, ingrese un número de placa válido.";
            return View();
        }

        try
        {
            var registro = await _parqueoService.RegistrarSalidaAsync(placa);
            
            if (registro != null)
            {
                var horas = registro.HorasTotales ?? 0;
                TempData["Success"] = $"Vehículo con placa {placa.ToUpper()} registrado con salida exitosamente.";
                TempData["HoraSalida"] = registro.HoraSalida?.ToString("dd/MM/yyyy HH:mm:ss");
                TempData["HorasTotales"] = horas.ToString("F2");
                
                if (horas > 10)
                {
                    TempData["Recompensa"] = $"¡Felicidades! El vehículo permaneció {horas:F2} horas. Ha recibido una recompensa.";
                }
            }
            else
            {
                TempData["Error"] = "No se encontró un registro activo para este vehículo o el vehículo no existe.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar salida");
            TempData["Error"] = "Ocurrió un error al registrar la salida. Por favor, intente nuevamente.";
        }

        return View();
    }

    // GET: Parqueo/Activos
    public async Task<IActionResult> Activos()
    {
        try
        {
            var registros = await _parqueoService.ObtenerRegistrosActivosAsync();
            return View(registros);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener registros activos");
            TempData["Error"] = "Ocurrió un error al cargar los vehículos activos.";
            return View(new List<RegistroParqueo>());
        }
    }

    // GET: Parqueo/Historial
    public async Task<IActionResult> Historial()
    {
        try
        {
            var vehiculos = await _parqueoService.ObtenerTodosLosVehiculosAsync();
            ViewBag.Vehiculos = vehiculos;
            return View(new List<RegistroParqueo>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener vehículos");
            ViewBag.Vehiculos = new List<Vehiculo>();
            return View(new List<RegistroParqueo>());
        }
    }

    // POST: Parqueo/Historial
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Historial(string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
        {
            TempData["Error"] = "Por favor, ingrese un número de placa válido.";
            var vehiculos = await _parqueoService.ObtenerTodosLosVehiculosAsync();
            ViewBag.Vehiculos = vehiculos;
            return View(new List<RegistroParqueo>());
        }

        try
        {
            var historial = await _parqueoService.ObtenerHistorialVehiculoAsync(placa);
            var recompensas = await _parqueoService.ObtenerRecompensasVehiculoAsync(placa);
            var vehiculos = await _parqueoService.ObtenerTodosLosVehiculosAsync();
            
            ViewBag.Placa = placa.ToUpper();
            ViewBag.Recompensas = recompensas;
            ViewBag.Vehiculos = vehiculos;
            
            return View(historial);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial");
            TempData["Error"] = "Ocurrió un error al obtener el historial. Por favor, intente nuevamente.";
            var vehiculos = await _parqueoService.ObtenerTodosLosVehiculosAsync();
            ViewBag.Vehiculos = vehiculos;
            return View(new List<RegistroParqueo>());
        }
    }

    // GET: Parqueo/HistorialJson?placa=XXX
    [HttpGet]
    public async Task<IActionResult> HistorialJson(string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
        {
            return Json(new { success = false, message = "Placa no válida" });
        }

        try
        {
            var historial = await _parqueoService.ObtenerHistorialVehiculoAsync(placa);
            var recompensas = await _parqueoService.ObtenerRecompensasVehiculoAsync(placa);
            
            return Json(new 
            { 
                success = true,
                placa = placa.ToUpper(),
                historial = historial,
                recompensas = recompensas,
                totalRegistros = historial.Count,
                totalHoras = historial.Where(r => r.HorasTotales.HasValue).Sum(r => r.HorasTotales.Value),
                totalRecompensas = recompensas.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial JSON");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // POST: Parqueo/EntradaJson
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> EntradaJson([FromBody] EntradaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Placa))
        {
            return Json(new { success = false, message = "Placa no válida" });
        }

        try
        {
            var registro = await _parqueoService.RegistrarEntradaAsync(request.Placa);
            
            if (registro != null)
            {
                return Json(new 
                { 
                    success = true,
                    message = $"Vehículo con placa {request.Placa.ToUpper()} registrado con entrada exitosamente.",
                    horaEntrada = registro.HoraEntrada.ToString("dd/MM/yyyy HH:mm:ss"),
                    placa = request.Placa.ToUpper()
                });
            }
            else
            {
                return Json(new { success = false, message = "No se pudo registrar la entrada del vehículo." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar entrada JSON");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // POST: Parqueo/SalidaJson
    [HttpPost]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SalidaJson([FromBody] SalidaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Placa))
        {
            return Json(new { success = false, message = "Placa no válida" });
        }

        try
        {
            var registro = await _parqueoService.RegistrarSalidaAsync(request.Placa);
            
            if (registro != null)
            {
                var horas = registro.HorasTotales ?? 0;
                var recompensa = horas > 10 ? $"¡Felicidades! El vehículo permaneció {horas:F2} horas. Ha recibido una recompensa." : null;
                
                return Json(new 
                { 
                    success = true,
                    message = $"Vehículo con placa {request.Placa.ToUpper()} registrado con salida exitosamente.",
                    horaSalida = registro.HoraSalida?.ToString("dd/MM/yyyy HH:mm:ss"),
                    horasTotales = horas.ToString("F2"),
                    recompensa = recompensa,
                    placa = request.Placa.ToUpper()
                });
            }
            else
            {
                return Json(new { success = false, message = "No se encontró un registro activo para este vehículo o el vehículo no existe." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar salida JSON");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    // GET: Parqueo/ActivosJson
    [HttpGet]
    public async Task<IActionResult> ActivosJson()
    {
        try
        {
            var registros = await _parqueoService.ObtenerRegistrosActivosAsync();
            return Json(new { success = true, registros = registros });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener registros activos JSON");
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }
}

public class EntradaRequest
{
    public string Placa { get; set; } = string.Empty;
}

public class SalidaRequest
{
    public string Placa { get; set; } = string.Empty;
}

