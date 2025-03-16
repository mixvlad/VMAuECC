import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { YamlFormComponent } from './components/yaml-form/yaml-form.component';

const routes: Routes = [
  { path: '', redirectTo: '/yaml-generator', pathMatch: 'full' },
  { path: 'yaml-generator', component: YamlFormComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
