export interface MemoryView {
    copyTo(target: any, sourceOffset?: number): void;

    get length(): number;

    slice(start?: number, end?: number): any;
}