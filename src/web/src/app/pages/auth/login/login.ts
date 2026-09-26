import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { Observable } from 'rxjs';
import { MsalService  } from '@azure/msal-angular';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  private router = inject(Router);
  private auth = inject(MsalService );
  
  hidePassword = true;

  login() {
    this.auth.loginRedirect().subscribe({
      next: (result) => console.log('success', result),
      error: (err) => console.error('error', err)
    });
  }

  logout() {
    this.auth.logoutPopup();
  }
}