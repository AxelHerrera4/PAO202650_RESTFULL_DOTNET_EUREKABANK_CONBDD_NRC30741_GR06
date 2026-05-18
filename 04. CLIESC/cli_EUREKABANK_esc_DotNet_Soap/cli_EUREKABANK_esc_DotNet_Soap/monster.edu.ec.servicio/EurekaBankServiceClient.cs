using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.servicio
{
    public class CuentaClienteDTO
    {
        public string NumeroCuenta { get; set; }
        public decimal Saldo { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string Disponibilidad { get; set; }
    }

    public class Movimiento
    {
        public int IdMovimiento { get; set; }
        public string NumeroOperacion { get; set; }
        public string NumeroCuenta { get; set; }
        public int IdEmpleado { get; set; }
        public string NombreEmpleado { get; set; }
        public string Tipo { get; set; }
        public decimal Monto { get; set; }
        public string FechaHora { get; set; }
    }

    public class EurekaBankServiceClient
    {
        private const string BASE_URL = "http://192.168.0.101:8083/";
        private static readonly HttpClient _http = new HttpClient { BaseAddress = new Uri(BASE_URL) };
        private static readonly JavaScriptSerializer _json = new JavaScriptSerializer();

        private StringContent ToJson(object obj)
        {
            return new StringContent(_json.Serialize(obj), Encoding.UTF8, "application/json");
        }

        private Dictionary<string, object> ParseDict(string json)
        {
            return _json.Deserialize<Dictionary<string, object>>(json);
        }

        // Devuelve "SUCCESS:{idEmpleado}:{nombre}" o lanza excepción
        public string AutenticarUsuario(string usuario, string clave)
        {
            try
            {
                var content  = ToJson(new { usuario, clave });
                var response = _http.PostAsync("api/eureka/autenticar", content).Result;
                var dict     = ParseDict(response.Content.ReadAsStringAsync().Result);
                string resultado = dict["resultado"].ToString();

                if (resultado == "OK")
                {
                    int    idEmp  = Convert.ToInt32(dict["idEmpleado"]);
                    string nombre = dict["nombre"].ToString();
                    return $"SUCCESS:{idEmp}:{nombre}";
                }

                return $"ERROR: {resultado}";
            }
            catch (Exception ex)
            {
                throw new Exception("Error al autenticar: " + ex.Message);
            }
        }

        // Devuelve "SUCCESS: mensaje" o "ERROR: mensaje"
        public string Depositar(string cuentaId, double monto, int empleadoId)
        {
            try
            {
                var content  = ToJson(new { cuentaId, monto, empleadoId });
                var response = _http.PostAsync("api/eureka/depositar", content).Result;
                var dict     = ParseDict(response.Content.ReadAsStringAsync().Result);
                string resultado = dict["resultado"].ToString();

                return resultado.Contains("correctamente")
                    ? "SUCCESS: " + resultado
                    : "ERROR: " + resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al depositar: " + ex.Message);
            }
        }

        public string Retirar(string cuentaId, double monto, int empleadoId)
        {
            try
            {
                var content  = ToJson(new { cuentaId, monto, empleadoId });
                var response = _http.PostAsync("api/eureka/retirar", content).Result;
                var dict     = ParseDict(response.Content.ReadAsStringAsync().Result);
                string resultado = dict["resultado"].ToString();

                return resultado.Contains("correctamente")
                    ? "SUCCESS: " + resultado
                    : "ERROR: " + resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al retirar: " + ex.Message);
            }
        }

        public string Transferir(string cuentaOrigenId, string cuentaDestinoId, double monto, int empleadoId)
        {
            try
            {
                var content  = ToJson(new { cuentaOrigenId, cuentaDestinoId, monto, empleadoId });
                var response = _http.PostAsync("api/eureka/transferir", content).Result;
                var dict     = ParseDict(response.Content.ReadAsStringAsync().Result);
                string resultado = dict["resultado"].ToString();

                return resultado.Contains("correctamente")
                    ? "SUCCESS: " + resultado
                    : "ERROR: " + resultado;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al transferir: " + ex.Message);
            }
        }

        public List<CuentaClienteDTO> ListarCuentasPorSucursal(int sucursalId)
        {
            try
            {
                var response = _http.GetAsync($"api/eureka/cuentas/sucursal/{sucursalId}").Result;
                var lista    = _json.Deserialize<List<CuentaClienteDTO>>(
                                   response.Content.ReadAsStringAsync().Result);
                return lista ?? new List<CuentaClienteDTO>();
            }
            catch
            {
                return new List<CuentaClienteDTO>();
            }
        }

        public List<Movimiento> ConsultarExtracto(string cuentaId)
        {
            try
            {
                var response = _http.GetAsync($"api/eureka/extracto/{cuentaId}").Result;
                var lista    = _json.Deserialize<List<Movimiento>>(
                                   response.Content.ReadAsStringAsync().Result);
                return lista ?? new List<Movimiento>();
            }
            catch
            {
                return new List<Movimiento>();
            }
        }
    }
}
