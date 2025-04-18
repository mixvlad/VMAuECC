import { Component, OnInit } from '@angular/core';
import { LanguageService } from '../../services/language.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  currentLanguage: string = 'en';

  constructor(private languageService: LanguageService, private router: Router) { }

  ngOnInit(): void {
    this.languageService.language$.subscribe(lang => {
      this.currentLanguage = lang;
    });
  }

  changeLanguage(lang: string): void {
    this.languageService.setLanguage(lang);
  }

  navigateToHome() {
    this.router.navigate(['/']);
  }
}