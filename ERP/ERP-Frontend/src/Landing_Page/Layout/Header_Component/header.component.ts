import { Component, OnInit } from '@angular/core';
import { ModelService } from '../../../core/services/model.service';
import { LoginModelComponent } from '../../../models/login_signup/login/login.component';
import { AuthStateService } from '../../../core/services/auth-state.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
    isLoggedIn: boolean = false; // Variable to track if the user is logged in
    constructor(private modelService: ModelService, private authStateService: AuthStateService) {}
    ngOnInit(): void {
        // Check if the user is logged in when the component initializes
        this.isLoggedIn = !!localStorage.getItem('token'); // Assuming token is stored in localStorage
        this.authStateService.setLoggedIn(this.isLoggedIn);
    }
    openLoginModel(): void {
        this.modelService.openModal(LoginModelComponent, { initialEmail: 'test@example.com'}).subscribe(result => {
            if (result && result.success) {
                console.log('Login successful', result.data);
                this.isLoggedIn = true; // Update the login status
                this.authStateService.setLoggedIn(true);
            } else {
                console.log('logging in not successful');
                this.isLoggedIn = false;
                this.authStateService.setLoggedIn(false);
            };
        })// Open the login model using ModelService
    }
}
