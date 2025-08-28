const cache = await caches.open("Phrases");

/** @type {RequestInit} */
const init = {headers: {"Content-Type": "audio/wav"}};

/** @type {Map<string, Uint8Array>} */
const prepared = new Map();

/**
 * @param language {string}
 * @param name {string}
 * @param data {MemoryView}
 */
export function save(language, name, data) {
    return cache.put(language + "/" + name, new Response(data.slice(), init));
}

/**
 * @param language {string}
 * @param name {string}
 */
export function remove(language, name) {
    return cache.delete(language + "/" + name);
}

/** @param language {string} */
export async function prepare(language) {
    const keys = await cache.keys();
    for (const key of keys) {
        const url = new URL(key.url);
        if (!url.pathname.startsWith(language, 1))
            continue;
        const match = await cache.match(key);
        const bytes = await match.bytes();
        prepared.set(decodeURI(url.pathname.substring(2 + language.length)), bytes);
    }
}

export function keys() {
    return Array.from(prepared.keys());
}

export function load(name) {
    return prepared.get(name);
}

export function clearMemory() {
    prepared.clear();
}
