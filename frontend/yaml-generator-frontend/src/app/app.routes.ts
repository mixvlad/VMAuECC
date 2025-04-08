import { Routes } from "@angular/router";
import { OsSelectionComponent } from "./components/os-selection/os-selection.component";
import { ControlTypeSelectionComponent } from "./components/control-type-selection/control-type-selection.component";
import { YamlFormComponent } from "./components/yaml-form/yaml-form.component";

export const routes: Routes = [
  { path: "", component: OsSelectionComponent },
  {
    path: "control-type-selection/:os",
    component: ControlTypeSelectionComponent,
  },
  { path: "yaml-generator/:os/:controlTypeId", component: YamlFormComponent },
  { path: "**", redirectTo: "" },
];
