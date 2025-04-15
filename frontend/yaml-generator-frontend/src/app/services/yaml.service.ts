import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

interface YamlRequestData {
  controlTypeId: string;
  osType: string;
  parameters: { [key: string]: string };
  customParameters: { [key: string]: string }; // Changed to object instead of string
}

@Injectable({
  providedIn: 'root'
})
export class YamlService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  generateYaml(formData: any, controlTypeId: string, osType: string): Observable<string> {
    // Parse customParameters if it's a JSON string
    let customParamsObj: { [key: string]: string } = {};
    if (formData.customParameters) {
      try {
        customParamsObj = JSON.parse(formData.customParameters);
      } catch (e) {
        console.error('Error parsing customParameters as JSON:', e);
        // If parsing fails, we'll send an empty object
      }
    }

    // Prepare the request data
    const requestData: YamlRequestData = {
      controlTypeId: controlTypeId,
      osType: osType,
      parameters: {},
      customParameters: customParamsObj
    };
    
    // Add all parameters from form to parameters object
    for (const key in formData) {
      if (key !== 'customParameters') {
        requestData.parameters[key] = formData[key];
      }
    }
    
    // Wrap the request data in a config object as expected by the backend
    const request = requestData;
    
    return this.http.post(`${this.apiUrl}/api/Yaml/generate`, request, { responseType: 'text' });
  }
}