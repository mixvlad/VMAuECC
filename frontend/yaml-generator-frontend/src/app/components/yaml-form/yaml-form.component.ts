import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { YamlService } from '../../services/yaml.service';
import { CollectorConfig } from '../../models/collector-config';

@Component({
    selector: 'app-yaml-form',
    templateUrl: './yaml-form.component.html',
    styleUrls: ['./yaml-form.component.scss']
})
export class YamlFormComponent {
    form: FormGroup;
    generatedYaml: string = '';

    constructor(
        private fb: FormBuilder,
        private yamlService: YamlService,
        private snackBar: MatSnackBar
    ) {
        this.form = this.fb.group({
            collectItems: ['', Validators.required],
            targetPath: ['', Validators.required],
            intervalSeconds: [60, [Validators.required, Validators.min(1)]],
            includeSubdirectories: [false],
            filePatterns: [''],
            customParameters: ['']
        });
    }

    onSubmit(): void {
        if (this.form.valid) {
            const formValue = this.form.value;
            const config: CollectorConfig = {
                collectItems: formValue.collectItems.split(',').map((item: string) => item.trim()),
                targetPath: formValue.targetPath,
                intervalSeconds: formValue.intervalSeconds,
                includeSubdirectories: formValue.includeSubdirectories,
                filePatterns: formValue.filePatterns.split(',').map((pattern: string) => pattern.trim()),
                customParameters: this.parseCustomParameters(formValue.customParameters)
            };

            this.yamlService.generateYaml(config).subscribe({
                next: (yaml) => {
                    this.generatedYaml = yaml;
                    this.snackBar.open('YAML успешно сгенерирован', 'Закрыть', { duration: 3000 });
                },
                error: (error) => {
                    this.snackBar.open('Ошибка при генерации YAML', 'Закрыть', { duration: 3000 });
                    console.error('Error generating YAML:', error);
                }
            });
        }
    }

    private parseCustomParameters(input: string): { [key: string]: string } {
        const result: { [key: string]: string } = {};
        if (!input) return result;

        const pairs = input.split(',');
        pairs.forEach(pair => {
            const [key, value] = pair.split(':').map(item => item.trim());
            if (key && value) {
                result[key] = value;
            }
        });
        return result;
    }

    downloadYaml(): void {
        if (!this.generatedYaml) return;

        const blob = new Blob([this.generatedYaml], { type: 'text/yaml' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'collector-config.yaml';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }
} 