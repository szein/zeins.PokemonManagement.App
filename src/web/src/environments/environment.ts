export const environment = {
  production: false,
  msalConfig: {
    auth: {
      clientId:"e828b9b6-48b9-4d01-bcc6-8ca7dbf7c6f6",
      authority:"https://zeins.ciamlogin.com/b1a86dd4-0adc-4a99-a182-920e55a2a363",
      knownAuthorities: ['zeins.ciamlogin.com'],
      redirectUri: 'http://localhost:4200/',
      postLogoutRedirectUri: 'http://localhost:4200/logout'
    }
  },
    system: {
       loggerOptions: {
         logLevel: 'Verbose' // see what's happening
       }
     },
  apiScope: 'api://fbd6924a-387a-4673-aa63-693d198ee022/user_access',
  apiUrl: 'http://localhost:5054'
};