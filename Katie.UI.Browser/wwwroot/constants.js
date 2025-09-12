/**
 * @typedef {Object} MemoryView
 * @property {(target: any, sourceOffset?: number) => void} copyTo
 * @property {number} length
 * @property {(start?: number, end?: number) => any} slice
 */

export const bufferSize = 128;
export const read = "read";
export const clear = "clear";
/** @type {RequestInit} */
export const init = {headers: {"Content-Type": "audio/wav"}};