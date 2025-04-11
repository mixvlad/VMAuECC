export interface ControlTypeParameter {
  name: string;
  displayName: string;
  description: string;
  type: string;
  required: boolean;
  defaultValue: string;
}

export interface ControlTypeParameters {
  controlTypeId: string;
  parameters: ControlTypeParameter[];
}
