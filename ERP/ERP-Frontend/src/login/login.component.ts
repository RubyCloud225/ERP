import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string | null = null;

  constructor(private http: HttpClient) {}

  onSubmit(): void {
    const credentials = {
      username: this.username,
      password: this.password
    };
    this.http.post('http://localhost:3000/login', credentials).subscribe(
      (response: any) => {
        console.log(response);
        // TODO: redirect to dashboard
      },
      (error: any) => {
        console.log(error);
      }
    )
  }
}
