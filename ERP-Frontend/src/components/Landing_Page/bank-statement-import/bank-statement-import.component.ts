import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-bank-statement-import',
  templateUrl: './bank-statement-import.component.html',
  styleUrls: ['./bank-statement-import.component.scss']
})
export class BankStatementImportComponent {
  selectedFile: File | null = null;
  isUploading: boolean = false;
  bankStatementDocuments: any[] = [];
  isLoadingDocuments: boolean = false;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadBankStatementDocuments();
  }

  loadBankStatementDocuments(): void {
    this.isLoadingDocuments = true;
    this.http.get<any[]>('/api/documents?category=bankstatement').subscribe({
      next: (docs) => {
        this.bankStatementDocuments = docs;
        this.isLoadingDocuments = false;
      },
      error: (error) => {
        console.error('Failed to load bank statement documents', error);
        this.isLoadingDocuments = false;
      }
    });
  }

  onFileSelected(event: any): void {
    this.selectedFile = event.target.files[0] ?? null;
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      alert('Please select a file first.');
      return;
    }

    if (!confirm('Are you sure you want to upload this bank statement?')) {
      return;
    }

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    this.isUploading = true;

    this.http.post('/api/bankstatement/import', formData).subscribe({
      next: () => {
        alert('Bank statement uploaded successfully.');
        this.selectedFile = null;
        this.isUploading = false;
        this.loadBankStatementDocuments();
      },
      error: (error) => {
        alert('Failed to upload bank statement.');
        console.error(error);
        this.isUploading = false;
      }
    });
  }

  importDocument(documentId: string): void {
    if (!confirm('Are you sure you want to import this bank statement document?')) {
      return;
    }

    this.isUploading = true;

    this.http.post(`/api/bankstatement/import/${documentId}`, {}).subscribe({
      next: () => {
        alert('Bank statement document imported successfully.');
        this.isUploading = false;
        this.loadBankStatementDocuments();
      },
      error: (error) => {
        alert('Failed to import bank statement document.');
        console.error(error);
        this.isUploading = false;
      }
    });
  }
}
