import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';


//src/app/auth/auth.routes.ts
export const authRoutes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', loadComponent: () => import('./register/register.component').then(m => m.RegisterComponent) },
  {
    path: 'forgot-password',
    loadComponent: () => import('./forgot-password/forgot-password.component').then(c => c.ForgotPasswordComponent)
  },
  {
    path: 'change-password',
    loadComponent: () => import('./change-password/change-password.component').then(c => c.ChangePasswordComponent)
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' }
];
