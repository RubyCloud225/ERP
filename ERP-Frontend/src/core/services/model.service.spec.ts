import { TestBed } from '@angular/core/testing';
import { ModelService } from './model.service';
import { ViewContainerRef } from '@angular/core';
import { ReplaySubject } from 'rxjs';

describe('ModelService', () => {
  let service: ModelService;
  let mockVcr: jasmine.SpyObj<ViewContainerRef>;

  beforeEach(() => {
    mockVcr = jasmine.createSpyObj('ViewContainerRef', ['clear', 'createComponent']);
    TestBed.configureTestingModule({
      providers: [ModelService]
    });
    service = TestBed.inject(ModelService);
    service.setModalContainer(mockVcr);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  class FakeModelComponent {
    data: any;
    close!: (result?: any) => void;
  }
  it('should open a modal and emit result', (done) => {
    const closeSubject = new ReplaySubject<any>(1);
    const fakeComponentRef = {
      instance: { close: closeSubject.asObservable() },
      destroy: jasmine.createSpy('destroy')
    } as any;
  
    mockVcr.createComponent.and.returnValue(fakeComponentRef);
  
    const testValue = { data: 'test' };
  
    service.openModal(FakeModelComponent).subscribe({
      next: (result) => {
        console.log('Got result:', result);
        expect(result).toEqual(testValue);
        expect(mockVcr.createComponent).toHaveBeenCalledTimes(1);
        expect(fakeComponentRef.destroy).toHaveBeenCalled();
        done();
      },
      error: (err) => {
        fail(`Observable errored: ${err}`);
        done();
      }
    });
    // Simulate closing the modal with a result
    fakeComponentRef.instance.close(testValue);
  });

  it('should close all modals', () => {
    service.closeAllModels();
    expect(mockVcr.clear).toHaveBeenCalled();
  });

  // Add more tests for observables and modal lifecycle
});