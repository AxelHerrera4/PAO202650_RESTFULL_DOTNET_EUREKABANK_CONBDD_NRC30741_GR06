using System;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.modelo
{
    public class Cuenta
    {
        public string NumeroCuenta { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public decimal Saldo { get; set; }
        public string Disponibilidad { get; set; }
        public int IdSucursal { get; set; }

        public Cuenta() { }

        public Cuenta(string numeroCuenta, string nombreCliente, string apellidoCliente,
                     decimal saldo, string disponibilidad, int idSucursal)
        {
            NumeroCuenta = numeroCuenta;
            NombreCliente = nombreCliente;
            ApellidoCliente = apellidoCliente;
            Saldo = saldo;
            Disponibilidad = disponibilidad;
            IdSucursal = idSucursal;
        }

        public override string ToString()
        {
            return $"Cuenta: {NumeroCuenta} - {NombreCliente} {ApellidoCliente} - S/ {Saldo:N2}";
        }
    }
}
