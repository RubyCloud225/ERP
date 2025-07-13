import { Component } from '@angular/core';
import { DocumentListService } from './document-list.service';
import { NgIf, NgForOf } from '@angular/common';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-document-list',
  imports: [NgIf, NgForOf, MatCardModule],
  templateUrl: './document-list.component.html',
  styleUrls: ['./document-list.component.css'],
  standalone: true, // Set to false if this component is part of a module
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
