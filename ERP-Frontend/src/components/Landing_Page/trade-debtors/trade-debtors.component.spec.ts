import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TradeDebtorsComponent } from './trade-debtors.component';
import { FormsModule } from '@angular/forms';

describe('TradeDebtorsComponent', () => {
  let component: TradeDebtorsComponent;
  let fixture: ComponentFixture<TradeDebtorsComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TradeDebtorsComponent],
      imports: [HttpClientTestingModule, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(TradeDebtorsComponent);
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

  it('should fetch sales invoices and payments on init', () => {
    const mockSalesInvoices = [
      { id: '1', customerName: 'Customer A', date: '2023-01-01', amount: 100 },
      { id: '2', customerName: 'Customer B', date: '2023-01-02', amount: 200 }
    ];

    const mockBankStatements = [
      {
        transactions: [
          { id: 't1', nominalTradeDebtor: 'Customer A', date: '2023-01-03', amount: 50, description: 'Payment 1' },
          { id: 't2', nominalTradeDebtor: 'Customer C', date: '2023-01-04', amount: 75, description: 'Payment 2' }
        ]
      }
    ];

    const userId = component.userId;

    const salesRequest = httpMock.expectOne('/api/salesinvoice');
    expect(salesRequest.request.method).toBe('GET');
    salesRequest.flush(mockSalesInvoices);

    const paymentsRequest = httpMock.expectOne(`/api/bankstatement/user/${userId}`);
    expect(paymentsRequest.request.method).toBe('GET');
    paymentsRequest.flush(mockBankStatements);

    expect(component.salesInvoices.length).toBe(2);
    expect(component.payments.length).toBe(2);
  });

  it('should filter sales invoices and payments based on filterTerm', () => {
    component.salesInvoices = [
      { id: '1', customerName: 'Customer A', date: '2023-01-01', amount: 100 },
      { id: '2', customerName: 'Customer B', date: '2023-01-02', amount: 200 }
    ];

    component.payments = [
      { id: 't1', nominalTradeDebtor: 'Customer A', date: '2023-01-03', amount: 50, description: 'Payment 1' },
      { id: 't2', nominalTradeDebtor: 'Customer C', date: '2023-01-04', amount: 75, description: 'Payment 2' }
    ];

    component.filterTerm = 'Customer A';
    component.applyFilters();

    expect(component.filteredSalesInvoices.length).toBe(1);
    expect(component.filteredSalesInvoices[0].customerName).toBe('Customer A');

    expect(component.filteredPayments.length).toBe(1);
    expect(component.filteredPayments[0].nominalTradeDebtor).toBe('Customer A');
  });
});
