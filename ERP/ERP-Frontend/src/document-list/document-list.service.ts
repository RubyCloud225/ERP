import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DocumentListService {
  private apiUrl = 'http://localhost:3000/documents'; // Local server API
  constructor(private http: HttpClient) {}
  getDocuments(): Observable<any> {
    return this.http.get(this.apiUrl);
  }
}
