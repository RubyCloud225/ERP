import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { PotentialSalesComponent } from './potential-sales.component';

describe('PotentialSalesComponent', () => {
  let component: PotentialSalesComponent;
  let fixture: ComponentFixture<PotentialSalesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PotentialSalesComponent],
      imports: [FormsModule]
    }).compileComponents();

    fixture = TestBed.createComponent(PotentialSalesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should alert if required fields are missing or invalid', () => {
    spyOn(window, 'alert');

    component.potentialSale = {
      customerName: '',
      opportunityName: '',
      stageId: 1,
      estimatedValue: 0,
      closeDate: ''
    };

    component.submitSale();

    expect(window.alert).toHaveBeenCalledWith('Please fill in all fields with valid values.');
  });

  it('should submit potential sale with valid data', () => {
    spyOn(window, 'alert');
    spyOn(console, 'log');

    component.potentialSale = {
      customerName: 'Customer A',
      opportunityName: 'Opportunity A',
      stageId: 2,
      estimatedValue: 1000,
      closeDate: '2023-12-31'
    };

    component.submitSale();

    expect(console.log).toHaveBeenCalledWith('Submitting potential sale:', component.potentialSale);
    expect(window.alert).toHaveBeenCalledWith('Potential sale submitted successfully.');
    expect(component.potentialSale.customerName).toBe('');
  });
});
