const cache = await caches.open("Phrases");

/** @type {RequestInit} */
const init = {headers: {"Content-Type": "audio/wav"}};

/**
 * @param name {string}
 * @param data {MemoryView}
 */
export function save(name, data) {
    return cache.put(name, new Response(data.slice(), init));
}

export async function load(name) {
    const match = await cache.match(name);
    return match == null ? null : await match.bytes();
}

/**
 * @param callback {(name: string, data: Uint8Array) => void}
 */
export async function list(callback) {
    const keys = await cache.keys();
    for (const key of keys) {
        const match = await cache.match(key);
        const bytes = await match.bytes();
        callback(key.url, bytes);
    }
}