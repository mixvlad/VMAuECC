import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CollectorConfig } from '../models/collector-config';

@Injectable({
    providedIn: 'root'
})
export class YamlService {
    private apiUrl = '/api/yaml';

    constructor(private http: HttpClient) { }

    generateYaml(config: CollectorConfig): Observable<string> {
        return this.http.post<string>(`${this.apiUrl}/generate`, config);
    }
} 