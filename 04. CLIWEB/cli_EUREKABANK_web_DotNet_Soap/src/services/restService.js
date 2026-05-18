const BASE_URL = import.meta.env.DEV ? '' : 'http://192.168.0.101:8083';

async function restPost(path, body) {
    const response = await fetch(`${BASE_URL}${path}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return response.json();
}

async function restGet(path) {
    const response = await fetch(`${BASE_URL}${path}`);
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return response.json();
}

export async function autenticarUsuario(usuario, clave) {
    try {
        const data = await restPost('/api/eureka/autenticar', { usuario, clave });

        if (data.resultado === 'OK') {
            return {
                success: true,
                nombre: data.nombre,
                rol: data.rol,
                idEmpleado: data.idEmpleado,
                idSucursal: 1,
                nombreSucursal: 'Sede Principal - Eureka Bank'
            };
        }

        return { success: false, message: data.resultado };
    } catch (error) {
        return { success: false, message: error.message };
    }
}

export async function listarCuentasPorSucursal(idSucursal) {
    try {
        const data = await restGet(`/api/eureka/cuentas/sucursal/${idSucursal}`);
        return data.map(c => ({
            numero: c.numeroCuenta,
            titular: `${c.nombreCliente} ${c.apellidoCliente}`.trim() || 'Consumidor Final',
            saldo: c.saldo,
            estado: c.disponibilidad === 'Activa' ? 'LIBRE' : 'OCUPADA'
        }));
    } catch (error) {
        console.error('Error al listar cuentas:', error);
        return [];
    }
}

export async function consultarExtracto(cuentaId) {
    try {
        const data = await restGet(`/api/eureka/extracto/${cuentaId}`);
        return data.map(m => ({
            numeroOperacion: m.numeroOperacion,
            tipo: m.tipo,
            monto: m.monto,
            fechaHora: m.fechaHora,
            nombreEmpleado: m.nombreEmpleado
        }));
    } catch (error) {
        console.error('Error al consultar extracto:', error);
        return [];
    }
}

export async function depositar(cuentaId, monto, empleadoId) {
    try {
        const data = await restPost('/api/eureka/depositar', {
            cuentaId,
            monto: parseFloat(monto),
            empleadoId: parseInt(empleadoId)
        });
        const ok = data.resultado?.includes('correctamente');
        return { success: ok, message: data.resultado };
    } catch (error) {
        return { success: false, message: error.message };
    }
}

export async function retirar(cuentaId, monto, empleadoId) {
    try {
        const data = await restPost('/api/eureka/retirar', {
            cuentaId,
            monto: parseFloat(monto),
            empleadoId: parseInt(empleadoId)
        });
        const ok = data.resultado?.includes('correctamente');
        return { success: ok, message: data.resultado };
    } catch (error) {
        return { success: false, message: error.message };
    }
}

export async function transferir(cuentaOrigen, cuentaDestino, monto, empleadoId) {
    try {
        const data = await restPost('/api/eureka/transferir', {
            cuentaOrigenId: cuentaOrigen,
            cuentaDestinoId: cuentaDestino,
            monto: parseFloat(monto),
            empleadoId: parseInt(empleadoId)
        });
        const ok = data.resultado?.includes('correctamente');
        return { success: ok, message: data.resultado };
    } catch (error) {
        return { success: false, message: error.message };
    }
}

export async function consultarCuentasPorCliente(dni) {
    try {
        const data = await restGet(`/api/eureka/cuentas/cliente/${dni}`);
        return {
            numero: data.numeroCuenta,
            titular: `${data.nombreCliente} ${data.apellidoCliente}`.trim(),
            saldo: data.saldo,
            estado: data.disponibilidad === 'Activa' ? 'LIBRE' : 'OCUPADA'
        };
    } catch (error) {
        console.error('Error al consultar cuenta:', error);
        return null;
    }
}
