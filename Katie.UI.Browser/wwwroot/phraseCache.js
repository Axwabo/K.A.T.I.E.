import { init } from "./constants.js";

const cacheName = "Phrases";

let phraseCache = await caches.open(cacheName);

/** @type {Map<string, Uint8Array>} */
const prepared = new Map();

/**
 * @param language {string}
 * @param name {string}
 * @param data {MemoryView}
 */
export function save(language, name, data) {
    return phraseCache.put(`/${language}/${name}`, new Response(data.slice(), init));
}

/** @param language {string} */
export async function prepare(language) {
    const keys = await phraseCache.keys();
    for (const key of keys) {
        const url = new URL(key.url);
        if (!url.pathname.startsWith(language, 1))
            continue;
        const match = await phraseCache.match(key);
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

export async function clear() {
    await caches.delete(cacheName);
    phraseCache = await caches.open(cacheName);
}
