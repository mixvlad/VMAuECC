import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ControlTypeService } from '../../services/control-type.service';
import { YamlService } from '../../services/yaml.service';
import { ControlTypeParameter, ControlTypeWithParameters } from '../../models/control-type';

@Component({
  selector: 'app-yaml-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatCardModule,
    MatButtonModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],  templateUrl: './yaml-form.component.html',
  styleUrls: ['./yaml-form.component.scss']
})
export class YamlFormComponent implements OnInit {
  controlTypeId: string = '';
  osType: string = '';
  controlTypeName: string = '';
  controlTypeDescription: string = '';
  controlTypeParameters: ControlTypeParameter[] = [];
  form!: FormGroup;
  generatedYaml: string = '';
  isLoading: boolean = false;
  error: string = '';
  isGeneratingYaml: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private yamlService: YamlService,
    private controlTypeService: ControlTypeService,
    private snackBar: MatSnackBar
  ) { }

  validateCustomParameters(control: AbstractControl): {[key: string]: any} | null {
    if (!control.value) {
      return null; // Empty is valid
    }
    
    try {
      JSON.parse(control.value);
      return null; // Valid JSON
    } catch (e) {
      return { 'invalidJson': true };
    }
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.controlTypeId = params['controlTypeId'];
      this.osType = params['osType'];
      this.loadControlType();
    });

    // Initialize form with customParameters and validation
    this.form = this.formBuilder.group({
      customParameters: ['', this.validateCustomParameters]
    });
  }

  loadControlType(): void {
    this.isLoading = true;
    
    this.controlTypeService.getControlType(this.osType, this.controlTypeId)
      .subscribe({
        next: (controlType: ControlTypeWithParameters) => {
          this.controlTypeName = controlType.name;
          this.controlTypeDescription = controlType.description;
          this.controlTypeParameters = controlType.parameters;
          
          // Add dynamic form controls based on parameters
          this.controlTypeParameters.forEach(param => {
            const validators = param.required ? [Validators.required] : [];
            this.form.addControl(param.name, this.formBuilder.control(param.defaultValue, validators));
          });
          
          this.isLoading = false;
        },
        error: (error) => {
          this.error = 'Failed to load control type. Please try again.';
          this.isLoading = false;
          console.error('Error loading control type:', error);
        }
      });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    this.isGeneratingYaml = true;
    this.error = '';
    this.generatedYaml = ''; // Сбрасываем предыдущий результат

    const formData = this.form.value;
    
    this.yamlService.generateYaml(formData, this.controlTypeId, this.osType)
      .subscribe({
        next: (yaml) => {
          this.generatedYaml = yaml;
          this.isGeneratingYaml = false;
          this.snackBar.open('YAML generated successfully', 'Close', {
            duration: 3000
          });
        },
        error: (error) => {
          this.error = 'Failed to generate YAML. Please try again.';
          this.isGeneratingYaml = false;
          console.error('Error generating YAML:', error);
          this.snackBar.open('Error generating YAML', 'Close', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
        }
      });
  }

  downloadYaml(): void {
    if (!this.generatedYaml) {
      return;
    }

    const blob = new Blob([this.generatedYaml], { type: 'text/yaml' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `${this.controlTypeId}-config.yaml`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);

    this.snackBar.open('YAML file downloaded successfully', 'Close', {
      duration: 3000
    });
  }

  goBack(): void {
    this.router.navigate(['/control-types', this.osType]);
  }
}