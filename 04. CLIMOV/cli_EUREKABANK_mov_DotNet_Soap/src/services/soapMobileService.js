import { API_CONFIG } from '../constants/Config';

function xmlEscape(str) {
    if (!str) return "";
    return String(str)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&apos;");
}

/**
 * Función auxiliar para realizar peticiones SOAP en React Native
 */
async function soapRequest(operation, params = {}) {
    let paramsXml = "";
    for (const [key, value] of Object.entries(params)) {
        paramsXml += `<${key}>${xmlEscape(value)}</${key}>`;
    }

    const envelope = `
        <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tns="${API_CONFIG.NAMESPACE}">
           <soap:Body>
              <tns:${operation}>
                 ${paramsXml}
              </tns:${operation}>
           </soap:Body>
        </soap:Envelope>
    `.trim();

    try {
        const response = await fetch(API_CONFIG.BASE_URL, {
            method: "POST",
            headers: {
                "Content-Type": "text/xml; charset=utf-8",
                "SOAPAction": `${API_CONFIG.SERVICE_ACTION}/${operation}`
            },
            body: envelope
        });

        const xmlText = await response.text();

        if (!response.ok) {
            console.error("HTTP Error:", response.status, xmlText);
            throw new Error(`Error de servidor (${response.status})`);
        }

        return xmlText;
    } catch (error) {
        console.error("SOAP Fetch Error:", error);
        throw error;
    }
}

/**
 * Extrae el contenido de una etiqueta XML simple usando Regex
 */
function getTagValue(xml, tagName) {
    // Acepta opcionalmente prefijos como <ns2:tipo> o <ax21:tipo>
    const regex = new RegExp(`<(\\w+:)?${tagName}[^>]*>([^<]*)</(\\w+:)?${tagName}>`, 'i');
    const match = xml.match(regex);
    return match ? match[2] : null; // El valor capturado estará en el segundo grupo
}

/**
 * Extrae múltiples ocurrencias de un bloque (para el extracto)
 */
function getBlocks(xml, blockName) {
    // Acepta opcionalmente prefijos como <ns2:return> o <ax21:return>
    const regex = new RegExp(`<(\\w+:)?${blockName}[^>]*>([\\s\\S]*?)</(\\w+:)?${blockName}>`, 'gi');
    const matches = [];
    let match;
    while ((match = regex.exec(xml)) !== null) {
        matches.push(match[2]); // El contenido del bloque estará en el segundo grupo de captura
    }
    return matches;
}

export const soapMobileService = {
    /**
     * Login: AutenticarUsuario
     */
    async login(usuario, clave) {
        try {
            const xml = await soapRequest("AutenticarUsuario", { usuario, clave });
            const resultElement = getTagValue(xml, "AutenticarUsuarioResult") || "";

            if (resultElement.startsWith("SUCCESS:")) {
                const match = resultElement.match(/Bienvenido\s+(.+)\s+\((.+)\)/i);
                if (match) {
                    const role = match[2].trim();
                    return {
                        success: true,
                        nombre: match[1].trim(),
                        rol: role,
                        usuario: usuario
                    };
                }
            }

            throw new Error(resultElement.replace("ERROR: ", "") || "Credenciales inválidas");
        } catch (error) {
            throw new Error(error.message || "Error al autenticar");
        }
    },

    /**
     * Obtener información de cuenta por DNI
     */
    async getAccountStatus(dni) {
        try {
            const xml = await soapRequest("ConsultarCuentasPorCliente", { dni });

            return {
                numero: getTagValue(xml, "NumeroCuenta"),
                saldo: getTagValue(xml, "Saldo"),
                disponibilidad: getTagValue(xml, "Disponibilidad"),
                nombre: getTagValue(xml, "NombreCliente"),
                apellido: getTagValue(xml, "ApellidoCliente")
            };
        } catch (error) {
            console.error("[SOAP] Error en getAccountStatus:", error);
            return null;
        }
    },

    /**
     * Obtener extracto/historial de movimientos
     */
    async getExtracto(numeroCuenta) {
        try {
            console.log("[SOAP] Consultando extracto para cuenta:", numeroCuenta);
            const xml = await soapRequest("ConsultarExtracto", { cuentaId: numeroCuenta });
            console.log("[SOAP] XML recibido de ConsultarExtracto");

            const blocks = getBlocks(xml, "Movimiento");
            console.log("[SOAP] Movimientos encontrados:", blocks.length);

            const result = blocks.map((block, index) => {
                const tipo = getTagValue(block, "Tipo");
                const monto = getTagValue(block, "Monto");
                const fecha = getTagValue(block, "FechaHora");
                const numOp = getTagValue(block, "NumeroOperacion");
                console.log(`[SOAP] Movimiento #${index} -> Op: ${numOp}, Tipo: ${tipo}, Monto: ${monto}, Fecha: ${fecha}`);
                return {
                    id: numOp || Math.random().toString(36).substr(2, 9),
                    numeroOperacion: numOp,
                    tipo: tipo || "Desconocido",
                    monto: monto || "0.0",
                    fecha: fecha || "Sin fecha"
                };
            });

            return result;
        } catch (error) {
            console.error("[SOAP] Error en getExtracto:", error);
            return [];
        }
    },

    /**
     * Realizar depósito
     */
    async depositar(cuentaId, monto, empleadoId) {
        try {
            const xml = await soapRequest("Depositar", {
                cuentaId,
                monto: parseFloat(monto),
                empleadoId: parseInt(empleadoId)
            });
            const resultado = getTagValue(xml, "DepositarResult") || "";

            return {
                success: resultado.startsWith("SUCCESS") || resultado.startsWith("DEP-"),
                operacion: resultado,
                message: resultado
            };
        } catch (error) {
            return {
                success: false,
                operacion: null,
                message: error.message
            };
        }
    },

    /**
     * Realizar retiro
     */
    async retirar(cuentaId, monto, empleadoId) {
        try {
            const xml = await soapRequest("Retirar", {
                cuentaId,
                monto: parseFloat(monto),
                empleadoId: parseInt(empleadoId)
            });
            const resultado = getTagValue(xml, "RetirarResult") || "";

            return {
                success: resultado.startsWith("SUCCESS") || resultado.startsWith("RET-"),
                operacion: resultado,
                message: resultado
            };
        } catch (error) {
            return {
                success: false,
                operacion: null,
                message: error.message
            };
        }
    },

    /**
     * Realizar transferencia
     */
    async transferir(cuentaOrigen, cuentaDestino, monto, empleadoId) {
        try {
            const xml = await soapRequest("Transferir", {
                cuentaOrigenId: cuentaOrigen,
                cuentaDestinoId: cuentaDestino,
                monto: parseFloat(monto),
                empleadoId: parseInt(empleadoId)
            });
            const resultado = getTagValue(xml, "TransferirResult") || "";

            return {
                success: resultado.startsWith("SUCCESS") || resultado.startsWith("TRA-"),
                operacion: resultado,
                message: resultado
            };
        } catch (error) {
            return {
                success: false,
                operacion: null,
                message: error.message
            };
        }
    }
};
