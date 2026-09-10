const crypto = require('crypto');

async function deleteItem(providerBaseUrl, id) {
    const response = await fetch(`${providerBaseUrl}/api/Item/${id}`, { method: 'DELETE' });

    // 404 es aceptable acá: si la interacción nunca llegó a crear el item (una corrida anterior
    // falló antes de ese punto, por ejemplo), no hay nada que limpiar y no es un error de
    // teardown. Cualquier otro código sí lo es.
    if (!response.ok && response.status !== 404) {
        const detail = await response.text();
        throw new Error(
            `Provider state teardown failed: DELETE /api/Item/${id} -> ${response.status}. ${detail}`
        );
    }
}

function createStateHandlers(providerBaseUrl) {
    return {
        'crear item': {
            setup: async () => {
                // Nada que sembrar: la precondición es que el item todavía no exista.
            },
            teardown: async (params) => {
                const id = params && params.id;
                if (id) {
                    await deleteItem(providerBaseUrl, id);
                }
            },
        },
    };
}

module.exports = { createStateHandlers };
