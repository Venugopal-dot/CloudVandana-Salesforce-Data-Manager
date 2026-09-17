import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SalesforceService {

  private apiUrl = 'https://localhost:7262/api/salesforce';

  constructor(private http: HttpClient) { }

  getRecords(
    objectName: string,
    page: number = 1,
    pageSize: number = 20
  ): Observable<any> {

    return this.http.get<any>(
      `${this.apiUrl}/${objectName}?page=${page}&pageSize=${pageSize}`,
      {
        withCredentials: true
      }
    );
  }

  getFields(objectName: string): Observable<string[]> {

    return this.http.get<string[]>(
      `${this.apiUrl}/${objectName}/fields`,
      {
        withCredentials: true
      }
    );
  }

  createRecord(
    objectName: string,
    fields: any
  ): Observable<string> {

    return this.http.post(
      `${this.apiUrl}/${objectName}`,
      fields,
      {
        withCredentials: true,
        responseType: 'text'
      }
    );

  }
  getAuthStatus(): Observable<any> {

    return this.http.get<any>(
      'https://localhost:7262/api/auth/status',
      {
        withCredentials: true
      }
    );

  }

  updateRecord(
    objectName: string,
    id: string,
    fields: any
  ): Observable<string> {

    return this.http.put(
      `${this.apiUrl}/${objectName}/${id}`,
      fields,
      {
        withCredentials: true,
        responseType: 'text'
      }
    );
  }

  deleteRecord(
    objectName: string,
    id: string
  ): Observable<string> {

    return this.http.delete(
      `${this.apiUrl}/${objectName}/${id}`,
      {
        withCredentials: true,
        responseType: 'text'
      }
    );
  }

}