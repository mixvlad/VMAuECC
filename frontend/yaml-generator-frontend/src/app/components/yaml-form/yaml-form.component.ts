import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute, Router } from "@angular/router";
import { YamlService } from "../../services/yaml.service";
import { CollectorConfig } from "../../models/collector-config";
import { ControlType } from "../../models/control-type";
import { ControlTypeService } from "../../services/control-type.service";

@Component({
  selector: "app-yaml-form",
  templateUrl: "./yaml-form.component.html",
  styleUrls: ["./yaml-form.component.scss"],
})
export class YamlFormComponent implements OnInit {
  form: FormGroup;
  generatedYaml: string = "";
  osType: string = "";
  controlTypeId: string = "";
  selectedControlType: ControlType | undefined;
  isLoading: boolean = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private yamlService: YamlService,
    private snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private router: Router,
    private controlTypeService: ControlTypeService,
  ) {
    this.form = this.fb.group({
      collectItems: ["", Validators.required],
      targetPath: ["", Validators.required],
      intervalSeconds: [60, [Validators.required, Validators.min(1)]],
      includeSubdirectories: [false],
      filePatterns: [""],
      customParameters: [""],
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.osType = params["os"];
      this.controlTypeId = params["controlTypeId"];
      this.loadControlTypeDetails();
    });
  }

  loadControlTypeDetails(): void {
    if (!this.osType || !this.controlTypeId) {
      this.router.navigate(["/"]);
      return;
    }

    this.isLoading = true;
    this.error = null;

    // Получаем язык из localStorage или используем английский по умолчанию
    const language = localStorage.getItem("preferredLanguage") || "en";

    this.controlTypeService
      .getControlTypesByOs(this.osType, language)
      .subscribe({
        next: (data) => {
          this.selectedControlType = data.controlTypes.find(
            (ct: ControlType) => ct.id === this.controlTypeId,
          );

          if (!this.selectedControlType) {
            this.router.navigate(["/control-type-selection", this.osType]);
          }

          this.isLoading = false;
        },
        error: (err) => {
          console.error("Error loading control type details:", err);
          this.error = "Failed to load control type details. Please try again.";
          this.isLoading = false;
        },
      });
  }

  onSubmit(): void {
    if (this.form.valid) {
      const formValue = this.form.value;
      const config: CollectorConfig = {
        osType: this.osType,
        controlType: this.controlTypeId,
        collectItems: formValue.collectItems
          .split(",")
          .map((item: string) => item.trim()),
        targetPath: formValue.targetPath,
        intervalSeconds: formValue.intervalSeconds,
        includeSubdirectories: formValue.includeSubdirectories,
        filePatterns: formValue.filePatterns
          .split(",")
          .map((pattern: string) => pattern.trim()),
        customParameters: this.parseCustomParameters(
          formValue.customParameters,
        ),
      };

      this.yamlService.generateYaml(config).subscribe({
        next: (yaml) => {
          this.generatedYaml = yaml;
          this.snackBar.open("YAML успешно сгенерирован", "Закрыть", {
            duration: 3000,
          });
        },
        error: (error) => {
          this.snackBar.open("Ошибка при генерации YAML", "Закрыть", {
            duration: 3000,
          });
          console.error("Error generating YAML:", error);
        },
      });
    }
  }

  private parseCustomParameters(input: string): { [key: string]: string } {
    const result: { [key: string]: string } = {};
    if (!input) return result;

    const pairs = input.split(",");
    pairs.forEach((pair) => {
      const [key, value] = pair.split(":").map((item) => item.trim());
      if (key && value) {
        result[key] = value;
      }
    });
    return result;
  }

  downloadYaml(): void {
    if (!this.generatedYaml) return;

    const blob = new Blob([this.generatedYaml], { type: "text/yaml" });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "collector-config.yaml";
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    window.URL.revokeObjectURL(url);
  }

  goBack(): void {
    this.router.navigate(["/control-type-selection", this.osType]);
  }
}
