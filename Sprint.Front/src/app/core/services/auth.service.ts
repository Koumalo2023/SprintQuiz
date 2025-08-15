import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { environment } from '../../../environments/environment';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponseDto, ChangePasswordDto, CreateUtilisateurDto, LoginDto, UpdateUtilisateurDto, UtilisateurDto } from '../models/auth.model';
import { HttpClient } from '@angular/common/http';
import { RoleUtilisateur } from '../models/enums.models';
import { isPlatformBrowser } from '@angular/common';

//src/app/core/services/auth.service.ts
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly API_URL = `${environment.apiUrl}/utilisateur`;

  private currentUserSubject = new BehaviorSubject<UtilisateurDto | null>(null);
  private tokenSubject = new BehaviorSubject<string | null>(null);

  currentUser$ = this.currentUserSubject.asObservable();
  token$ = this.tokenSubject.asObservable();

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.loadUserFromStorage();
    }
  }

  // --- Gestion d'état ---
  private setAuth(user: UtilisateurDto, token: string, expiresAt: Date): void {
    if (!isPlatformBrowser(this.platformId)) return;
    localStorage.setItem('jwt_token', token);
    localStorage.setItem('user_data', JSON.stringify(user));
    localStorage.setItem('token_expires_at', expiresAt.getTime().toString());
    this.currentUserSubject.next(user);
    this.tokenSubject.next(token);
    this.autoLogout(expiresAt);
  }

  private clearAuth(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user_data');
    localStorage.removeItem('token_expires_at');
    this.currentUserSubject.next(null);
    this.tokenSubject.next(null);
  }

  // --- Chargement initial ---
  private loadUserFromStorage(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const token = localStorage.getItem('jwt_token');
    const userData = localStorage.getItem('user_data');
    const expiresAtStr = localStorage.getItem('token_expires_at');

    if (!token || !userData || !expiresAtStr) return;

    try {
      const user = JSON.parse(userData) as UtilisateurDto;
      const expiresAt = parseInt(expiresAtStr, 10);

      if (Date.now() >= expiresAt) {
        this.logout();
        return;
      }

      this.currentUserSubject.next(user);
      this.tokenSubject.next(token);
      this.autoLogout(new Date(expiresAt));
    } catch (error) {
      console.error('Erreur lors du chargement des données utilisateur:', error);
      this.logout();
    }
  }

  // --- Auto-déconnexion ---
  private autoLogout(expiresAt: Date): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const now = new Date();
    const timeUntilExpire = expiresAt.getTime() - now.getTime();

    if (timeUntilExpire <= 0) {
      this.logout();
      return;
    }

    setTimeout(() => {
      this.logout();
    }, timeUntilExpire);
  }

  // --- Authentification ---
  login(loginData: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.API_URL}/login`, loginData).pipe(
      tap(response => {
        this.setAuth(response.utilisateur, response.token, new Date(response.expiresAt));
      })
    );
  }

  logout(): void {
    this.clearAuth();
  }

  isAuthenticated(): boolean {
    if (!isPlatformBrowser(this.platformId)) return false;

    const token = localStorage.getItem('jwt_token');
    const expiresAtStr = localStorage.getItem('token_expires_at');

    if (!token || !expiresAtStr) return false;

    const now = Date.now();
    const expiration = parseInt(expiresAtStr, 10);

    if (now >= expiration) {
      this.logout();
      return false;
    }

    return true;
  }

  register(userData: CreateUtilisateurDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(this.API_URL, userData).pipe(
      tap(response => {
        this.setAuth(response.utilisateur, response.token, new Date(response.expiresAt));
      })
    );
  }

  // --- Informations utilisateur ---
  getCurrentUser(): UtilisateurDto | null {
    return this.currentUserSubject.value;
  }

  hasRole(role: RoleUtilisateur): boolean {
    const user = this.getCurrentUser();
    return !!user && user.role === role;
  }

  isAdmin(): boolean {
    return this.hasRole(RoleUtilisateur.Admin);
  }

  isStudent(): boolean {
    return this.hasRole(RoleUtilisateur.Etudiant);
  }

  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem('jwt_token');
  }

  // --- Gestion des utilisateurs (Admin) ---
  getAllUsers(): Observable<UtilisateurDto[]> {
    return this.http.get<UtilisateurDto[]>(this.API_URL);
  }

  getUserById(id: string): Observable<UtilisateurDto> {
    return this.http.get<UtilisateurDto>(`${this.API_URL}/${id}`);
  }

  createUser(dto: CreateUtilisateurDto): Observable<UtilisateurDto> {
    return this.http.post<UtilisateurDto>(this.API_URL, dto);
  }

  updateUser(id: string, dto: UpdateUtilisateurDto): Observable<UtilisateurDto> {
    return this.http.put<UtilisateurDto>(`${this.API_URL}/${id}`, dto);
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  // --- Mise à jour du profil connecté ---
  updateCurrentUser(userData: Partial<UpdateUtilisateurDto>): Observable<UtilisateurDto> {
    return this.http.put<UtilisateurDto>(`${this.API_URL}/profil`, userData).pipe(
      tap(updatedUser => {
        const currentUser = this.getCurrentUser();
        if (currentUser && isPlatformBrowser(this.platformId)) {
          const mergedUser = { ...currentUser, ...updatedUser };
          this.currentUserSubject.next(mergedUser);
          localStorage.setItem('user_data', JSON.stringify(mergedUser));
        }
      })
    );
  }

  // --- Changement de mot de passe ---
  changePassword(dto: ChangePasswordDto): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/profil/password`, dto);
  }

  // --- Upload photo de profil ---
  uploadProfilePhoto(imageBase64: string): Observable<{ photoUrl: string }> {
    return this.http.post<{ photoUrl: string }>(`${this.API_URL}/profil/photo`, { image: imageBase64 }).pipe(
      tap(response => {
        const user = this.getCurrentUser();
        if (user && isPlatformBrowser(this.platformId)) {
          const updatedUser = { ...user, photoUrl: response.photoUrl };
          this.currentUserSubject.next(updatedUser);
          localStorage.setItem('user_data', JSON.stringify(updatedUser));
        }
      })
    );
  }
}