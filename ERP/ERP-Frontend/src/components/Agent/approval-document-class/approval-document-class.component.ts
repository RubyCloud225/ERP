import { Component } from '@angular/core';
import { DocumentService } from './DocumentService';
import { NgIf, NgForOf } from '@angular/common';

@Component({
  selector: 'app-approval-document-class',
  imports: [NgIf, NgForOf],
  templateUrl: './approval-document-class.component.html',
  styleUrls: ['./approval-document-class.component.css']
})
export class ApprovalDocumentClassComponent {
  classifiedDocuments: any[] = [];
  errorMessage: string | null = null;

  constructor(private documentService: DocumentService) {}

  ngOnInit(): void {
    this.loadClassifiedDocuments();
  }
  loadClassifiedDocuments(): void {
    this.documentService.getClassifiedDocuments().subscribe(
      (data) => {
        this.classifiedDocuments = data;
      },
      (error) => {
        console.error('Error fetching documents', error);
        this.errorMessage = error;
      }
    );
  }

  approveDocument(documentId: string, category: string): void {
    this.documentService.approveDocument(documentId, category).subscribe(
      (response) => {
        console.log('Document approved successfully', response);
        this.loadClassifiedDocuments();
      },
      (error) => {
        console.error('Error approving document', error);
        this.errorMessage = error;
      }
    );
  }
}
