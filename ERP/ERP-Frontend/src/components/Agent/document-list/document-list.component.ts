import { Component } from '@angular/core';
import { DocumentListService } from './document-list.service';

@Component({
  selector: 'app-document-list',
  imports: [],
  templateUrl: './document-list.component.html',
  styleUrl: './document-list.component.css'
})
export class DocumentListComponent {
  documents: any[] = [];
  constructor(private documentListService: DocumentListService) {}
  ngOnInit(): void {
    this.documentListService.getDocuments().subscribe((data) => {
      this.documents = data;
    },
    (error) => {
      console.error('Error fetching documents:', error);
    });
  }
}
