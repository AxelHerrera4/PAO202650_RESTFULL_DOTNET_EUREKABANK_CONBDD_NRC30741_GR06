namespace monster.edu.ec.modelo;

public class AuthRequest
{
    public string Usuario { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
}

public class DepositarRequest
{
    public string CuentaId { get; set; } = string.Empty;
    public double Monto { get; set; }
    public int EmpleadoId { get; set; }
}

public class RetirarRequest
{
    public string CuentaId { get; set; } = string.Empty;
    public double Monto { get; set; }
    public int EmpleadoId { get; set; }
}

public class TransferirRequest
{
    public string CuentaOrigenId { get; set; } = string.Empty;
    public string CuentaDestinoId { get; set; } = string.Empty;
    public double Monto { get; set; }
    public int EmpleadoId { get; set; }
}
