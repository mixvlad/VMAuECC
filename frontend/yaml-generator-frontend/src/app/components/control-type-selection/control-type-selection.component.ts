import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { ControlType } from "../../models/control-type";
import { ControlTypeService } from "../../services/control-type.service";

@Component({
  selector: "app-control-type-selection",
  templateUrl: "./control-type-selection.component.html",
  styleUrls: ["./control-type-selection.component.scss"],
})
export class ControlTypeSelectionComponent implements OnInit {
  osType: string = "";
  controlTypes: ControlType[] = [];
  language: string = "en"; // По умолчанию английский
  isLoading: boolean = false;
  error: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private controlTypeService: ControlTypeService,
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      this.osType = params["os"];
      this.loadControlTypes();
    });

    // Можно добавить получение предпочитаемого языка из localStorage или из настроек пользователя
    const savedLanguage = localStorage.getItem("preferredLanguage");
    if (savedLanguage) {
      this.language = savedLanguage;
    }
  }

  loadControlTypes(): void {
    this.isLoading = true;
    this.error = null;

    this.controlTypeService
      .getControlTypesByOs(this.osType, this.language)
      .subscribe({
        next: (data) => {
          this.controlTypes = data.controlTypes;
          this.isLoading = false;
        },
        error: (err) => {
          console.error("Error loading control types:", err);
          this.error = "Failed to load control types. Please try again.";
          this.isLoading = false;
        },
      });
  }

  selectControlType(controlTypeId: string): void {
    this.router.navigate(["/yaml-generator", this.osType, controlTypeId]);
  }

  goBack(): void {
    this.router.navigate(["/"]);
  }

  changeLanguage(language: string): void {
    this.language = language;
    localStorage.setItem("preferredLanguage", language);
    this.loadControlTypes();
  }
}
