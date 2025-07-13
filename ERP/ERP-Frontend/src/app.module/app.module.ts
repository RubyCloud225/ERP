import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppComponent } from '../app/app.component';
import { HeaderComponent } from '../components/Landing_Page/Layout/Header_Component/header.component';
import { HeroComponent } from '../components/Landing_Page/Hero_Component/hero.component';
import { FeaturesSectionComponent } from '../components/Landing_Page/FeaturesSectionComponent/features-section.component';
import { FooterComponent } from '../components/Landing_Page/Footer_Component/footer.component';
import { ModelContainerComponent } from '../core/services/modelContainerComponent';

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    AppComponent,
    HeaderComponent,
    HeroComponent,
    FeaturesSectionComponent,
    FooterComponent,
    ModelContainerComponent
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
