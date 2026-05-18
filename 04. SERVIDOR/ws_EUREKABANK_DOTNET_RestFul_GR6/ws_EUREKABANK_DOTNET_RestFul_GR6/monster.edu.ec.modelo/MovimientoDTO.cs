namespace monster.edu.ec.modelo;

public class MovimientoDTO
{
    public int IdMovimiento { get; set; }
    public string NumeroOperacion { get; set; } = string.Empty;
    public string NumeroCuenta { get; set; } = string.Empty;
    public int IdEmpleado { get; set; }
    public string NombreEmpleado { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string FechaHora { get; set; } = string.Empty;
}
