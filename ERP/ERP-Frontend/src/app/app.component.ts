import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: '../app/app.component.html',
  styleUrl: '../app/app.component.scss'
})
export class AppComponent {
  title = 'ERP-Frontend';
}
