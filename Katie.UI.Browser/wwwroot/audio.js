const buffer = new Float64Array(128);
buffer.fill(0);

const context = new AudioContext();
let connected = false;
let paused = true;

export async function play() {
    if (!connected) {
        await context.audioWorklet.addModule("naudio-processor.js");
        const naudio = new AudioWorkletNode(context, "naudio-processor");
        naudio.port.onmessage = (ev => {
            if (ev.data !== "read")
                return;
            const view = readFromProvider();
            view.copyTo(buffer, 0);
            naudio.port.postMessage({buffer, length: view.length});
        });
        naudio.connect(context.destination);
        connected = true;
    }
    if (paused)
        await context.resume();
    paused = false;
}

export async function stop() {
    paused = true;
    await context.suspend();
}
