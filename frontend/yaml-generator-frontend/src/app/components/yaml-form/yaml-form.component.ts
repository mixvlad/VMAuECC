import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { ControlTypeService } from '../../services/control-type.service';
import { YamlService } from '../../services/yaml.service';
import { ControlTypeParameter, ControlTypeWithParameters } from '../../models/control-type';
import { LanguageService } from '../../services/language.service';

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
    MatProgressSpinnerModule,
    TranslateModule
  ],  
  templateUrl: './yaml-form.component.html',
  styleUrls: ['./yaml-form.component.scss']
})
export class YamlFormComponent implements OnInit {  controlTypeId: string = '';
  osType: string = '';
  controlTypeName: string = '';
  controlTypeDescription: string = '';
  controlTypeParameters: ControlTypeParameter[] = [];
  form!: FormGroup;
  generatedYaml: string = '';
  isLoading: boolean = false;
  error: string = '';
  isGeneratingYaml: boolean = false;
  currentLanguage: string = 'en';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private yamlService: YamlService,
    private controlTypeService: ControlTypeService,
    private snackBar: MatSnackBar,
    private languageService: LanguageService
  ) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.controlTypeId = params['controlTypeId'];
      this.osType = params['osType'];
      this.loadControlType();
    });

    this.form = this.formBuilder.group({
      // Only include other form controls here, not customParameters
    });

    this.languageService.language$.subscribe(lang => {
      this.currentLanguage = lang;
      this.loadControlType();
    });
  }

  loadControlType(): void {
    this.isLoading = true;
    
    this.controlTypeService.getControlType(this.osType, this.controlTypeId, this.currentLanguage)
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

  // Добавим метод для генерации и скачивания ZIP
  generateZip(): void {
    if (this.form.valid) {
      this.isLoading = true;
      this.yamlService.generateZip(this.form.value, this.controlTypeId, this.osType)
        .subscribe({
          next: (data: Blob) => {
            this.isLoading = false;
            // Создаем ссылку для скачивания
            const url = window.URL.createObjectURL(data);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'configuration.zip';
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
            
            // Показываем уведомление об успехе
            this.snackBar.open('ZIP file generated successfully!', 'Close', {
              duration: 3000,
            });
          },
          error: (error) => {
            this.isLoading = false;
            console.error('Error generating ZIP:', error);
            this.snackBar.open('Error generating ZIP file', 'Close', {
              duration: 3000,
            });
          }
        });
    } else {
      this.markFormGroupTouched(this.form);
    }
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  // Add this method to the YamlFormComponent class
  generateYaml(): void {
    // This method should handle the YAML generation
    if (this.form.valid) {
      this.onSubmit(); // We can reuse the existing onSubmit method
    } else {
      this.markFormGroupTouched(this.form);
    }
  }
}