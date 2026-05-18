namespace monster.edu.ec.servicio
{
    public interface IEurekaBankService
    {
        monster.edu.ec.modelo.Usuario Login(string usuario, string clave);
        string Depositar(string cuentaId, decimal monto, int empleadoId);
        string Retirar(string cuentaId, decimal monto, int empleadoId);
        string Transferir(string cuentaOrigen, string cuentaDestino, decimal monto, int empleadoId);
        dynamic GetCuentaInfo(string dni);
        List<dynamic> GetExtracto(string cuentaId);
        List<dynamic> ListarCuentasPorSucursal(int sucursalId);
        dynamic GetEstadisticas();
        monster.edu.ec.modelo.Usuario GetUsuarioActual();
        void Logout();
    }
}
