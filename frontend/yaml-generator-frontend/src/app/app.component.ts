import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <div class="app-container">
      <mat-toolbar color="primary">
        <span>YAML Generator</span>
      </mat-toolbar>
      <div class="content">
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styles: [`
    .app-container {
      height: 100vh;
      display: flex;
      flex-direction: column;
    }
    .content {
      flex: 1;
      padding: 20px;
      background-color: #f5f5f5;
    }
  `]
})
export class AppComponent {
  title = 'YAML Generator';
}
