import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-document-upload',
  templateUrl: './document-upload.component.html',
  styleUrl: './document-upload.component.css'
})
export class DocumentUploadComponent {
  selectedFile: File[] = [];
  isDragOver: boolean = false;
  constructor(private http: HttpClient) { }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedFile = Array.from(input.files);
    }
  }
  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = true;
  }
  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = false;
  }
  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDragOver = false;
    if (event.dataTransfer?.files) {
      this.selectedFile = Array.from(event.dataTransfer.files);
    }
  }
  uploadFile(): void {
    if (this.selectedFile.length == 0) {
      return;
    }
    const formData = new FormData();
    this.selectedFile.forEach((file: string | Blob) => {
      formData.append('file', file);
    });

    this.http.post('http://localhost:3000/upload', formData).subscribe(
      response => {
        console.log('File uploaded Successfully', response);
      },
      error => {
        console.error('Error while uploading file', error);
      }
    );
  }
}
