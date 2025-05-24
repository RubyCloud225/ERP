import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { jsPDF } from 'jspdf';
import html2canvas from 'html2canvas';

@Component({
  selector: 'app-sales-invoice-editor',
  imports: [],
  templateUrl: './sales-invoice-editor.component.html',
  styleUrl: './sales-invoice-editor.component.css'
})
export class SalesInvoiceEditorComponent {
  SalesInvoiceContent: string = '';
  constructor(private http: HttpClient) { }
  ngOnInit() {
    this.fetchInvoice();
  }
  fetchInvoice() {
    this.http.get<any>('http://localhost:3000/invoice/1')
    .subscribe(data => {
      this.SalesInvoiceContent = data.content;
    });
  }
  printInvoice() {
    const printWindow = window.open('', '_blank');
    printWindow!.document.write(`
      <html>
      <head>
      <title>Invoice</title>
      </head>
      <body>
      ${this.SalesInvoiceContent}
      </body>
      </html>`
    );
    printWindow!.document.close();
  }
  downloadPDF() {
    const pdf = new jsPDF();
    const invoiceElement = document.getElementById('salesInvoiceContent');
    html2canvas(invoiceElement!).then(canvas => {
      const imgData = canvas.toDataURL('image/png');
      const imgWidth = 190;
      const pageHeight = 290;
      const imgHeight = (canvas.height * 190) / canvas.width;
      let heightLeft = imgHeight;
      let position = 0;
      pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
      heightLeft -= pageHeight;
      while (heightLeft >= 0) {
        position = heightLeft - imgHeight;
        pdf.addPage();
        pdf.addImage(imgData, 'PNG', 0, position, imgWidth, imgHeight);
        heightLeft -= pageHeight;
      }
      pdf.save('sales-invoice.pdf');
    });
  }
}

