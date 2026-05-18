using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using monster.edu.ec.datos;
using monster.edu.ec.modelo;

namespace monster.edu.ec.controlador;

[ApiController]
[Route("api/eureka")]
public class EurekaBankController : ControllerBase
{
    private readonly AppDbContext _context;

    public EurekaBankController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>Autentica un empleado usando usuario y password (BCrypt).</summary>
    [HttpPost("autenticar")]
    public async Task<IActionResult> AutenticarUsuario([FromBody] AuthRequest request)
    {
        var empleado = await _context.Empleados
            .FirstOrDefaultAsync(e => e.Usuario == request.Usuario);

        if (empleado == null || !BCrypt.Net.BCrypt.Verify(request.Clave, empleado.Password))
            return Ok(new { resultado = "Credenciales incorrectas" });

        return Ok(new { resultado = "OK", rol = empleado.Rol, nombre = empleado.NombreCompleto, idEmpleado = empleado.IdEmpleado });
    }

    /// <summary>Realiza un depósito en la cuenta especificada.</summary>
    [HttpPost("depositar")]
    public async Task<IActionResult> Depositar([FromBody] DepositarRequest request)
    {
        using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var cuenta = await _context.Cuentas.FindAsync(request.CuentaId);
            if (cuenta == null)
                return Ok(new { resultado = "Cuenta no encontrada" });

            if (cuenta.Estado != "Activa")
                return Ok(new { resultado = "La cuenta está bloqueada" });

            cuenta.Saldo += (decimal)request.Monto;

            _context.Movimientos.Add(new Movimiento
            {
                NumeroOperacion = $"DEP-{DateTime.Now:yyyyMMddHHmmss}",
                NumeroCuenta    = request.CuentaId,
                IdEmpleado      = request.EmpleadoId,
                Tipo            = "Deposito",
                Monto           = (decimal)request.Monto,
                FechaHora       = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return Ok(new { resultado = "Depósito realizado correctamente" });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return Ok(new { resultado = $"Error: {ex.Message}" });
        }
    }

    /// <summary>Realiza un retiro de la cuenta especificada.</summary>
    [HttpPost("retirar")]
    public async Task<IActionResult> Retirar([FromBody] RetirarRequest request)
    {
        using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var cuenta = await _context.Cuentas.FindAsync(request.CuentaId);
            if (cuenta == null)
                return Ok(new { resultado = "Cuenta no encontrada" });

            if (cuenta.Estado != "Activa")
                return Ok(new { resultado = "La cuenta está bloqueada" });

            if (cuenta.Saldo < (decimal)request.Monto)
                return Ok(new { resultado = "Saldo insuficiente" });

            cuenta.Saldo -= (decimal)request.Monto;

            _context.Movimientos.Add(new Movimiento
            {
                NumeroOperacion = $"RET-{DateTime.Now:yyyyMMddHHmmss}",
                NumeroCuenta    = request.CuentaId,
                IdEmpleado      = request.EmpleadoId,
                Tipo            = "Retiro",
                Monto           = (decimal)request.Monto,
                FechaHora       = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return Ok(new { resultado = "Retiro realizado correctamente" });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return Ok(new { resultado = $"Error: {ex.Message}" });
        }
    }

    /// <summary>Transfiere fondos entre dos cuentas (Retiro + Depósito).</summary>
    [HttpPost("transferir")]
    public async Task<IActionResult> Transferir([FromBody] TransferirRequest request)
    {
        using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            var cuentaOrigen  = await _context.Cuentas.FindAsync(request.CuentaOrigenId);
            var cuentaDestino = await _context.Cuentas.FindAsync(request.CuentaDestinoId);

            if (cuentaOrigen == null)
                return Ok(new { resultado = "Cuenta origen no encontrada" });
            if (cuentaDestino == null)
                return Ok(new { resultado = "Cuenta destino no encontrada" });
            if (cuentaOrigen.Estado != "Activa")
                return Ok(new { resultado = "La cuenta origen está bloqueada" });
            if (cuentaDestino.Estado != "Activa")
                return Ok(new { resultado = "La cuenta destino está bloqueada" });
            if (cuentaOrigen.Saldo < (decimal)request.Monto)
                return Ok(new { resultado = "Saldo insuficiente en cuenta origen" });

            string ts = DateTime.Now.ToString("yyyyMMddHHmmss");

            cuentaOrigen.Saldo  -= (decimal)request.Monto;
            cuentaDestino.Saldo += (decimal)request.Monto;

            _context.Movimientos.Add(new Movimiento
            {
                NumeroOperacion = $"TRF-SAL-{ts}",
                NumeroCuenta    = request.CuentaOrigenId,
                IdEmpleado      = request.EmpleadoId,
                Tipo            = "Retiro",
                Monto           = (decimal)request.Monto,
                FechaHora       = DateTime.Now
            });

            _context.Movimientos.Add(new Movimiento
            {
                NumeroOperacion = $"TRF-ENT-{ts}",
                NumeroCuenta    = request.CuentaDestinoId,
                IdEmpleado      = request.EmpleadoId,
                Tipo            = "Deposito",
                Monto           = (decimal)request.Monto,
                FechaHora       = DateTime.Now
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            return Ok(new { resultado = "Transferencia realizada correctamente" });
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            return Ok(new { resultado = $"Error: {ex.Message}" });
        }
    }

    /// <summary>Lista todas las cuentas de una sucursal con datos del cliente.</summary>
    [HttpGet("cuentas/sucursal/{sucursalId}")]
    public async Task<IActionResult> ListarCuentasPorSucursal(int sucursalId)
    {
        var cuentas = await _context.Cuentas
            .Include(c => c.Cliente)
            .Where(c => c.IdSucursal == sucursalId)
            .Select(c => new CuentaClienteDTO
            {
                NumeroCuenta    = c.NumeroCuenta,
                Saldo           = c.Saldo,
                IdCliente       = c.IdCliente,
                NombreCliente   = c.Cliente != null ? c.Cliente.Nombre   : string.Empty,
                ApellidoCliente = c.Cliente != null ? c.Cliente.Apellido : string.Empty,
                Disponibilidad  = c.Estado
            })
            .ToListAsync();

        return Ok(cuentas);
    }

    /// <summary>Consulta el extracto (movimientos) de una cuenta.</summary>
    [HttpGet("extracto/{cuentaId}")]
    public async Task<IActionResult> ConsultarExtracto(string cuentaId)
    {
        var movimientos = await _context.Movimientos
            .Include(m => m.Empleado)
            .Where(m => m.NumeroCuenta == cuentaId)
            .OrderByDescending(m => m.FechaHora)
            .Select(m => new MovimientoDTO
            {
                IdMovimiento    = m.IdMovimiento,
                NumeroOperacion = m.NumeroOperacion,
                NumeroCuenta    = m.NumeroCuenta,
                IdEmpleado      = m.IdEmpleado,
                NombreEmpleado  = m.Empleado != null ? m.Empleado.NombreCompleto : string.Empty,
                Tipo            = m.Tipo,
                Monto           = m.Monto,
                FechaHora       = m.FechaHora.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync();

        return Ok(movimientos);
    }

    /// <summary>Retorna estadísticas generales de la base de datos.</summary>
    [HttpGet("estadisticas")]
    public async Task<IActionResult> GetEstadisticas()
    {
        var totalEmpleados     = await _context.Empleados.CountAsync();
        var totalClientes      = await _context.Clientes.CountAsync();
        var cuentasActivas     = await _context.Cuentas.CountAsync(c => c.Estado == "Activa");
        var totalTransacciones = await _context.Movimientos.CountAsync();
        var saldoTotal         = await _context.Cuentas.SumAsync(c => c.Saldo);

        return Ok(new { totalEmpleados, totalClientes, cuentasActivas, totalTransacciones, saldoTotal });
    }

    /// <summary>Consulta la cuenta de un cliente por su número de cédula (DNI).</summary>
    [HttpGet("cuentas/cliente/{dni}")]
    public async Task<IActionResult> ConsultarCuentasPorCliente(string dni)
    {
        var cuenta = await _context.Cuentas
            .Include(c => c.Cliente)
            .Where(c => c.Cliente != null && c.Cliente.Dni == dni)
            .Select(c => new CuentaClienteDTO
            {
                NumeroCuenta    = c.NumeroCuenta,
                Saldo           = c.Saldo,
                IdCliente       = c.IdCliente,
                NombreCliente   = c.Cliente != null ? c.Cliente.Nombre   : string.Empty,
                ApellidoCliente = c.Cliente != null ? c.Cliente.Apellido : string.Empty,
                Disponibilidad  = c.Estado
            })
            .FirstOrDefaultAsync();

        if (cuenta == null)
            return NotFound(new { mensaje = "No se encontró cuenta para el DNI proporcionado" });

        return Ok(cuenta);
    }
}
