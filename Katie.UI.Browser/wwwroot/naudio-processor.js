/** @type {number[]} */
const buffer = new Array(128);
buffer.fill(0);

class NAudioProcessor extends AudioWorkletProcessor {

    constructor() {
        super();
        this.port.onmessage = ev => {
            if (!(ev.data?.buffer instanceof Float64Array))
                return;
            for (let i = 0; i < ev.data.length; i++)
                buffer.push(ev.data.buffer[i]);
        };
    }

    process(inputs, outputs) {
        this.port.postMessage("read");
        if (buffer.length === 0)
            return true;
        const channel = outputs[0][0];
        for (let i = 0; i < channel.length; i++)
            channel[i] = buffer.shift() || 0;
        return true;
    }
}

registerProcessor("naudio-processor", NAudioProcessor);