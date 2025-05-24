import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class DocumentService {
    private apiUrl = 'http://localhost:3000/documents';
    constructor(private http: HttpClient) {}
    getClassifiedDocuments(): Observable<any> {
        return this.http.get(`${this.apiUrl}/classified`);
    }
    approveDocument(documentId: string, catagory: string): Observable<any> {
        return this.http.post(`${this.apiUrl}/approve`, { documentId, catagory });
    }
}