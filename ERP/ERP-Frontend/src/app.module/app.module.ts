import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentImportComponent } from '../components/document-import/document-import.component';
import { DocumentUploadComponent } from '../components/Agent/document-upload/document-upload.component';
import { DocumentListComponent } from '../components/Agent/document-list/document-list.component';
import { AppComponent } from '../app/app.component';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
/* Removed import of ApprovalDocumentClassComponent as it is not found */
import { SignUpComponent } from '../models/login_signup/sign-up/sign-up.component';
import { QuillModule } from 'ngx-quill';

@NgModule({
  declarations: [
    AppComponent,
    DocumentUploadComponent,
    DocumentListComponent,
    SignUpComponent,
    DocumentImportComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatCardModule,
    HttpClientModule,
    FormsModule,
    QuillModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
