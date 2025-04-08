import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { OsControlTypes } from "../models/control-type";
import { environment } from "../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class ControlTypeService {
  private apiUrl = `${environment.apiUrl}/api/Yaml`;

  constructor(private http: HttpClient) {}

  getControlTypesByOs(
    osType: string,
    language: string = "en",
  ): Observable<OsControlTypes> {
    return this.http.get<OsControlTypes>(
      `${this.apiUrl}/GetAvailableGenMethods?osType=${osType}&language=${language}`,
    );
  }

  getAllControlTypes(language: string = "en"): Observable<OsControlTypes[]> {
    return this.http.get<OsControlTypes[]>(
      `${this.apiUrl}/GetAvailableGenMethods?language=${language}`,
    );
  }
}
