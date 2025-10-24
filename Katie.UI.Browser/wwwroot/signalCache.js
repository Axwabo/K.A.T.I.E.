import { init } from "./constants.js";

const cacheName = "Signals";

let signalCache = await caches.open(cacheName);

/** @type {Map<string, Uint8Array>} */
const prepared = new Map();

/**
 * @param name {string}
 * @param data {MemoryView}
 */
export function save(name, data) {
    return signalCache.put(`/${name}`, new Response(data.slice(), init));
}

export async function prepare() {
    const keys = await signalCache.keys();
    for (const key of keys) {
        const url = new URL(key.url);
        const match = await signalCache.match(key);
        const bytes = await match.bytes();
        prepared.set(decodeURI(url.pathname.substring(1)), bytes);
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
    signalCache = await caches.open(cacheName);
}
