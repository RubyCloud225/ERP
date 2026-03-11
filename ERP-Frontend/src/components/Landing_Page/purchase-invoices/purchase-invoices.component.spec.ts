import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PurchaseInvoicesComponent } from './purchase-invoices.component';
import { FormsModule } from '@angular/forms';

describe('PurchaseInvoicesComponent', () => {
  let component: PurchaseInvoicesComponent;
  let fixture: ComponentFixture<PurchaseInvoicesComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PurchaseInvoicesComponent],
      imports: [HttpClientTestingModule, FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(PurchaseInvoicesComponent);
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

  it('should fetch purchase invoices on init', () => {
    const mockPurchaseInvoices = [
      { id: '1', supplierName: 'Supplier A', date: '2023-01-01', amount: 100, paid: true },
      { id: '2', supplierName: 'Supplier B', date: '2023-01-02', amount: 200, paid: false }
    ];

    const req = httpMock.expectOne('/api/purchaseinvoice');
    expect(req.request.method).toBe('GET');
    req.flush(mockPurchaseInvoices);

    expect(component.purchaseInvoices.length).toBe(2);
    expect(component.filteredPurchaseInvoices.length).toBe(2);
  });

  it('should filter purchase invoices based on filters', () => {
    component.purchaseInvoices = [
      { id: '1', supplierName: 'Supplier A', date: '2023-01-01', amount: 100, paid: true },
      { id: '2', supplierName: 'Supplier B', date: '2023-01-02', amount: 200, paid: false },
      { id: '3', supplierName: 'Supplier A', date: '2023-01-03', amount: 150, paid: false }
    ];

    component.filterDate = '2023-01-01';
    component.filterSupplier = 'Supplier A';
    component.filterPaidStatus = 'paid';
    component.applyFilters();

    expect(component.filteredPurchaseInvoices.length).toBe(1);
    expect(component.filteredPurchaseInvoices[0].id).toBe('1');
  });
});
