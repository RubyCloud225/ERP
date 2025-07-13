import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { DocumentListService } from './document-list.service';

describe('DocumentListService', () => {
  let service: DocumentListService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule], // Import HttpClientTestingModule for HTTP requests in the service
      providers: [DocumentListService] // Provide the service for testing
    });
    service = TestBed.inject(DocumentListService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
