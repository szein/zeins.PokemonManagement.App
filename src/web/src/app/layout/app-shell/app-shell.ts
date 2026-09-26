import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';

import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
 import { MsalService  } from '@azure/msal-angular';

import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule
  ],
  templateUrl: './app-shell.html',
  styleUrls: ['./app-shell.scss']
})
export class AppShellComponent {
private router = inject(Router);
constructor(private auth: MsalService ) {}

logout() {
  this.auth.logoutRedirect().subscribe({
    next: (result) => console.log('logout success', result),
    error: (err) => console.log('error',err)
  })
}
}