import { Component, EventEmitter, Output } from '@angular/core';
import { HttpClient, HttpEventType } from '@angular/common/http';

@Component({
  selector: 'app-document-import',
  templateUrl: './document-import.component.html',
  styleUrls: ['./document-import.component.scss']
})
export class DocumentImportComponent {
  @Output() uploadComplete = new EventEmitter<any>();

  isDragging = false;
  uploadProgress: number | null = null;
  uploadResponse: any = null;
  errorMessage: string | null = null;

  constructor(private http: HttpClient) {}

  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
    if (event.dataTransfer?.files.length) {
      this.uploadFile(event.dataTransfer.files[0]);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      this.uploadFile(input.files[0]);
    }
  }

  uploadFile(file: File) {
    this.uploadProgress = 0;
    this.errorMessage = null;
    const formData = new FormData();
    formData.append('document', file);

    this.http.post('/api/document/import', formData, {
      reportProgress: true,
      observe: 'events'
    }).subscribe({
      next: event => {
        if (event.type === HttpEventType.UploadProgress && event.total) {
          this.uploadProgress = Math.round(100 * event.loaded / event.total);
        } else if (event.type === HttpEventType.Response) {
          this.uploadResponse = event.body;
          this.uploadComplete.emit(this.uploadResponse);
          this.uploadProgress = null;
        }
      },
      error: err => {
        this.errorMessage = 'Upload failed. Please try again.';
        this.uploadProgress = null;
      }
    });
  }
}
