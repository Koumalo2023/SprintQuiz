// src/app/core/guards/role.guard.ts

import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  Router,
  UrlTree,
} from '@angular/router';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';

import { AuthService } from '../services/auth.service'; 
import { RoleUtilisateur } from '../models/enums.models';

@Injectable({
  providedIn: 'root',
})
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    // Récupérer le rôle requis depuis les données de la route
    const requiredRole: RoleUtilisateur = route.data['role'];

    // Si aucun rôle requis, on autorise
    if (!requiredRole) {
      return true;
    }

    return this.authService.currentUser$.pipe(
      take(1), // On ne prend qu'une seule émission
      map((user) => {
        if (user && user.role === requiredRole) {
          return true; // ✅ Accès autorisé
        } else {
          // ❌ Refusé → redirection
          return this.router.createUrlTree(['/login']);
        }
      })
    );
  }
}