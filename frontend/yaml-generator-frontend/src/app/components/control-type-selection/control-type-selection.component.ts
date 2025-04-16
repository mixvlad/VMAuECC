import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ControlTypeService } from '../../services/control-type.service';
import { LanguageService } from '../../services/language.service';
import { ControlType, OsControlTypes } from '../../models/control-type';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-control-type-selection',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    MatButtonModule, 
    MatIconModule,
    MatCardModule,
    TranslateModule
  ],
  templateUrl: './control-type-selection.component.html',
  styleUrls: ['./control-type-selection.component.scss']
})
export class ControlTypeSelectionComponent implements OnInit {
  osType: string = '';
  controlTypes: ControlType[] = [];
  isLoading: boolean = true;
  error: string = '';
  language: string = 'en';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private controlTypeService: ControlTypeService,
    private languageService: LanguageService
  ) { }
  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.osType = params['osType'];
      
      this.languageService.language$.subscribe(lang => {
        this.language = lang;
        this.loadControlTypes();
      });
    });
  }

  loadControlTypes(): void {
    this.isLoading = true;
    this.error = '';
    
    this.controlTypeService.getControlTypes(this.osType, this.language)
      .subscribe({
        next: (data: OsControlTypes[]) => {
          console.log('API Response:', data);
          
          if (Array.isArray(data)) {
            const osData = data.find(item => item.os.toLowerCase() === this.osType.toLowerCase());
            this.controlTypes = osData?.controlTypes || [];
          } 
          else if (data && typeof data === 'object') {
            // Use type assertion to tell TypeScript what type data is
            const typedData = data as unknown as OsControlTypes;
            this.controlTypes = typedData.controlTypes || [];
          }
          else {
            this.controlTypes = [];
            console.error('Unexpected data format:', data);
          }
          
          this.isLoading = false;
        },
        error: (err) => {
          this.error = 'Failed to load control types. Please try again.';
          this.isLoading = false;
          console.error('Error loading control types:', err);
        }
      });
  }

  selectControlType(id: string): void {
    this.router.navigate(['/yaml-form', this.osType, id]);
  }

  goBack(): void {
    this.router.navigate(['/']);
  }  
}
