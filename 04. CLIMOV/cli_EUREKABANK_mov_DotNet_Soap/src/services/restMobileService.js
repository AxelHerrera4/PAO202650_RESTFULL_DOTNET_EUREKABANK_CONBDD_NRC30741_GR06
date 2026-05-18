import { API_CONFIG } from '../constants/Config';

const BASE = API_CONFIG.BASE_URL;

async function restPost(path, body) {
    const response = await fetch(`${BASE}${path}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    });
    if (!response.ok) throw new Error(`Error del servidor (${response.status})`);
    return response.json();
}

async function restGet(path) {
    const response = await fetch(`${BASE}${path}`);
    if (!response.ok) throw new Error(`Error del servidor (${response.status})`);
    return response.json();
}

export const restMobileService = {

    // Clientes usan su cédula (DNI) como usuario Y contraseña
    async login(usuario, clave) {
        if (usuario !== clave) {
            throw new Error('Usuario o contraseña incorrectos');
        }

        try {
            const response = await fetch(`${BASE}/api/eureka/cuentas/cliente/${usuario}`);

            if (response.status === 404) {
                throw new Error('No se encontró ninguna cuenta para esa cédula');
            }
            if (!response.ok) {
                throw new Error(`Error del servidor (${response.status})`);
            }

            const data = await response.json();
            return {
                success: true,
                nombre: `${data.nombreCliente} ${data.apellidoCliente}`.trim(),
                rol: 'cliente',
                usuario: usuario
            };
        } catch (error) {
            throw new Error(error.message || 'Error al autenticar');
        }
    },

    async getAccountStatus(dni) {
        try {
            const data = await restGet(`/api/eureka/cuentas/cliente/${dni}`);
            return {
                numero: data.numeroCuenta,
                saldo: data.saldo,
                disponibilidad: data.disponibilidad === 'Activa' ? 'VERDE' : 'ROJO',
                nombre: data.nombreCliente,
                apellido: data.apellidoCliente
            };
        } catch (error) {
            console.error('[REST] Error en getAccountStatus:', error);
            return null;
        }
    },

    async getExtracto(numeroCuenta) {
        try {
            const data = await restGet(`/api/eureka/extracto/${numeroCuenta}`);
            return data.map(m => ({
                id: m.numeroOperacion || Math.random().toString(36).substr(2, 9),
                numeroOperacion: m.numeroOperacion,
                tipo: m.tipo || 'Desconocido',
                monto: m.monto != null ? m.monto.toString() : '0',
                fecha: m.fechaHora || 'Sin fecha'
            }));
        } catch (error) {
            console.error('[REST] Error en getExtracto:', error);
            return [];
        }
    },

    async depositar(cuentaId, monto, empleadoId) {
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
    },

    async retirar(cuentaId, monto, empleadoId) {
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
    },

    async transferir(cuentaOrigen, cuentaDestino, monto, empleadoId) {
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
};
