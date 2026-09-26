import { CanActivateFn, Router, Routes } from '@angular/router';
import { inject } from '@angular/core';

import { LoginComponent } from './pages/auth/login/login';
import { LogoutComponent } from './pages/auth/logout/logout';
import { AppShellComponent } from './layout/app-shell/app-shell';

import { Dashboard } from './pages/dashboard/dashboard';
import { Collection } from './pages/collection/collection';
import { Store } from './pages/store/store';
import { MsalGuard, MsalService } from '@azure/msal-angular';

const basePathGuard: CanActivateFn = () => {
  const router = inject(Router);
  const msal = inject(MsalService);

  return router.parseUrl(
    msal.instance.getAllAccounts().length > 0 ? '/dashboard' : '/login'
  );
};

export const routes: Routes = [
  {
    path: '',
    component: LoginComponent,
    canActivate: [basePathGuard]
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'logout',
    component: LogoutComponent
  },
  {
    path: '',
    component: AppShellComponent,
    children: [
      {
        path: 'dashboard',
        component: Dashboard,
        canActivate: [MsalGuard]
      },
      {
        path: 'collection',
        component: Collection,
        canActivate: [MsalGuard]
      },
      {
        path: 'store',
        component: Store,
        canActivate: [MsalGuard]
      }
    ]
  }
];
