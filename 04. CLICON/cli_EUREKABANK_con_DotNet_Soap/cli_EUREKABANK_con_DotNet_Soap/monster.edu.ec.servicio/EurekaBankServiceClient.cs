using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using monster.edu.ec.modelo;

namespace monster.edu.ec.servicio
{
    public class EurekaBankServiceClient : IEurekaBankService
    {
        private static readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("http://192.168.0.101:8083/")
        };

        private Usuario? _usuarioActual;

        public Usuario Login(string usuario, string clave)
        {
            try
            {
                var body = new { usuario, clave };
                var response = _http.PostAsJsonAsync("api/eureka/autenticar", body).Result;
                var json = response.Content.ReadAsStringAsync().Result;
                var doc = JsonDocument.Parse(json).RootElement;

                string resultado = doc.GetProperty("resultado").GetString() ?? "";

                if (resultado != "OK")
                    throw new Exception(resultado);

                string rol    = doc.GetProperty("rol").GetString()    ?? "";
                string nombre = doc.GetProperty("nombre").GetString() ?? "";

                _usuarioActual = new Usuario
                {
                    NombreUsuario = usuario,
                    Nombre        = nombre,
                    Rol           = rol
                };

                return _usuarioActual;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en login: {ex.Message}");
            }
        }

        public string Depositar(string cuentaId, decimal monto, int empleadoId)
        {
            try
            {
                var body = new { cuentaId, monto, empleadoId };
                var response = _http.PostAsJsonAsync("api/eureka/depositar", body).Result;
                var doc = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result).RootElement;
                return doc.GetProperty("resultado").GetString() ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en depósito: {ex.Message}");
            }
        }

        public string Retirar(string cuentaId, decimal monto, int empleadoId)
        {
            try
            {
                var body = new { cuentaId, monto, empleadoId };
                var response = _http.PostAsJsonAsync("api/eureka/retirar", body).Result;
                var doc = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result).RootElement;
                return doc.GetProperty("resultado").GetString() ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en retiro: {ex.Message}");
            }
        }

        public string Transferir(string cuentaOrigen, string cuentaDestino, decimal monto, int empleadoId)
        {
            try
            {
                var body = new { cuentaOrigenId = cuentaOrigen, cuentaDestinoId = cuentaDestino, monto, empleadoId };
                var response = _http.PostAsJsonAsync("api/eureka/transferir", body).Result;
                var doc = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result).RootElement;
                return doc.GetProperty("resultado").GetString() ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en transferencia: {ex.Message}");
            }
        }

        public dynamic GetCuentaInfo(string dni)
        {
            try
            {
                var response = _http.GetAsync($"api/eureka/cuentas/cliente/{dni}").Result;

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null!;

                var cuenta = response.Content.ReadFromJsonAsync<CuentaClienteDTO>().Result;

                if (cuenta == null) return null!;

                return new
                {
                    NumeroCuenta   = cuenta.NumeroCuenta,
                    Saldo          = cuenta.Saldo,
                    Disponibilidad = cuenta.Disponibilidad,
                    NombreCliente  = cuenta.NombreCliente
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener cuenta: {ex.Message}");
            }
        }

        public List<dynamic> GetExtracto(string cuentaId)
        {
            try
            {
                var movimientos = _http.GetFromJsonAsync<List<MovimientoDTO>>($"api/eureka/extracto/{cuentaId}").Result;
                var resultado   = new List<dynamic>();

                if (movimientos != null)
                {
                    foreach (var m in movimientos)
                    {
                        resultado.Add(new
                        {
                            TipoMovimiento  = m.Tipo,
                            Monto           = m.Monto,
                            Fecha           = m.FechaHora,
                            Empleado        = m.NombreEmpleado,
                            NumeroOperacion = m.NumeroOperacion
                        });
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener extracto: {ex.Message}");
            }
        }

        public List<dynamic> ListarCuentasPorSucursal(int sucursalId)
        {
            try
            {
                var cuentas   = _http.GetFromJsonAsync<List<CuentaClienteDTO>>($"api/eureka/cuentas/sucursal/{sucursalId}").Result;
                var resultado = new List<dynamic>();

                if (cuentas != null)
                {
                    foreach (var c in cuentas)
                    {
                        resultado.Add(new
                        {
                            NumeroCuenta    = c.NumeroCuenta,
                            Saldo           = c.Saldo,
                            NombreCliente   = c.NombreCliente,
                            ApellidoCliente = c.ApellidoCliente,
                            Disponibilidad  = c.Disponibilidad
                        });
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar cuentas por sucursal: {ex.Message}");
            }
        }

        public dynamic GetEstadisticas()
        {
            try
            {
                var response = _http.GetAsync("api/eureka/estadisticas").Result;
                var doc = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result).RootElement;
                return new
                {
                    TotalEmpleados     = doc.GetProperty("totalEmpleados").GetInt32(),
                    TotalClientes      = doc.GetProperty("totalClientes").GetInt32(),
                    CuentasActivas     = doc.GetProperty("cuentasActivas").GetInt32(),
                    TotalTransacciones = doc.GetProperty("totalTransacciones").GetInt32(),
                    SaldoTotal         = doc.GetProperty("saldoTotal").GetDecimal()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas: {ex.Message}");
            }
        }

        public Usuario GetUsuarioActual() => _usuarioActual!;

        public void Logout() => _usuarioActual = null;
    }
}
