import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface SalesInvoice {
  id: string;
  customerName: string;
  date: string;
  amount: number;
  // other fields as needed
}

interface Payment {
  id: string;
  nominalTradeDebtor: string;
  date: string;
  amount: number;
  description?: string;
  // other fields as needed
}

@Component({
  selector: 'app-trade-debtors',
  templateUrl: './trade-debtors.component.html',
  styleUrls: ['./trade-debtors.component.scss']
})
export class TradeDebtorsComponent implements OnInit {
  salesInvoices: SalesInvoice[] = [];
  payments: Payment[] = [];
  filteredSalesInvoices: SalesInvoice[] = [];
  filteredPayments: Payment[] = [];
  userId: string = 'placeholder-user-id'; // TODO: Replace with actual user ID from auth service
  filterTerm: string = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchSalesInvoices();
    this.fetchPayments();
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

  fetchPayments(): void {
    this.http.get<Payment[]>(`/api/bankstatement/user/${this.userId}`) // Fetch bank statements by user ID
      .subscribe({
        next: (bankStatements) => {
          // Extract transactions from bank statements
          this.payments = [];
          bankStatements.forEach((bs: any) => {
            if (bs.transactions) {
              this.payments.push(...bs.transactions);
            }
          });
          this.applyFilters();
        },
        error: (error) => {
          console.error('Failed to fetch payments', error);
        }
      });
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    const term = this.filterTerm.toLowerCase();

    this.filteredSalesInvoices = this.salesInvoices.filter(invoice =>
      invoice.customerName.toLowerCase().includes(term)
    );

    this.filteredPayments = this.payments.filter(payment =>
      (payment.nominalTradeDebtor && payment.nominalTradeDebtor.toLowerCase().includes(term)) ||
      (payment.description && payment.description.toLowerCase().includes(term))
    );
  }
}
