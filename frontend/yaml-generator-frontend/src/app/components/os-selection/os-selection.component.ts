import { Component, OnInit } from '@angular/core';
import { Router } from "@angular/router";
import { CommonModule } from "@angular/common";
import { LanguageService } from '../../services/language.service';

@Component({
      selector: "app-os-selection",
      templateUrl: "./os-selection.component.html",
      styleUrls: ["./os-selection.component.scss"],
      standalone: true,
      imports: [CommonModule]
})
export class OsSelectionComponent implements OnInit {
    currentLanguage: string = 'en';

    constructor(
      private router: Router,
      private languageService: LanguageService
    ) {}

    ngOnInit(): void {
      this.languageService.language$.subscribe(lang => {
        this.currentLanguage = lang;
      });
    }

    selectOs(os: string): void {
      console.log('Selecting OS:', os);
      this.router.navigate(["/control-types", os]);
    }
}