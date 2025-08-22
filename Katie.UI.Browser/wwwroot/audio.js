import { bufferSize, clear, read } from "./constants.js";

const buffer = new Float64Array(bufferSize);
buffer.fill(0);

const context = new AudioContext();
/** @type {MessagePort | undefined} */
let port = undefined;
let paused = true;

export async function play() {
    if (!port) {
        await context.audioWorklet.addModule("naudio-processor.js");
        const naudio = new AudioWorkletNode(context, "naudio-processor");
        naudio.port.onmessage = (ev => {
            if (ev.data !== read)
                return;
            const view = readFromProvider();
            view.copyTo(buffer, 0);
            naudio.port.postMessage({buffer, length: view.length});
        });
        naudio.connect(context.destination);
        port = naudio.port;
    }
    if (paused)
        await context.resume();
    paused = false;
}

export async function stop() {
    port?.postMessage(clear);
    paused = true;
    await context.suspend();
}
