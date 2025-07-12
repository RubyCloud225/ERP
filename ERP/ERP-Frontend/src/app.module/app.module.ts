import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentUploadComponent } from '../components/Agent/document-upload/document-upload.component';
import { DocumentListComponent } from '../components/Agent/document-list/document-list.component';
import { AppComponent } from '../app/app.component';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { HttpClient } from '@angular/common/http';
import { LoginComponent } from '../models/login_signup/login/login.component';
import { FormsModule } from '@angular/forms';
import { ApprovalDocumentClassComponent } from '../components/Agent/approval-document-class/approval-document-class.component';
import { SignUpComponent } from '../models/login_signup/sign-up/sign-up.component';
import { QuillModule } from 'ngx-quill';



@NgModule({
  declarations: [
    AppComponent,
    DocumentUploadComponent,
    DocumentListComponent,
    LoginComponent,
    ApprovalDocumentClassComponent,
    SignUpComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatCardModule,
    HttpClient,
    FormsModule,
    QuillModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
