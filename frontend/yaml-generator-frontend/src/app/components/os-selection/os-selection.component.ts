import { Component } from "@angular/core";
import { Router } from "@angular/router";

@Component({
  selector: "app-os-selection",
  templateUrl: "./os-selection.component.html",
  styleUrls: ["./os-selection.component.scss"],
})
export class OsSelectionComponent {
  constructor(private router: Router) {}

  selectOs(os: string): void {
    this.router.navigate(["/control-type-selection", os]);
  }
}
