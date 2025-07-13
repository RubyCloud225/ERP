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
import { CustomerListComponent } from '../components/Landing_Page/customer-list/customer-list.component';
import { TradeDebtorsComponent } from '../components/Landing_Page/trade-debtors/trade-debtors.component';
import { PurchaseInvoicesComponent } from '../components/Landing_Page/purchase-invoices/purchase-invoices.component';
import { BankStatementImportComponent } from '../components/Landing_Page/bank-statement-import/bank-statement-import.component';
import { JournalEntryComponent } from '../components/Landing_Page/journal-entry/journal-entry.component';
import { PotentialSalesComponent } from '../components/Landing_Page/potential-sales/potential-sales.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    HeroComponent,
    FeaturesSectionComponent,
    FooterComponent,
    ModelContainerComponent,
    CustomerListComponent,
    TradeDebtorsComponent,
    PurchaseInvoicesComponent,
    BankStatementImportComponent,
    JournalEntryComponent,
    PotentialSalesComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
