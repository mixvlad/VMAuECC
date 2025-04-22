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

  // Generate AUE YAML
  generateAUE(formData: any, controlTypeId: string, osType: string): Observable<string> {
    const requestData = this.prepareRequestData(formData, controlTypeId, osType);
    return this.http.post(`${this.apiUrl}/api/Yaml/generateAUE`, requestData, { responseType: 'text' });
  }

  // Download AUE YAML
  downloadAUE(formData: any, controlTypeId: string, osType: string): Observable<Blob> {
    const requestData = this.prepareRequestData(formData, controlTypeId, osType);
    return this.http.post(`${this.apiUrl}/api/Yaml/downloadAUE`, requestData, { 
      responseType: 'blob' 
    });
  }

  // Generate Standard YAML
  generateStandard(formData: any, controlTypeId: string, osType: string): Observable<string> {
    const requestData = this.prepareRequestData(formData, controlTypeId, osType);
    return this.http.post(`${this.apiUrl}/api/Yaml/generateStandard`, requestData, { responseType: 'text' });
  }

  // Download Standard YAML
  downloadStandard(formData: any, controlTypeId: string, osType: string): Observable<Blob> {
    const requestData = this.prepareRequestData(formData, controlTypeId, osType);
    return this.http.post(`${this.apiUrl}/api/Yaml/downloadStandard`, requestData, { 
      responseType: 'blob' 
    });
  }

  // Generate Requirement ZIP
  generateRequirement(formData: any, controlTypeId: string, osType: string): Observable<string> {
    const requestData = this.prepareRequestData(formData, controlTypeId, osType);
    return this.http.post(`${this.apiUrl}/api/Yaml/generateRequirement`, requestData, { responseType: 'text' });
  }

  // Download Requirement ZIP
  downloadRequirement(formData: any, controlTypeId: string, osType: string): Observable<Blob> {
    const requestData = this.prepareRequestData(formData, controlTypeId, osType);
    return this.http.post(`${this.apiUrl}/api/Yaml/downloadRequirement`, requestData, { 
      responseType: 'blob' 
    });
  }

  // Helper method to prepare request data
  private prepareRequestData(formData: any, controlTypeId: string, osType: string): YamlRequestData {
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
    
    return requestData;
  }
}