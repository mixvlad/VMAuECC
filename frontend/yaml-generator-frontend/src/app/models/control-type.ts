export interface ControlType {
  id: string;
  name: string;
  description: string;
}

export interface OsControlTypes {
  os: string;
  controlTypes: ControlType[];
}

// Создадим сервис для получения контрольных типов с бэкенда
