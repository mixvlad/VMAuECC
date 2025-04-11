import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { CommonModule } from "@angular/common";

@Component({
    selector: "app-os-selection",
    templateUrl: "./os-selection.component.html",
    styleUrls: ["./os-selection.component.scss"],
    standalone: true,
    imports: [CommonModule]
})
export class OsSelectionComponent {
  constructor(private router: Router) {}

  selectOs(os: string): void {
    console.log('Selecting OS:', os); // Добавьте для отладки
    this.router.navigate(["/control-types", os]);
  }
}
