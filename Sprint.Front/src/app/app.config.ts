import {  APP_INITIALIZER, ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideHttpClient, withFetch, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { AdminGuard } from './core/guards/admin.guard';
import { EnumService } from './core/services/enum.service';
import { lastValueFrom } from 'rxjs';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(
      withFetch(), 
      withInterceptors([authInterceptor])
    ),
    {
      provide: APP_INITIALIZER,
      useFactory: (enumService: EnumService) => {
        return () => {
          if (typeof localStorage !== 'undefined') {
            return lastValueFrom(enumService.loadConfig());
          }
          return Promise.resolve();
        };
      },
      deps: [EnumService],
      multi: true
    }
     
  ]
};

