import { Component } from '@angular/core';

interface SalesStage {
  id: number;
  name: string;
}

interface PotentialSale {
  customerName: string;
  opportunityName: string;
  stageId: number;
  estimatedValue: number;
  closeDate: string;
}

@Component({
  selector: 'app-potential-sales',
  templateUrl: './potential-sales.component.html',
  styleUrls: ['./potential-sales.component.scss']
})
export class PotentialSalesComponent {
  salesStages: SalesStage[] = [
    { id: 1, name: 'Prospecting' },
    { id: 2, name: 'Qualification' },
    { id: 3, name: 'Needs Analysis' },
    { id: 4, name: 'Proposal' },
    { id: 5, name: 'Negotiation' },
    { id: 6, name: 'Closed Won' },
    { id: 7, name: 'Closed Lost' }
  ];

  potentialSale: PotentialSale = {
    customerName: '',
    opportunityName: '',
    stageId: 1,
    estimatedValue: 0,
    closeDate: ''
  };

  isSubmitting: boolean = false;

  submitSale(): void {
    if (!this.potentialSale.customerName || !this.potentialSale.opportunityName || !this.potentialSale.closeDate || this.potentialSale.estimatedValue <= 0) {
      alert('Please fill in all fields with valid values.');
      return;
    }

    this.isSubmitting = true;

    // For now, just log the sale data. Backend integration can be added later.
    console.log('Submitting potential sale:', this.potentialSale);

    alert('Potential sale submitted successfully.');

    this.potentialSale = {
      customerName: '',
      opportunityName: '',
      stageId: 1,
      estimatedValue: 0,
      closeDate: ''
    };

    this.isSubmitting = false;
  }
}
