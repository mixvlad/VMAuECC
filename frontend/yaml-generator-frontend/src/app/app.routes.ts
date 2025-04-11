import { Routes } from '@angular/router';
import { OsSelectionComponent } from './components/os-selection/os-selection.component';
import { ControlTypeSelectionComponent } from './components/control-type-selection/control-type-selection.component';
import { YamlFormComponent } from './components/yaml-form/yaml-form.component';

export const routes: Routes = [
  { path: '', component: OsSelectionComponent },
  { path: 'control-types/:osType', component: ControlTypeSelectionComponent },
  { path: 'yaml-form/:osType/:controlTypeId', component: YamlFormComponent },
  { path: '**', redirectTo: '' }
];