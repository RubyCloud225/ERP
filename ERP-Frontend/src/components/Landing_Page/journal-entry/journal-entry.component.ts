import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface JournalEntry {
  date: string;
  description: string;
  debitAccount: string;
  creditAccount: string;
  amount: number;
}

@Component({
  selector: 'app-journal-entry',
  templateUrl: './journal-entry.component.html',
  styleUrls: ['./journal-entry.component.scss']
})
export class JournalEntryComponent {
  journalEntry: JournalEntry = {
    date: '',
    description: '',
    debitAccount: '',
    creditAccount: '',
    amount: 0
  };
  isSubmitting: boolean = false;

  constructor(private http: HttpClient) {}

  submitEntry(): void {
    if (!this.journalEntry.date || !this.journalEntry.description || !this.journalEntry.debitAccount || !this.journalEntry.creditAccount || this.journalEntry.amount <= 0) {
      alert('Please fill in all fields with valid values.');
      return;
    }

    this.isSubmitting = true;

    this.http.post('/api/journalentry', this.journalEntry).subscribe({
      next: () => {
        alert('Journal entry submitted successfully.');
        this.journalEntry = {
          date: '',
          description: '',
          debitAccount: '',
          creditAccount: '',
          amount: 0
        };
        this.isSubmitting = false;
      },
      error: (error) => {
        alert('Failed to submit journal entry.');
        console.error(error);
        this.isSubmitting = false;
      }
    });
  }
}
