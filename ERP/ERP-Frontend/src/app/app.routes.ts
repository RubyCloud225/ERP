import { RouterModule, Routes } from '@angular/router';
import { LoginModelComponent } from '../models/login_signup/login/login.component';
import { DocumentListComponent } from '../components/Agent/document-list/document-list.component';
import { DocumentUploadComponent } from '../components/Agent/document-upload/document-upload.component';
import { ApprovalDocumentClassComponent } from '../components/Agent/approval-document-class/approval-document-class.component';
import { ModelContainerComponent } from '../core/services/modelContainerComponent';
import { SalesInvoiceEditorComponent } from '../components/Agent/Finance/sales-invoice-editor/sales-invoice-editor.component';
import { FeaturesSectionComponent } from '../components/Landing_Page/FeaturesSectionComponent/features-section.component';
import { FooterComponent } from '../components/Landing_Page/Footer_Component/footer.component';
import { HeroComponent } from '../components/Landing_Page/Hero_Component/hero.component';
import { HeaderComponent } from '../components/Landing_Page/Layout/Header_Component/header.component';
import { SignUpComponent } from '../models/login_signup/sign-up/sign-up.component';
import { NgModule } from '@angular/core';

export const routes: Routes = [
    { path: 'login', component: LoginModelComponent },
    { path: 'documentlist', component: DocumentListComponent },
    { path: 'documentUpload', component: DocumentUploadComponent },
    { path: 'approvalDocumentClass', component: ApprovalDocumentClassComponent },
    { path: '', component: ModelContainerComponent },
    { path: 'salesInvoiceEditor', component: SalesInvoiceEditorComponent },
    { path: 'features', component: FeaturesSectionComponent },
    { path: 'footer', component: FooterComponent },
    { path: 'hero', component: HeroComponent },
    { path: 'header', component: HeaderComponent },
    { path: 'signup', component: SignUpComponent },

];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
