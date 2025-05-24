import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentUploadComponent } from '../document-upload/document-upload.component';
import { DocumentListComponent } from '../document-list/document-list.component';
import { AppComponent } from '../app/app.component';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { HttpClient } from '@angular/common/http';
import { LoginComponent } from '../login/login.component';
import { FormsModule } from '@angular/forms';
import { ApprovalDocumentClassComponent } from '../approval-document-class/approval-document-class.component';
import { SignUpComponent } from '../sign-up/sign-up.component';
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
