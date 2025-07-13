import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';

@Component({
  selector: 'app-sign-up',
  imports: [ReactiveFormsModule, NgIf, HttpClientModule],
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss'],
  standalone: true, // Set to false if this component is part of a module
})
export class SignUpComponent implements OnInit{
  signupForm: FormGroup;

  constructor(private fb: FormBuilder, private http: HttpClient, private router: Router) {
    this.signupForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    });
  }
  ngOnInit(): void {}
  onSubmit(): void {
    if (this.signupForm.valid) {
      this.http.post('http://localhost:3000/api/signup', this.signupForm.value)
      .subscribe((response: any) => {
        console.log(response);
        this.router.navigate(['/login']);
      }, error => {
        console.log(error);
      });
    }
  }
}
