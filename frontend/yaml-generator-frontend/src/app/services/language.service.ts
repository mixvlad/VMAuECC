import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  private languageSubject = new BehaviorSubject<string>('en');
  public language$ = this.languageSubject.asObservable();

  constructor(private translateService: TranslateService) {
    // Initialize with browser language or default to English
    const browserLang = navigator.language.split('-')[0];
    const defaultLang = browserLang.match(/en|ru/) ? browserLang : 'en';
    this.setLanguage(defaultLang);
  }

  setLanguage(lang: string): void {
    this.languageSubject.next(lang);
    this.translateService.use(lang);
  }

  getCurrentLanguage(): string {
    return this.languageSubject.value;
  }
}
