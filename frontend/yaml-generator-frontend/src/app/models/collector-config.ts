export interface CollectorConfig {
  osType: string;
  controlType: string;
  collectItems: string[];
  targetPath: string;
  intervalSeconds: number;
  includeSubdirectories: boolean;
  filePatterns: string[];
  customParameters: { [key: string]: string };
}
