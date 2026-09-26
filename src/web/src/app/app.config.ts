import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { MsalInterceptor, MsalModule } from '@azure/msal-angular';
import { InteractionType, PublicClientApplication } from '@azure/msal-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { environment } from '../environments/environment';


export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(
      withInterceptorsFromDi()
    ),
    importProvidersFrom(
      MsalModule.forRoot(
        new PublicClientApplication(environment.msalConfig),
        {
          interactionType: InteractionType.Redirect,
          authRequest: {scopes: [environment.apiScope]}
        },
        {
          interactionType: InteractionType.Redirect,
          protectedResourceMap: new Map([
            [`${environment.apiUrl}/*`, [environment.apiScope]]
          ])
        }
        
      )
    ),
    { provide: HTTP_INTERCEPTORS, useClass: MsalInterceptor, multi: true }
  ]
};
