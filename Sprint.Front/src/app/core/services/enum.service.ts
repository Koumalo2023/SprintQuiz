// enum.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';

// --- Modèles ---
export interface EnumMetadata {
  label: string;
  color: string;
  icon: string;
  class: string;
}

export interface EnumConfig {
  NiveauEnum: Record<number, EnumMetadata>;
  NiveauDifficulte: Record<number, EnumMetadata>;
  RoleUtilisateur: Record<number, EnumMetadata>;
  TypeExercice: Record<number, EnumMetadata>;
}

// --- Enumérations ---
export enum NiveauEnum {
  Cours = 0,
  Module = 1,
  Sprint = 2
}

export enum NiveauDifficulte {
  Facile = 0,
  Moyen = 1,
  Difficile = 2
}

export enum RoleUtilisateur {
  Admin = 0,
  Etudiant = 1
}

export enum TypeExercice {
  Basique = 0,
  Applique = 1,
  Analyse = 2,
  Cas = 3,
  Defi = 4
}

@Injectable({
  providedIn: 'root'
})
export class EnumService {
  private config: EnumConfig | null = null;

  constructor(private http: HttpClient) {}

  // Charger la configuration au démarrage
  loadConfig(): Observable<EnumConfig> {
    return this.http.get<EnumConfig>('/assets/config/enums.config.json').pipe(
      tap(config => {
        this.config = config;
      }),
      catchError(err => {
        console.error('Erreur lors du chargement de enums.config.json', err);
        this.config = this.getDefaultConfig();
        return of(this.config);
      })
    );
  }

  // --- Valeurs par défaut ---
  private getDefaultConfig(): EnumConfig {
    return {
      NiveauEnum: {
        0: { label: 'Cours', color: '#4361ee', icon: 'book', class: 'badge-cours' },
        1: { label: 'Module', color: '#f72585', icon: 'layers', class: 'badge-module' },
        2: { label: 'Sprint', color: '#4cc9f0', icon: 'calendar', class: 'badge-sprint' }
      },
      NiveauDifficulte: {
        0: { label: 'Facile', color: '#22b24c', icon: 'star-half', class: 'badge-success' },
        1: { label: 'Moyen', color: '#f57a00', icon: 'star', class: 'badge-warning' },
        2: { label: 'Difficile', color: '#e83e8c', icon: 'star-full', class: 'badge-danger' }
      },
      RoleUtilisateur: {
        0: { label: 'Administrateur', color: '#6f42c1', icon: 'shield', class: 'badge-admin' },
        1: { label: 'Étudiant', color: '#17a2b8', icon: 'user', class: 'badge-etudiant' }
      },
      TypeExercice: {
        0: { label: 'Basique', color: '#adb5bd', icon: 'circle', class: 'badge-basic' },
        1: { label: 'À appliquer', color: '#007bff', icon: 'arrow-right', class: 'badge-apply' },
        2: { label: 'Analyse', color: '#6f42c1', icon: 'zoom-in', class: 'badge-analyze' },
        3: { label: 'Cas pratique', color: '#fd7e14', icon: 'file-text', class: 'badge-case' },
        4: { label: 'Défi', color: '#d63384', icon: 'flame', class: 'badge-challenge' }
      }
    };
  }

  // --- Méthodes d'accès aux métadonnées ---
  getNiveauMetadata(niveau: number): EnumMetadata {
    return this.getMetadata('NiveauEnum', niveau) || this.getDefaultMetadata();
  }

  getDifficulteMetadata(difficulte: number): EnumMetadata {
    return this.getMetadata('NiveauDifficulte', difficulte) || this.getDefaultMetadata();
  }

  getRoleMetadata(role: number): EnumMetadata {
    return this.getMetadata('RoleUtilisateur', role) || this.getDefaultMetadata();
  }

  getTypeExerciceMetadata(type: number): EnumMetadata {
    return this.getMetadata('TypeExercice', type) || this.getDefaultMetadata();
  }

  // Méthode générique
  private getMetadata<T extends keyof EnumConfig>(enumName: T, value: number): EnumMetadata | null {
    if (!this.config) return null;
    return this.config[enumName][value] || null;
  }

  // --- Méthodes utilitaires (pour les templates) ---
  getNiveauLabel(niveau: number): string {
    return this.getNiveauMetadata(niveau).label;
  }

  getNiveauColor(niveau: number): string {
    return this.getNiveauMetadata(niveau).color;
  }

  getNiveauIcon(niveau: number): string {
    return this.getNiveauMetadata(niveau).icon;
  }

  getNiveauClass(niveau: number): string {
    return this.getNiveauMetadata(niveau).class;
  }

  /**
 * Convertit un libellé de niveau (ex: "Sprint", "Module", "Cours") en sa valeur enum correspondante.
 * @param label string
 * @returns NiveauEnum | null
 */
mapNiveauLabelToValue(label: string): NiveauEnum | null {
  const normalized = label.trim().toLowerCase();
  const config = this.config?.NiveauEnum;

  if (!config) return null;

  // Parcourir les entrées du config pour trouver le bon label
  for (const [key, meta] of Object.entries(config)) {
    if (meta.label.toLowerCase() === normalized) {
      return +key as NiveauEnum; // +key convertit string en number
    }
  }

  return null;
}

  private getDefaultMetadata(): EnumMetadata {
    return {
      label: 'Inconnu',
      color: '#adb5bd',
      icon: 'help',
      class: 'badge-secondary'
    };
  }
}