import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { SalesInvoiceEditorComponent } from './sales-invoice-editor.component';

describe('SalesInvoiceEditorComponent', () => {
  let component: SalesInvoiceEditorComponent;
  let fixture: ComponentFixture<SalesInvoiceEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SalesInvoiceEditorComponent, HttpClientTestingModule],
    })
    .compileComponents();

    fixture = TestBed.createComponent(SalesInvoiceEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
