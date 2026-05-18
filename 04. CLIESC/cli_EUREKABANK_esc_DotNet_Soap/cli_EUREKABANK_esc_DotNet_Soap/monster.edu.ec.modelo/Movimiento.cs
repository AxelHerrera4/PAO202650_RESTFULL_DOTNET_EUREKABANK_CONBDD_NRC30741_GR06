using System;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.modelo
{
    public class Movimiento
    {
        public string NumeroOperacion { get; set; }
        public DateTime FechaHora { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public string NombreEmpleado { get; set; }
        public string NumeroCuenta { get; set; }

        public Movimiento() { }

        public Movimiento(string numeroOperacion, DateTime fechaHora, string tipo,
                         decimal monto, string nombreEmpleado, string numeroCuenta)
        {
            NumeroOperacion = numeroOperacion;
            FechaHora = fechaHora;
            Tipo = tipo;
            Monto = monto;
            NombreEmpleado = nombreEmpleado;
            NumeroCuenta = numeroCuenta;
        }

        public override string ToString()
        {
            return $"{NumeroOperacion} - {FechaHora:dd/MM/yyyy HH:mm} - {Tipo} - S/ {Monto:N2} - {NombreEmpleado}";
        }
    }
}
