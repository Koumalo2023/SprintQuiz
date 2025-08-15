// src/app/core/services/order.service.ts
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

import { SprintService } from './sprint.service';
import { ModuleService } from './module.service';
import { CoursService } from './cours.service'; 
import { SprintDto } from '../models/sprint.model';
import { ModuleDto } from '../models/module.model';


// 🔹 Type union pour les entités
export type EntityType = 'sprint' | 'module' | 'cours';

// 🔹 Interface unifiée pour éviter les surcharges
export interface OrderRequest {
  entityType: EntityType;
 parentId?: string | undefined; // requis pour 'module' et 'cours'
}

@Injectable({ providedIn: 'root' })
export class OrderService {
  constructor(
    private sprintService: SprintService,
    private moduleService: ModuleService,
    private coursService: CoursService
  ) {}

  /**
   * Renvoie le prochain ordre disponible pour une entité donnée
   * @param request { entityType, parentId? }
   * @returns Observable<number>
   */
  getNextOrder(request: OrderRequest): Observable<number> {
    const { entityType, parentId } = request;

    switch (entityType) {
      case 'sprint':
        return this.sprintService.getAllSprints().pipe(
          map(sprints => this.findNextOrder(sprints)),
          catchError(() => of(1))
        );

      case 'module':
        if (!parentId) {
          console.error('OrderService: parentId est requis pour les modules');
          return of(1);
        }
        return this.moduleService.getModulesBySprintId(parentId).pipe(
          map(modules => this.findNextOrder(modules)),
          catchError(() => of(1))
        );

      case 'cours':
        if (!parentId) {
          console.error('OrderService: parentId est requis pour les cours');
          return of(1);
        }
        return this.coursService.getCoursByModuleId(parentId).pipe(
          map(cours => this.findNextOrder(cours)),
          catchError(() => of(1))
        );

      default:
        return of(1);
    }
  }

  /**
   * Trouve le prochain ordre (max + 1) dans une liste d'entités
   */
  private findNextOrder<T extends { ordre?: number }>(entities: T[]): number {
    if (!entities || entities.length === 0) return 1;
    const maxOrder = entities
      .filter(e => e.ordre !== undefined && e.ordre !== null)
      .reduce((max, e) => (e.ordre! > max ? e.ordre! : max), 0);
    return maxOrder + 1;
  }
}