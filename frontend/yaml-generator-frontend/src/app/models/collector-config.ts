export interface CollectorConfig {
    collectItems: string[];
    targetPath: string;
    intervalSeconds: number;
    includeSubdirectories: boolean;
    filePatterns: string[];
    customParameters: { [key: string]: string };
} 