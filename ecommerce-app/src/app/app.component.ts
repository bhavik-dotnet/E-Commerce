// app.component.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FooterComponent } from './components/footer/footer.component'; // adjust path

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, FooterComponent],
  template: `
    <router-outlet></router-outlet>
    <app-footer></app-footer> <!-- Footer added here -->
  `,
  styles: []
})
export class AppComponent {
  title = 'ecommerce-app';
  
}
