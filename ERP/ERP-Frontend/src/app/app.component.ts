import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { FeaturesSectionComponent } from '../components/Landing_Page/FeaturesSectionComponent/features-section.component';
import { HeroComponent } from '../components/Landing_Page/Hero_Component/hero.component';
import { HeaderComponent } from '../components/Landing_Page/Layout/Header_Component/header.component';
import { FooterComponent } from '../components/Landing_Page/Footer_Component/footer.component';
import { ModelContainerComponent } from '../core/services/modelContainerComponent';

@Component({
  selector: 'app-root',
  imports: [ ModelContainerComponent, FooterComponent, NgIf,HeroComponent, HeaderComponent, FeaturesSectionComponent],
  templateUrl: '../app/app.component.html',
  styleUrls: ['../app/app.component.scss'],
  standalone: true, // Set to false if this component is part of a module
})
export class AppComponent {
  title = 'ERP-Frontend';
}
