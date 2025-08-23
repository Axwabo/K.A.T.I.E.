const cache = await caches.open("Phrases");

/**
 * @param name {string}
 * @param data {MemoryView}
 */
export function save(name, data) {
    return cache.put(name, new Response(new ReadableStream(data.slice())));
}

export async function load(name) {
    const match = await cache.match(name);
    return match == null ? null : await match.bytes();
}

export async function list() {
    const keys = await cache.keys();
    return keys.map(e => e.url);
}