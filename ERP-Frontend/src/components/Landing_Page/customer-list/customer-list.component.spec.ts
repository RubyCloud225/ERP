import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CustomerListComponent } from './customer-list.component';
import { FormsModule } from '@angular/forms';

describe('CustomerListComponent', () => {
  let component: CustomerListComponent;
  let fixture: ComponentFixture<CustomerListComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CustomerListComponent],
      imports: [HttpClientTestingModule, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(CustomerListComponent);
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

  it('should fetch sales invoices and transactions on init', () => {
    const mockSalesInvoices = [
      { id: '1', customerName: 'Customer A', date: '2023-01-01' },
      { id: '2', customerName: 'Customer B', date: '2023-01-02' }
    ];

    const mockTransactions = [
      { id: 't1', customerName: 'Customer A', date: '2023-01-03', description: 'Transaction 1', amount: 50 },
      { id: 't2', customerName: 'Customer C', date: '2023-01-04', description: 'Transaction 2', amount: 75 }
    ];

    const salesRequest = httpMock.expectOne('/api/salesinvoice');
    expect(salesRequest.request.method).toBe('GET');
    salesRequest.flush(mockSalesInvoices);

    const transactionsRequest = httpMock.expectOne('/api/transactions');
    expect(transactionsRequest.request.method).toBe('GET');
    transactionsRequest.flush(mockTransactions);

    expect(component.salesInvoices.length).toBe(2);
    expect(component.transactions.length).toBe(2);
  });

  it('should filter sales invoices and transactions based on searchTerm and filterDate', () => {
    component.salesInvoices = [
      { id: '1', customerName: 'Customer A', date: '2023-01-01' },
      { id: '2', customerName: 'Customer B', date: '2023-01-02' }
    ];

    component.transactions = [
      { id: 't1', customerName: 'Customer A', date: '2023-01-03', description: 'Transaction 1', amount: 50 },
      { id: 't2', customerName: 'Customer C', date: '2023-01-04', description: 'Transaction 2', amount: 75 }
    ];

    component.searchTerm = 'Customer A';
    component.filterDate = '2023-01-03';
    component.applyFilters();

    expect(component.filteredInvoices.length).toBe(0); // No invoice on 2023-01-03
    expect(component.filteredTransactions.length).toBe(1);
    expect(component.filteredTransactions[0].customerName).toBe('Customer A');
  });
});
