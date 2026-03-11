import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { LoginModelComponent } from './login-model.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('LoginModelComponent', () => {
  let component: LoginModelComponent;
  let fixture: ComponentFixture<LoginModelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ ReactiveFormsModule, HttpClientTestingModule, LoginModelComponent ],
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginModelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have invalid form when empty', () => {
    expect(component.loginForm.valid).toBeFalsy();
  });

  it('should validate email field', () => {
    let email = component.loginForm.controls['email'];
    email.setValue('');
    expect(email.valid).toBeFalsy();
    email.setValue('invalid-email');
    expect(email.valid).toBeFalsy();
    email.setValue('test@example.com');
    expect(email.valid).toBeTruthy();
  });

  it('should validate password field', () => {
    let password = component.loginForm.controls['password'];
    password.setValue('');
    expect(password.valid).toBeFalsy();
    password.setValue('short');
    expect(password.valid).toBeFalsy();
    password.setValue('longenoughpassword');
    expect(password.valid).toBeTruthy();
  });
});
