import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FormService {
  private apiUrl = 'https://localhost:7143/api'; // Your .NET API URL

  constructor(private http: HttpClient) {}

  // Helper to get JWT token from localStorage
  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  getFormMetadata(formId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/form/${formId}`, { headers: this.getHeaders() });
  }

  submitFormResponse(formId: number, data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/form/${formId}/submit`, data, { headers: this.getHeaders() });
  }
}