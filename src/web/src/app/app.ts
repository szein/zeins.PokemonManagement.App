import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { MsalService } from '@azure/msal-angular';

@Component({
  imports: [RouterOutlet],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App implements OnInit {
  protected readonly title = signal('angular-app');
  private authService = inject(MsalService);
  private router = inject(Router);

  ngOnInit() {
    this.authService.handleRedirectObservable()
      .subscribe({
        next: (result) => {
          if (!result?.account) return;

          this.authService.instance.setActiveAccount(result.account);
          void this.router.navigateByUrl('/dashboard');
        },
        error: (err) => {
          console.error('error', err);
          void this.router.navigateByUrl('/login');
        }
      });
  }
}
