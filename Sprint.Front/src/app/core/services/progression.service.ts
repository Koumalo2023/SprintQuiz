// src/app/core/services/progression.service.ts
import { Injectable } from '@angular/core';
import {  Observable } from 'rxjs';  
import { AuthService } from './auth.service';
import { NiveauEnum } from '../models/enums.models';
import { ProfilService } from './profil.service';

@Injectable({ providedIn: 'root' })
export class ProgressionService {
  private currentUserId: string | null = null;
  private cache = new Map<string, number>(); // Clé: `${niveau}-${niveauId}-${userId}`

  constructor(private authService: AuthService, private profilService: ProfilService) {
    const user = this.authService.getCurrentUser();
    this.currentUserId = user?.id || null;
  }

  /**
   * Récupère la progression d'un utilisateur pour un niveau donné
   * @param niveau Sprint | Module | Cours
   * @param niveauId ID du sprint/module/cours
   * @param niveauNom Nom du niveau (optionnel)
   * @returns Pourcentage de progression (0-100)
   */
  getProgress(
    niveau: NiveauEnum,
    niveauId: string,
    niveauNom?: string
  ): Observable<number> {
    return new Observable<number>((observer) => {
      const cacheKey = `${niveau}-${niveauId}-${this.currentUserId}`;
      const cached = this.cache.get(cacheKey);
      if (cached !== undefined) {
        observer.next(cached);
        observer.complete();
        return;
      }

      if (!this.currentUserId) {
        observer.next(0);
        observer.complete();
        return;
      }

      // Recherche dans les données de progression de l'utilisateur
      this.profilService.getMyProgression().subscribe({
        next: (progressions) => {
          const prog = progressions.find(
            (p) =>
              p.utilisateurId === this.currentUserId &&
              p.niveau === niveau &&
              p.niveauId === niveauId
          );

          const progress = prog?.pourcentageComplet ?? 0;
          this.cache.set(cacheKey, progress);
          observer.next(progress);
          observer.complete();
        },
        error: () => {
          observer.next(0);
          observer.complete();
        }
      });
    });
  }

  /**
   * Met à jour le cache (utile après une action utilisateur)
   * @param niveau
   * @param niveauId
   * @param progress
   */
  updateProgress(niveau: NiveauEnum, niveauId: string, progress: number): void {
    const cacheKey = `${niveau}-${niveauId}-${this.currentUserId}`;
    this.cache.set(cacheKey, progress);
  }

  /**
   * Réinitialise le cache (ex: après déconnexion)
   */
  clearCache(): void {
    this.cache.clear();
  }
}