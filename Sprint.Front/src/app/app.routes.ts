import { Routes } from '@angular/router';
import { RoleGuard } from './core/guards/roleGuard.guards';
import { RoleUtilisateur } from './core/models/enums.models';
import { AdminGuard } from './core/guards/admin.guard';

//src/app/app.routes.ts
export const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () => import('./auth/auth.routes').then(m => m.authRoutes)
  },
  {
    path: 'user',
    loadChildren: () => import('./user/user.routes').then(m => m.userRoutes)
  },
  {
    path: 'admin',
    loadChildren: () => import('./admin/admin.routes').then(m => m.adminRoutes),
     canActivate: [AdminGuard]
  },
  { path: '', redirectTo: '/user/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/user/dashboard' }
];
