import { readFromProvider } from "./main";

/** @type {number[]} */
const buffer = new Array(128);
buffer.fill(0);

class NAudioProcessor extends AudioWorkletProcessor {
    process(inputs, outputs, parameters) {
        const view = readFromProvider();
        view.copyTo(buffer, 0);
        for (let i = 0; i < view.length; i++)
            outputs[0][i] = buffer[i];
        return true;
    }
}

registerProcessor("naudio-processor", NAudioProcessor);