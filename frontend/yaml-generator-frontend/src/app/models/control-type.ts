export interface ControlType {
  id: string;
  name: string;
  description: string;
}

export interface ControlTypeWithParameters extends ControlType {
  parameters: ControlTypeParameter[];
}

export interface ControlTypeParameter {
  name: string;
  displayName: string;
  description: string;
  type: string;
  required: boolean;
  defaultValue: string;
}

export interface OsControlTypes {
  os: string;
  controlTypes: ControlType[];
}
