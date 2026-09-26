import { bootstrapApplication } from '@angular/platform-browser';
import { MsalModule, MsalInterceptor } from '@azure/msal-angular';
import { PublicClientApplication } from '@azure/msal-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { environment } from './environments/environment';

const msalInstance = new PublicClientApplication(environment.msalConfig);

msalInstance.initialize().then(() => {
bootstrapApplication(App, appConfig)
  .catch((err) => console.error(err));
});
