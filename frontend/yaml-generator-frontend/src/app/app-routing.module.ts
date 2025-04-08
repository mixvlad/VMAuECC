import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { YamlFormComponent } from "./components/yaml-form/yaml-form.component";
import { OsSelectionComponent } from "./components/os-selection/os-selection.component";
import { ControlTypeSelectionComponent } from "./components/control-type-selection/control-type-selection.component";

const routes: Routes = [
  { path: "", component: OsSelectionComponent },
  {
    path: "control-type-selection/:os",
    component: ControlTypeSelectionComponent,
  },
  { path: "yaml-generator/:os/:controlTypeId", component: YamlFormComponent },
  { path: "**", redirectTo: "" },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
