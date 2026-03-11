import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { JournalEntryComponent } from './journal-entry.component';

describe('JournalEntryComponent', () => {
  let component: JournalEntryComponent;
  let fixture: ComponentFixture<JournalEntryComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [JournalEntryComponent],
      imports: [HttpClientTestingModule, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(JournalEntryComponent);
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

  it('should submit journal entry with valid data', () => {
    component.journalEntry = {
      date: '2023-01-01',
      description: 'Test entry',
      debitAccount: 'Account A',
      creditAccount: 'Account B',
      amount: 100
    };

    spyOn(window, 'alert');

    component.submitEntry();

    const req = httpMock.expectOne('/api/journalentry');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(component.journalEntry);
    req.flush({});

    expect(window.alert).toHaveBeenCalledWith('Journal entry submitted successfully.');
    expect(component.journalEntry.amount).toBe(0);
  });

  it('should alert if required fields are missing or invalid', () => {
    spyOn(window, 'alert');

    component.journalEntry = {
      date: '',
      description: '',
      debitAccount: '',
      creditAccount: '',
      amount: 0
    };

    component.submitEntry();

    expect(window.alert).toHaveBeenCalledWith('Please fill in all fields with valid values.');
  });

  it('should handle submission error', () => {
    component.journalEntry = {
      date: '2023-01-01',
      description: 'Test entry',
      debitAccount: 'Account A',
      creditAccount: 'Account B',
      amount: 100
    };

    spyOn(window, 'alert');

    component.submitEntry();

    const req = httpMock.expectOne('/api/journalentry');
    req.error(new ErrorEvent('Network error'));

    expect(window.alert).toHaveBeenCalledWith('Failed to submit journal entry.');
  });
});
