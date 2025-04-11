import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ControlType, ControlTypeWithParameters, OsControlTypes } from '../models/control-type';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ControlTypeService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getControlTypes(osType: string, language: string = 'en'): Observable<OsControlTypes[]> {
    return this.http.get<OsControlTypes[]>(`${this.apiUrl}/api/ControlTypes/${osType}?language=${language}`);
  }

  getControlType(osType: string, controlTypeId: string, language: string = 'en'): Observable<ControlTypeWithParameters> {
    return this.http.get<ControlTypeWithParameters>(
      `${this.apiUrl}/api/ControlTypes/detail/${controlTypeId}?osType=${osType}&language=${language}`
    );
  }
}