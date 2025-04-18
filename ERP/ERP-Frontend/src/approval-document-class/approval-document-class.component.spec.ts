import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApprovalDocumentClassComponent } from './approval-document-class.component';

describe('ApprovalDocumentClassComponent', () => {
  let component: ApprovalDocumentClassComponent;
  let fixture: ComponentFixture<ApprovalDocumentClassComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApprovalDocumentClassComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ApprovalDocumentClassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
