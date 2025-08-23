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

export async function list() {
    const keys = await cache.keys();
    return keys.map(e => e.url);
}