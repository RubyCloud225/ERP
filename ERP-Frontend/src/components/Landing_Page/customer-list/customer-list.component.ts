import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface SalesInvoice {
  id: string;
  customerName: string;
  date: string;
  // other fields as needed
}

interface Transaction {
  id: string;
  customerName: string;
  date: string;
  description: string;
  amount: number;
  // other fields as needed
}

@Component({
  selector: 'app-customer-list',
  templateUrl: './customer-list.component.html',
  styleUrls: ['./customer-list.component.scss']
})
export class CustomerListComponent implements OnInit {
  salesInvoices: SalesInvoice[] = [];
  transactions: Transaction[] = [];
  filteredInvoices: SalesInvoice[] = [];
  filteredTransactions: Transaction[] = [];
  searchTerm: string = '';
  filterDate: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchSalesInvoices();
    this.fetchTransactions();
  }

  fetchSalesInvoices(): void {
    this.http.get<SalesInvoice[]>('/api/salesinvoice') // Adjust API endpoint as needed
      .subscribe({
        next: (data) => {
          this.salesInvoices = data;
          this.applyFilters();
        },
        error: (error) => {
          console.error('Failed to fetch sales invoices', error);
        }
      });
  }

  fetchTransactions(): void {
    this.http.get<Transaction[]>('/api/transactions') // Adjust API endpoint as needed
      .subscribe({
        next: (data) => {
          this.transactions = data;
          this.applyFilters();
        },
        error: (error) => {
          console.error('Failed to fetch transactions', error);
        }
      });
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onDateChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    const term = this.searchTerm.toLowerCase();
    const dateFilter = this.filterDate;

    this.filteredInvoices = this.salesInvoices.filter(invoice =>
      invoice.customerName.toLowerCase().includes(term) &&
      (dateFilter ? invoice.date.startsWith(dateFilter) : true)
    );

    this.filteredTransactions = this.transactions.filter(transaction =>
      transaction.customerName.toLowerCase().includes(term) &&
      (dateFilter ? transaction.date.startsWith(dateFilter) : true)
    );
  }
}
