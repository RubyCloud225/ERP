import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/AuthService'; // Updated to AuthService
import { ModelContent } from '../../../core/services/model.service';
import { NgIf } from '@angular/common'; // Import NgIf for conditional rendering

@Component({
  selector: 'app-login-model', // This selector is used to identify the component in templates
  templateUrl: './login-model.component.html', // This template is used to render the component
  styleUrls: ['./login-model.component.scss'], // This stylesheet is used to style the component
  standalone: true, // Set to false if this component is part of a module
  imports: [ReactiveFormsModule, NgIf] // Add necessary imports here if needed
})
export class LoginModelComponent implements OnInit, ModelContent {
  loginForm!: FormGroup; // FormGroup to manage the login form state
  errorMessage: string | null = null; // Variable to hold error messages
  isLoading: boolean = false; // Variable to manage loading state
  close!: (result?: any) => void; // Function to close the model, provided by the ModelService
  constructor(
    private fb: FormBuilder,
    private authService: AuthService // AuthService to handle authentication logic
  ) {}

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]], // Email field with validation
      password: ['', [Validators.required, Validators.minLength(8)]] // Password field with validation
    }); // Initialize the login form with FormBuilder
  }
  get email() {
    return this.loginForm.get('email'); // Getter for email form control
  }
  get password() {
    return this.loginForm.get('password'); // Getter for password form control
  }
  onSubmit(): void {
    this.errorMessage = null; // Reset error message
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched(); // Mark all form controls as touched to show validation errors
      return;
    }
    this.isLoading = true; // Set loading state to true
    const { email, password } = this.loginForm.value; // Extract email and password from the form value
    // Call the login method from AuthService with email and password
    this.authService.login(email, password).subscribe({
      next: (response) => {
        this.isLoading = false; // Set loading state to false
        console.log('Login successful', response); // Log success message
        this.close(); // Close the model after successful login
      },
      error: (err) => {
        this.isLoading = false; // Set loading state to false
        this.errorMessage = err.error.message || 'Login failed'; // Set error message from the response or a default message
        console.error('Login failed', err); // Log error message
      }
    });
  }
  onClose(): void {
    this.close(); // Function to close the model, provided by the ModelService
  }
  openSignupModelFromLogin(): void {
    // This function is used to open the signup model from the login model
    console.log('Opening signup model from login model');
    this.close({ action: 'openSignup'}); // Close the login model
  }

  // Added method to trigger OAuth login
  oauthLogin(): void {
    this.authService.oauthLogin();
  }
}
