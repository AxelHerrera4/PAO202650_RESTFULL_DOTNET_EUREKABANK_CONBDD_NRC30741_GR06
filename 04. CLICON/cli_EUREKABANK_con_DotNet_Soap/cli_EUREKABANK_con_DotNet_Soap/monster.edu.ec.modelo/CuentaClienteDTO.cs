namespace monster.edu.ec.modelo
{
    public class CuentaClienteDTO
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public decimal Saldo { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string ApellidoCliente { get; set; } = string.Empty;
        public string Disponibilidad { get; set; } = string.Empty;
    }
}
