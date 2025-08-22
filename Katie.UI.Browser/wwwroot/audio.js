const context = new AudioContext();
let connected = false;
let paused = false;

export async function play() {
    if (!connected) {
        await context.audioWorklet.addModule("naudio-processor.js");
        const naudio = new AudioWorkletNode(context, "naudio-processor");
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
