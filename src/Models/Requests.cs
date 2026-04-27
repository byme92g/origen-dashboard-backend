using OrigenDashboard.Models.Enums;

namespace OrigenDashboard.Models;

// Auth
public record LoginRequest(string NombreUsuario, string Password);

// Usuarios
public record CrearUsuarioRequest(string NombreCompleto, string NombreUsuario, string Password, RolUsuario Rol);
public record ActualizarUsuarioRequest(string NombreCompleto, string? Password, RolUsuario Rol, bool Activo);

// Empleados
public record CrearEmpleadoRequest(string Nombre, string Cargo, decimal ComisionPct, string? UsuarioLogin);
public record ActualizarEmpleadoRequest(string Nombre, string Cargo, decimal ComisionPct, string? UsuarioLogin, bool Activo);

// Servicios
public record CrearServicioRequest(string Nombre, string Categoria, decimal Precio, int DuracionMin, decimal ComisionPct);
public record ActualizarServicioRequest(string Nombre, string Categoria, decimal Precio, int DuracionMin, decimal ComisionPct, bool Activo);

// Productos
public record CrearProductoRequest(string Nombre, string Categoria, decimal PrecioVenta, int StockInicial);
public record ActualizarProductoRequest(string Nombre, string Categoria, decimal PrecioVenta, int Stock, bool Activo);

// Paquetes
public record CrearPaqueteRequest(
    string Nombre, string? Descripcion, decimal Precio, decimal Descuento,
    List<int> ServicioIds, List<int> ProductoIds, decimal ComisionPct = 0);

public record ActualizarPaqueteRequest(
    string Nombre, string? Descripcion, decimal Precio, decimal Descuento, bool Activo,
    List<int> ServicioIds, List<int> ProductoIds, decimal ComisionPct = 0);

// Clientes
public record CrearClienteRequest(string Nombre, string? Telefono, string? Email, string? Observaciones);
public record ActualizarClienteRequest(string Nombre, string? Telefono, string? Email, string? Observaciones);

// Ingresos
public record CrearIngresoRequest(
    DateTime Fecha, int? ClienteId, int? EmpleadoId,
    TipoIngreso Tipo, int? ServicioId, int? ProductoId, int? PaqueteId,
    string? ConceptoPersonalizado, decimal Monto, decimal Descuento,
    MetodoPago MetodoPago, string? Referencia, decimal Comision,
    string? Observaciones, int Cantidad = 1);

// Egresos
public record CrearEgresoRequest(
    DateTime Fecha, int CategoriaId, string Descripcion,
    decimal Monto, string? Proveedor, string? Comprobante, string? Observaciones);

public record CrearCategoriaRequest(string Nombre, TipoCategoria Tipo);
public record ActualizarCategoriaRequest(string Nombre, TipoCategoria Tipo, bool Activo);

// Caja
public record AbrirCajaRequest(decimal MontoInicial, string? Responsables);
public record CerrarCajaRequest(decimal TotalIngresos, decimal TotalEgresos, decimal SaldoFinal, string? Observaciones);

// Reportes
public record ReporteRequest(DateTime Desde, DateTime Hasta);

// Paginación
public record PagedResult<T>(IEnumerable<T> Items, int Total, int Page, int PageSize);
