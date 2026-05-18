// En desarrollo usa el proxy de Vite, en producción usa la URL directa
const SOAP_URL = import.meta.env.DEV ? "/eureka-soap/EurekaBankWS.asmx" : "http://192.168.0.101:8080/EurekaBankWS.asmx";
const NAMESPACE = "http://tempuri.org/";
const SERVICE_ACTION = "http://tempuri.org/EurekaBankWSControlador";

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
 * Función auxiliar para realizar peticiones SOAP
 */
async function soapRequest(operation, params = {}) {
    let paramsXml = "";
    for (const [key, value] of Object.entries(params)) {
        paramsXml += `<${key}>${xmlEscape(value)}</${key}>`;
    }

    const envelope = `
        <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tns="${NAMESPACE}">
           <soap:Body>
              <tns:${operation}>
                 ${paramsXml}
              </tns:${operation}>
           </soap:Body>
        </soap:Envelope>
    `.trim();

    try {
        const response = await fetch(SOAP_URL, {
            method: "POST",
            headers: {
                "Content-Type": "text/xml; charset=utf-8",
                "SOAPAction": `${SERVICE_ACTION}/${operation}`
            },
            body: envelope
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const xmlText = await response.text();
        const parser = new DOMParser();
        const xmlDoc = parser.parseFromString(xmlText, "text/xml");

        if (xmlDoc.getElementsByTagName("parsererror").length > 0) {
            throw new Error("Invalid XML response");
        }

        return xmlDoc;
    } catch (error) {
        console.error(`SOAP Error [${operation}]:`, error);
        throw error;
    }
}

/**
 * Autentica al usuario y devuelve sus datos si tiene éxito
 * Formato backend: "SUCCESS: Bienvenido Nombre (Rol)"
 */
export async function autenticarUsuario(usuario, clave) {
    try {
        const xmlDoc = await soapRequest("AutenticarUsuario", { usuario, clave });

        const resultElement = xmlDoc.querySelector("AutenticarUsuarioResult");
        const returnText = resultElement?.textContent || "";

        console.log("DEBUG: Respuesta login recibida ->", returnText);

        if (returnText.startsWith("SUCCESS:")) {
            // Extraemos nombre y rol del formato "SUCCESS: Bienvenido Nombre (Rol)"
            const match = returnText.match(/Bienvenido\s+(.+)\s+\((.+)\)/i);

            if (match) {
                const nombre = match[1].trim();
                const rol = match[2].trim();

                return {
                    success: true,
                    nombre: nombre,
                    rol: rol,
                    idSucursal: 1,
                    nombreSucursal: "Sede Principal - Eureka Bank",
                    idEmpleado: 1
                };
            }
        }

        return {
            success: false,
            message: returnText.replace("ERROR: ", "") || "Error de autenticación"
        };
    } catch (error) {
        return {
            success: false,
            message: error.message || "Error de conexión"
        };
    }
}

/**
 * Lista las cuentas de una sucursal específica
 * Consume el DTO retornado por el backend
 */
export async function listarCuentasPorSucursal(idSucursal) {
    try {
        const xmlDoc = await soapRequest("ListarCuentasPorSucursal", { sucursalId: idSucursal });

        const resultElement = xmlDoc.querySelector("ListarCuentasPorSucursalResult");
        const accounts = [];

        if (!resultElement) return accounts;

        const cuentaElements = resultElement.querySelectorAll("CuentaClienteDTO");

        cuentaElements.forEach((el) => {
            const numero = el.querySelector("NumeroCuenta")?.textContent || "N/A";
            const saldo = el.querySelector("Saldo")?.textContent || "0";
            const disponibilidad = el.querySelector("Disponibilidad")?.textContent || "1";
            const nombre = el.querySelector("NombreCliente")?.textContent || "";
            const apellido = el.querySelector("ApellidoCliente")?.textContent || "";

            accounts.push({
                numero: numero,
                titular: `${nombre} ${apellido}`.trim() || "Consumidor Final",
                saldo: parseFloat(saldo),
                estado: disponibilidad === "1" || disponibilidad.toUpperCase() === "DISPONIBLE" ? "LIBRE" : "OCUPADA"
            });
        });

        return accounts;
    } catch (error) {
        console.error("Error listing accounts:", error);
        return [];
    }
}

/**
 * Consulta el extracto (movimientos) de una cuenta
 */
export async function consultarExtracto(cuentaId) {
    try {
        const xmlDoc = await soapRequest("ConsultarExtracto", { cuentaId });

        const resultElement = xmlDoc.querySelector("ConsultarExtractoResult");
        const movimientos = [];

        if (!resultElement) return movimientos;

        const movElements = resultElement.querySelectorAll("Movimiento");

        movElements.forEach((el) => {
            const numOp = el.querySelector("NumeroOperacion")?.textContent || "";
            const tipo = el.querySelector("Tipo")?.textContent || "";
            const monto = el.querySelector("Monto")?.textContent || "0";
            const fecha = el.querySelector("FechaHora")?.textContent || "";
            const empleado = el.querySelector("NombreEmpleado")?.textContent || "";

            movimientos.push({
                numeroOperacion: numOp,
                tipo: tipo,
                monto: parseFloat(monto),
                fechaHora: fecha,
                nombreEmpleado: empleado
            });
        });

        return movimientos;
    } catch (error) {
        console.error("Error consulting extract:", error);
        return [];
    }
}

/**
 * Realiza un depósito a una cuenta
 */
export async function depositar(cuentaId, monto, empleadoId) {
    try {
        const xmlDoc = await soapRequest("Depositar", { cuentaId, monto: parseFloat(monto), empleadoId: parseInt(empleadoId) });

        const resultElement = xmlDoc.querySelector("DepositarResult");
        const response = resultElement?.textContent || "";

        return {
            success: response.startsWith("SUCCESS") || response.startsWith("DEP-"),
            operacion: response,
            message: response
        };
    } catch (error) {
        return {
            success: false,
            operacion: null,
            message: error.message
        };
    }
}

/**
 * Realiza un retiro de una cuenta
 */
export async function retirar(cuentaId, monto, empleadoId) {
    try {
        const xmlDoc = await soapRequest("Retirar", { cuentaId, monto: parseFloat(monto), empleadoId: parseInt(empleadoId) });

        const resultElement = xmlDoc.querySelector("RetirarResult");
        const response = resultElement?.textContent || "";

        return {
            success: response.startsWith("SUCCESS") || response.startsWith("RET-"),
            operacion: response,
            message: response
        };
    } catch (error) {
        return {
            success: false,
            operacion: null,
            message: error.message
        };
    }
}

/**
 * Realiza una transferencia entre cuentas
 */
export async function transferir(cuentaOrigen, cuentaDestino, monto, empleadoId) {
    try {
        const xmlDoc = await soapRequest("Transferir", {
            cuentaOrigenId: cuentaOrigen,
            cuentaDestinoId: cuentaDestino,
            monto: parseFloat(monto),
            empleadoId: parseInt(empleadoId)
        });

        const resultElement = xmlDoc.querySelector("TransferirResult");
        const response = resultElement?.textContent || "";

        return {
            success: response.startsWith("SUCCESS") || response.startsWith("TRA-"),
            operacion: response,
            message: response
        };
    } catch (error) {
        return {
            success: false,
            operacion: null,
            message: error.message
        };
    }
}

/**
 * Consulta cuenta por DNI
 */
export async function consultarCuentasPorCliente(dni) {
    try {
        const xmlDoc = await soapRequest("ConsultarCuentasPorCliente", { dni });

        const resultElement = xmlDoc.querySelector("ConsultarCuentasPorClienteResult");

        if (!resultElement) return null;

        const numero = resultElement.querySelector("NumeroCuenta")?.textContent || "";
        const saldo = resultElement.querySelector("Saldo")?.textContent || "0";
        const nombre = resultElement.querySelector("NombreCliente")?.textContent || "";
        const apellido = resultElement.querySelector("ApellidoCliente")?.textContent || "";
        const disponibilidad = resultElement.querySelector("Disponibilidad")?.textContent || "1";

        return {
            numero: numero,
            titular: `${nombre} ${apellido}`.trim(),
            saldo: parseFloat(saldo),
            estado: disponibilidad === "1" ? "LIBRE" : "OCUPADA"
        };
    } catch (error) {
        console.error("Error consulting account:", error);
        return null;
    }
}
