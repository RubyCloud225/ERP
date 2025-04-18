import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from '../login/login.component';
import { DocumentListComponent } from '../document-list/document-list.component';
import { DocumentUploadComponent } from '../document-upload/document-upload.component';
import { NgModule } from '@angular/core';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'documentlist', component: DocumentListComponent },
    { path: 'documentUpload', component: DocumentUploadComponent }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }
