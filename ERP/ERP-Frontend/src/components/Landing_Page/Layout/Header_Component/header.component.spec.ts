import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HeaderComponent } from './header.component';
import { ModelService } from '../../../../core/services/model.service';
import { LoginModelComponent } from '../../../../models/login_signup/login/login-model.component';
import { of } from 'rxjs';
import { DebugElement } from '@angular/core';
import { By } from '@angular/platform-browser';
import { NgIf } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';

class MockModelService {
  openModal(component: any, config: any) {
    return of({ success: true, data: {} });
  }
}

describe('HeaderComponent', () => {
  let component: HeaderComponent;
  let fixture: ComponentFixture<HeaderComponent>;
  let modelService: ModelService;
  let debugElement: DebugElement;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [{ provide: ModelService, useClass: MockModelService }],
      imports: [HttpClientTestingModule, NgIf], // Import HttpClientTestingModule for HTTP requests in the component
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HeaderComponent);
    component = fixture.componentInstance;
    modelService = TestBed.inject(ModelService);
    debugElement = fixture.debugElement;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show login button when not logged in', () => {
    component.isLoggedIn = false;
    fixture.detectChanges();
    const loginButton = debugElement.query(By.css('.login-button'));
    expect(loginButton).toBeTruthy();
    const userInfo = debugElement.query(By.css('.user-info'));
    expect(userInfo).toBeNull();
  });

  it('should show user info when logged in', () => {
    component.isLoggedIn = true;
    fixture.detectChanges();
    const loginButton = debugElement.query(By.css('.login-button'));
    expect(loginButton).toBeNull();
    const userInfo = debugElement.query(By.css('.user-info'));
    expect(userInfo).toBeTruthy();
  });

  it('should call openLoginModel and update isLoggedIn on success', () => {
    spyOn(modelService, 'openModal').and.returnValue(of({ success: true, data: {} }));
    component.isLoggedIn = false;
    component.openLoginModel();
    expect(modelService.openModal).toHaveBeenCalledWith(LoginModelComponent, { initialEmail: 'test@example.com' });
    // Since openModal returns observable, we need to wait for subscription to complete
    fixture.detectChanges();
    expect(component.isLoggedIn).toBeTrue();
  });

  it('should set isLoggedIn to false if login not successful', () => {
    spyOn(modelService, 'openModal').and.returnValue(of({ success: false }));
    component.isLoggedIn = true;
    component.openLoginModel();
    fixture.detectChanges();
    expect(component.isLoggedIn).toBeFalse();
  });
});
