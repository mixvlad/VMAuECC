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
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { ControlTypeService } from '../../services/control-type.service';
import { YamlService } from '../../services/yaml.service';
import { ControlTypeParameter, ControlTypeWithParameters } from '../../models/control-type';
import { LanguageService } from '../../services/language.service';

// Define interface for requirement files
interface RequirementFiles {
  [key: string]: string;
}

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
    MatSelectModule,
    MatTabsModule,
    MatTooltipModule,
    TranslateModule
  ],  
  templateUrl: './yaml-form.component.html',
  styleUrls: ['./yaml-form.component.scss']
})
export class YamlFormComponent implements OnInit {
  controlTypeId: string = '';
  osType: string = '';
  controlTypeName: string = '';
  controlTypeDescription: string = '';
  controlTypeParameters: ControlTypeParameter[] = [];
  form!: FormGroup;
  
  // YAML content for different types
  generatedAUE: string = '';
  generatedStandard: string = '';
  generatedRequirement: string = '';
  
  // New properties for requirement files
  generatedRequirementFiles: RequirementFiles = {};
  selectedRequirementFile: string = '';
  requirementFileNames: string[] = [];
  
  isLoading: boolean = false;
  error: string = '';
  isGeneratingYaml: boolean = false;
  currentLanguage: string = 'en';
  activeTab: number = 0; // Track active tab

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

  // Method to handle tab changes
  onTabChange(event: any): void {
    this.activeTab = event.index;
  }

  // Generate AUE YAML
  generateAUE(): void {
    if (this.form.invalid) {
      this.markFormGroupTouched(this.form);
      return;
    }

    this.isGeneratingYaml = true;
    this.error = '';
    
    const formData = this.form.value;
    
    this.yamlService.generateAUE(formData, this.controlTypeId, this.osType)
      .subscribe({
        next: (yaml) => {
          this.generatedAUE = yaml;
          this.isGeneratingYaml = false;
          this.snackBar.open('AUE YAML generated successfully', 'Close', {
            duration: 3000
          });
        },
        error: (error) => {
          this.handleError('AUE YAML');
        }
      });
  }

  // Download AUE YAML
  downloadAUE(): void {
    if (this.form.invalid) {
      this.markFormGroupTouched(this.form);
      return;
    }

    this.isLoading = true;
    
    this.yamlService.downloadAUE(this.form.value, this.controlTypeId, this.osType)
      .subscribe({
        next: (data: Blob) => {
          this.downloadFile(data, `${this.controlTypeId}-aue.yaml`);
          this.isLoading = false;
        },
        error: (error) => {
          this.handleError('AUE YAML download');
        }
      });
  }

  // Generate Standard YAML
  generateStandard(): void {
    if (this.form.invalid) {
      this.markFormGroupTouched(this.form);
      return;
    }

    this.isGeneratingYaml = true;
    this.error = '';
    
    const formData = this.form.value;
    
    this.yamlService.generateStandard(formData, this.controlTypeId, this.osType)
      .subscribe({
        next: (yaml) => {
          this.generatedStandard = yaml;
          this.isGeneratingYaml = false;
          this.snackBar.open('Standard YAML generated successfully', 'Close', {
            duration: 3000
          });
        },
        error: (error) => {
          this.handleError('Standard YAML');
        }
      });
  }

  // Download Standard YAML
  downloadStandard(): void {
    if (this.form.invalid) {
      this.markFormGroupTouched(this.form);
      return;
    }

    this.isLoading = true;
    
    this.yamlService.downloadStandard(this.form.value, this.controlTypeId, this.osType)
      .subscribe({
        next: (data: Blob) => {
          this.downloadFile(data, `${this.controlTypeId}-standard.yaml`);
          this.isLoading = false;
        },
        error: (error) => {
          this.handleError('Standard YAML download');
        }
      });
  }

  // Generate Requirement
  generateRequirement(): void {
    if (this.form.invalid) {
      this.markFormGroupTouched(this.form);
      return;
    }

    this.isGeneratingYaml = true;
    this.error = '';
    
    const formData = this.form.value;
    
    this.yamlService.generateRequirement(formData, this.controlTypeId, this.osType)
      .subscribe({
        next: (jsonResponse) => {
          try {
            // Parse the JSON response
            this.generatedRequirementFiles = JSON.parse(jsonResponse);
            this.requirementFileNames = Object.keys(this.generatedRequirementFiles);
            
            // Select the first file by default
            if (this.requirementFileNames.length > 0) {
              this.selectedRequirementFile = this.requirementFileNames[0];
            }
            
            this.isGeneratingYaml = false;
            this.snackBar.open('Requirement generated successfully', 'Close', {
              duration: 3000
            });
          } catch (e) {
            this.handleError('Requirement parsing');
            console.error('Error parsing requirement JSON:', e);
          }
        },
        error: (error) => {
          this.handleError('Requirement');
        }
      });
  }

  // Method to select a requirement file
  selectRequirementFile(fileName: string): void {
    this.selectedRequirementFile = fileName;
  }
  
  // Get the content of the currently selected requirement file
  getSelectedRequirementFileContent(): string {
    return this.generatedRequirementFiles[this.selectedRequirementFile] || '';
  }
  
  // Check if we have requirement files
  hasRequirementFiles(): boolean {
    return this.requirementFileNames && this.requirementFileNames.length > 0;
  }

  // Download Requirement ZIP
  downloadRequirement(): void {
    if (this.form.invalid) {
      this.markFormGroupTouched(this.form);
      return;
    }

    this.isLoading = true;
    
    this.yamlService.downloadRequirement(this.form.value, this.controlTypeId, this.osType)
      .subscribe({
        next: (data: Blob) => {
          this.downloadFile(data, 'requirement.zip');
          this.isLoading = false;
        },
        error: (error) => {
          this.handleError('Requirement download');
        }
      });
  }

  // Helper method to download a file
  private downloadFile(data: Blob, filename: string): void {
    const url = window.URL.createObjectURL(data);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
    document.body.removeChild(a);
    
    this.snackBar.open(`${filename} downloaded successfully`, 'Close', {
      duration: 3000
    });
  }

  // Helper method to handle errors
  private handleError(operation: string): void {
    this.error = `Failed to generate ${operation}. Please try again.`;
    this.isGeneratingYaml = false;
    this.isLoading = false;
    this.snackBar.open(`Error generating ${operation}`, 'Close', {
      duration: 3000,
      panelClass: ['error-snackbar']
    });
  }

  // Helper method to mark all form controls as touched
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/control-types', this.osType]);
  }

  // Метод для копирования текста в буфер обмена
  copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(
      () => {
        this.snackBar.open('Copied to clipboard!', 'Close', {
          duration: 2000,
        });
      },
      (err) => {
        console.error('Could not copy text: ', err);
        this.snackBar.open('Failed to copy to clipboard', 'Close', {
          duration: 2000,
          panelClass: ['error-snackbar']
        });
      }
    );
  }

  // Методы для копирования конкретных типов YAML
  copyAUE(): void {
    this.copyToClipboard(this.generatedAUE);
  }

  copyStandard(): void {
    this.copyToClipboard(this.generatedStandard);
  }

  copyRequirementFile(): void {
    this.copyToClipboard(this.getSelectedRequirementFileContent());
  }
}