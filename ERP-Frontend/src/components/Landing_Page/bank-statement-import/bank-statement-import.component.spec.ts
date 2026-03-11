import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { BankStatementImportComponent } from './bank-statement-import.component';

describe('BankStatementImportComponent', () => {
  let component: BankStatementImportComponent;
  let fixture: ComponentFixture<BankStatementImportComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BankStatementImportComponent],
      imports: [HttpClientTestingModule]
    }).compileComponents();

    fixture = TestBed.createComponent(BankStatementImportComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should upload file after user confirmation', () => {
    spyOn(window, 'confirm').and.returnValue(true);
    spyOn(window, 'alert');

    const file = new File(['dummy content'], 'test.csv', { type: 'text/csv' });
    component.selectedFile = file;

    component.uploadFile();

    const req = httpMock.expectOne('/api/bankstatement/import');
    expect(req.request.method).toBe('POST');
    req.flush({});

    expect(window.alert).toHaveBeenCalledWith('Bank statement uploaded successfully.');
    expect(component.selectedFile).toBeNull();
  });

  it('should not upload file if user cancels confirmation', () => {
    spyOn(window, 'confirm').and.returnValue(false);
    spyOn(window, 'alert');

    const file = new File(['dummy content'], 'test.csv', { type: 'text/csv' });
    component.selectedFile = file;

    component.uploadFile();

    httpMock.expectNone('/api/bankstatement/import');
    expect(window.alert).not.toHaveBeenCalled();
  });

  it('should alert if no file selected', () => {
    spyOn(window, 'alert');

    component.selectedFile = null;
    component.uploadFile();

    expect(window.alert).toHaveBeenCalledWith('Please select a file first.');
  });
});
