import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable } from 'rxjs';
import { RoleUtilisateur } from '../models/enums.models';

@Injectable({
  providedIn: 'root'  
})
export class AdminGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    
    if (this.authService.isAuthenticated() && this.authService.hasRole(RoleUtilisateur.Admin)) {
      return true;
    } else {
      // Rediriger vers le dashboard si pas admin ou non authentifié
      return this.router.createUrlTree(['/user/dashboard']);
    }
  }
}

