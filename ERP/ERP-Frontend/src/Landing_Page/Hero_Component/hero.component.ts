import { Component } from '@angular/core';
import { ModelService } from '../../core/services/model.service';
import { LoginModelComponent } from '../../models/login_signup/login/login.component';
import { SignUpComponent } from '../../models/login_signup/sign-up/sign-up.component';

@Component({
  selector: 'app-hero',
  templateUrl: './hero.component.html',
  styleUrls: ['./hero.component.scss']
})
export class HeroComponent {
  constructor(private modelService: ModelService) {}

  openLoginModel(): void {
    this.modelService.openModal(LoginModelComponent, {}).subscribe((result: any) => {
      if (result && result.success) {
        console.log('Login successful', result.data);
      } else {
        console.log('Login not successful');
      }
    });
  }

  openSignUpModel(): void {
    this.modelService.openModal(SignUpComponent as any, {}).subscribe((result: any) => {
      if (result && result.success) {
        console.log('Sign up successful', result.data);
      } else {
        console.log('Sign up not successful');
      }
    });
  }
}
