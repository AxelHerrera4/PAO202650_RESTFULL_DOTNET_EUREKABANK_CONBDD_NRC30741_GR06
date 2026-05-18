using System;
using System.Collections.Generic;
using System.Windows.Forms;
using cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.vista;
using cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.servicio;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.controlador
{
    public class CajeroDashboardController
    {
        private CajeroDashboardForm vista;
        private EurekaBankServiceClient port;
        private int idEmpleado;
        private string nombreEmpleado;
        private string cuentaActivaId = null;

        public CajeroDashboardController(CajeroDashboardForm vista, int idEmpleado, string nombreEmpleado)
        {
            this.vista = vista;
            this.idEmpleado = idEmpleado;
            this.nombreEmpleado = nombreEmpleado;
            this.port = new EurekaBankServiceClient();

            vista.GetBtnBuscarCuenta().Click += BuscarCuenta;
            vista.GetBtnProcesarDeposito().Click += ProcesarDeposito;
            vista.GetBtnProcesarRetiro().Click += ProcesarRetiro;
            vista.GetBtnProcesarTransferencia().Click += ProcesarTransferencia;
        }

        private void BuscarCuenta(object sender, EventArgs e)
        {
            string numCuenta = vista.GetNumCuentaBusqueda();
            if (string.IsNullOrWhiteSpace(numCuenta))
            {
                vista.MostrarMensaje("Ingrese un número de cuenta.", "Validación", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                CuentaClienteDTO cuenta = null;
                for (int i = 1; i <= 5; i++)
                {
                    List<CuentaClienteDTO> cuentas = port.ListarCuentasPorSucursal(i);
                    foreach (var c in cuentas)
                    {
                        if (c.NumeroCuenta.Equals(numCuenta))
                        {
                            cuenta = c;
                            break;
                        }
                    }
                    if (cuenta != null) break;
                }

                if (cuenta != null)
                {
                    cuentaActivaId = cuenta.NumeroCuenta;
                    vista.SetTitular(cuenta.NombreCliente + " " + cuenta.ApellidoCliente);
                    vista.SetSaldo("S/ " + string.Format("{0:F2}", cuenta.Saldo));
                    vista.SetEstado(cuenta.Disponibilidad);
                    CargarMovimientos(cuentaActivaId);
                }
                else
                {
                    vista.MostrarMensaje("Cuenta no encontrada.", "Error", MessageBoxIcon.Error);
                    LimpiarDatosCuenta();
                }
            }
            catch (Exception ex)
            {
                vista.MostrarMensaje("Error: " + ex.Message, "Error de Red", MessageBoxIcon.Error);
            }
        }

        private void CargarMovimientos(string cuentaId)
        {
            try
            {
                List<Movimiento> movimientos = port.ConsultarExtracto(cuentaId);
                DataGridView tbl = vista.GetTblMovimientos();
                tbl.Rows.Clear();

                foreach (Movimiento mov in movimientos)
                {
                    tbl.Rows.Add(
                        mov.NumeroOperacion,
                        mov.FechaHora ?? "N/A",
                        mov.Tipo,
                        "S/ " + string.Format("{0:F2}", mov.Monto),
                        mov.NombreEmpleado ?? "Desconocido"
                    );
                }
            }
            catch (Exception ex)
            {
                vista.MostrarMensaje("Error al cargar historial: " + ex.Message, "Error", MessageBoxIcon.Error);
            }
        }

        private void ProcesarDeposito(object sender, EventArgs e)
        {
            if (cuentaActivaId == null)
            {
                vista.MostrarMensaje("Seleccione una cuenta primero.", "Validación", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                double monto = double.Parse(vista.GetMontoDeposito());
                string respuesta = port.Depositar(cuentaActivaId, monto, idEmpleado);
                ManejarRespuesta(respuesta);
            }
            catch
            {
                vista.MostrarMensaje("Monto inválido.", "Error", MessageBoxIcon.Error);
            }
        }

        private void ProcesarRetiro(object sender, EventArgs e)
        {
            if (cuentaActivaId == null)
            {
                vista.MostrarMensaje("Seleccione una cuenta primero.", "Validación", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                double monto = double.Parse(vista.GetMontoRetiro());
                string respuesta = port.Retirar(cuentaActivaId, monto, idEmpleado);
                ManejarRespuesta(respuesta);
            }
            catch
            {
                vista.MostrarMensaje("Monto inválido.", "Error", MessageBoxIcon.Error);
            }
        }

        private void ProcesarTransferencia(object sender, EventArgs e)
        {
            if (cuentaActivaId == null)
            {
                vista.MostrarMensaje("Seleccione una cuenta primero.", "Validación", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string destino = vista.GetCuentaDestino();
                double monto = double.Parse(vista.GetMontoTransferencia());
                string respuesta = port.Transferir(cuentaActivaId, destino, monto, idEmpleado);
                ManejarRespuesta(respuesta);
            }
            catch
            {
                vista.MostrarMensaje("Datos inválidos.", "Error", MessageBoxIcon.Error);
            }
        }

        private void ManejarRespuesta(string respuesta)
        {
            if (respuesta != null && respuesta.Contains("SUCCESS"))
            {
                vista.MostrarMensaje("Operación exitosa.", "Éxito", MessageBoxIcon.Information);
                vista.LimpiarCamposTransaccion();
                BuscarCuenta(null, null);
            }
            else
            {
                vista.MostrarMensaje(respuesta, "Error", MessageBoxIcon.Error);
            }
        }

        private void LimpiarDatosCuenta()
        {
            cuentaActivaId = null;
            vista.SetTitular("-");
            vista.SetSaldo("S/ 0.00");
            vista.SetEstado("-");
            vista.GetTblMovimientos().Rows.Clear();
        }
    }
}
