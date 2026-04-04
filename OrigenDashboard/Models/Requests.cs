namespace OrigenDashboard.Models;

// --- Auth ---
public record LoginRequest(
    string NombreUsuario,
    string Password
);

// --- Usuarios ---
public record CrearUsuarioRequest(
    string NombreCompleto,
    string NombreUsuario,
    string Password,
    string Rol // admin | empleado
);

public record ActualizarUsuarioRequest(
    string NombreCompleto,
    string? Password, // null = no cambiar
    string Rol,
    bool Activo
);

// --- Empleados ---
public record CrearEmpleadoRequest(
    string Nombre,
    string Cargo,
    decimal ComisionPct,
    string? UsuarioLogin
);

public record ActualizarEmpleadoRequest(
    string Nombre,
    string Cargo,
    decimal ComisionPct,
    string? UsuarioLogin,
    bool Activo
);

// --- Servicios ---
public record CrearServicioRequest(
    string Nombre,
    string Categoria,
    decimal Precio,
    int DuracionMin,
    decimal ComisionPct
);

public record ActualizarServicioRequest(
    string Nombre,
    string Categoria,
    decimal Precio,
    int DuracionMin,
    decimal ComisionPct,
    bool Activo
);

// --- Productos ---
public record CrearProductoRequest(
    string Nombre,
    string Categoria,
    decimal PrecioVenta,
    int StockInicial
);

public record ActualizarProductoRequest(
    string Nombre,
    string Categoria,
    decimal PrecioVenta,
    int Stock,
    bool Activo
);

// --- Paquetes ---
public record CrearPaqueteRequest(
    string Nombre,
    string? Descripcion,
    decimal Precio,
    decimal Descuento,
    List<int> ServicioIds
);

public record ActualizarPaqueteRequest(
    string Nombre,
    string? Descripcion,
    decimal Precio,
    decimal Descuento,
    bool Activo,
    List<int> ServicioIds
);

// --- Clientes ---
public record CrearClienteRequest(
    string Nombre,
    string? Telefono,
    string? Email,
    string? Observaciones
);

public record ActualizarClienteRequest(
    string Nombre,
    string? Telefono,
    string? Email,
    string? Observaciones
);

// --- Ingresos ---
public record CrearIngresoRequest(
    DateTime Fecha,
    int? ClienteId,
    int? EmpleadoId,
    string Tipo, // servicio | producto | paquete | personalizado
    int? ServicioId,
    int? ProductoId,
    int? PaqueteId,
    string? ConceptoPersonalizado,
    decimal Monto,
    decimal Descuento,
    string MetodoPago, // efectivo | transferencia | pos | yape | plin | otro
    string? Referencia,
    decimal Comision,
    string? Observaciones
);

// --- Egresos ---
public record CrearEgresoRequest(
    DateTime Fecha,
    string Categoria,
    string Descripcion,
    decimal Monto,
    string? Proveedor,
    string? Comprobante,
    string? Observaciones
);

// --- Reportes ---
public record ReporteRequest(
    DateTime Desde,
    DateTime Hasta
);
