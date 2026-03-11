import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface PurchaseInvoice {
  id: string;
  supplierName: string;
  date: string;
  amount: number;
  paid: boolean;
  // other fields as needed
}

@Component({
  selector: 'app-purchase-invoices',
  templateUrl: './purchase-invoices.component.html',
  styleUrls: ['./purchase-invoices.component.scss']
})
export class PurchaseInvoicesComponent implements OnInit {
  purchaseInvoices: PurchaseInvoice[] = [];
  filteredPurchaseInvoices: PurchaseInvoice[] = [];
  filterDate: string = '';
  filterSupplier: string = '';
  filterPaidStatus: string = ''; // 'paid', 'unpaid', or ''

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.fetchPurchaseInvoices();
  }

  fetchPurchaseInvoices(): void {
    this.http.get<PurchaseInvoice[]>('/api/purchaseinvoice') // Adjust API endpoint as needed
      .subscribe({
        next: (data) => {
          this.purchaseInvoices = data;
          this.applyFilters();
        },
        error: (error) => {
          console.error('Failed to fetch purchase invoices', error);
        }
      });
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    const dateFilter = this.filterDate;
    const supplierFilter = this.filterSupplier.toLowerCase();
    const paidStatusFilter = this.filterPaidStatus;

    this.filteredPurchaseInvoices = this.purchaseInvoices.filter(invoice => {
      const matchesDate = dateFilter ? invoice.date.startsWith(dateFilter) : true;
      const matchesSupplier = supplierFilter ? invoice.supplierName.toLowerCase().includes(supplierFilter) : true;
      const matchesPaidStatus = paidStatusFilter === 'paid' ? invoice.paid === true :
                                paidStatusFilter === 'unpaid' ? invoice.paid === false : true;
      return matchesDate && matchesSupplier && matchesPaidStatus;
    });
  }
}
