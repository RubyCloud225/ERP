import { RouterModule, Routes } from '@angular/router';
import { LoginModelComponent } from '../models/login_signup/login/login.component';
import { DocumentListComponent } from '../components/Agent/document-list/document-list.component';
import { DocumentUploadComponent } from '../components/Agent/document-upload/document-upload.component';
import { NgModule } from '@angular/core';

export const routes: Routes = [
    { path: 'login', component: LoginModelComponent },
    { path: 'documentlist', component: DocumentListComponent },
    { path: 'documentUpload', component: DocumentUploadComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
