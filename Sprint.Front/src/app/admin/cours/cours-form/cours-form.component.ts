// src/app/admin/cours/cours-form.component.ts
import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { LoaderComponent } from '../../../shared/components/loader/loader.component';
import { TabsComponent } from '../../../shared/components/tabs/tabs.component';

import { CoursService } from '../../../core/services/cours.service';
import { ModuleService } from '../../../core/services/module.service';
import { SprintService } from '../../../core/services/sprint.service';
import { NotificationService } from '../../../core/services/notification.service';

import { CoursDto, CreateCoursDto } from '../../../core/models/cours.model';
import { ModuleDto } from '../../../core/models/module.model';
import { SprintDto } from '../../../core/models/sprint.model';
import { Subscription } from 'rxjs';
import { OrderService } from '../../../core/services/order.services';



@Component({
  selector: 'app-cours-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AdminLayoutComponent,
    SearchBarComponent,
    LoaderComponent
  ],
  templateUrl: './cours-form.component.html',
  styleUrls: ['./cours-form.component.scss']
})
export class CoursFormComponent implements OnInit {
  isEditMode = signal(false);
  loading = signal(false);

  // Données du cours
  cours: CoursDto = {
    id: '',
    titre: '',
    description: '',
    ordre: 1,
    moduleId: null,
    moduleNom: '',
    quizzes: [],
    qaQuestions: [],
    progressions: []
  };

  // Données pour la sélection
  sprints = signal<SprintDto[]>([]);
  modules = signal<ModuleDto[]>([]);
  filteredModules = signal<ModuleDto[]>([]);

  selectedSprintId = signal<string | null>(null);
  sprintSearchTerm = signal('');
  moduleSearchTerm = signal('');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private coursService: CoursService,
    private moduleService: ModuleService,
    private sprintService: SprintService,
    private orderService: OrderService,
    private notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.isEditMode.set(!!id);

    if (this.isEditMode() && id) {
      this.loadCours(id);
    } else {
      this.initEmptyCours();
    }

    this.loadAllSprints();
    this.loadAllModules();
  }

  private initEmptyCours(): void {
  this.loading.set(true);

  // ❌ Ne PAS appeler getNextOrder ici
  this.cours = {
    id: '',
    titre: '',
    description: '',
    ordre: 1, // Valeur temporaire
    moduleId: null,
    moduleNom: '',
    quizzes: [],
    qaQuestions: [],
    progressions: []
  };

  // Charge les données de base
  this.loadAllSprints();
  this.loadAllModules();

  this.loading.set(false); // ✅ On peut arrêter le loader ici
}


  private loadCours(id: string): void {
    this.loading.set(true);
    this.coursService.getCoursById(id).subscribe({
      next: (data) => {
        this.cours = { ...data };
        // Pré-sélectionner le sprint via le module
        if (this.cours.moduleId) {
          this.moduleService.getModuleById(this.cours.moduleId).subscribe({
            next: (module) => {
              this.selectedSprintId.set(module.sprintId);
              this.filterModulesBySprint();
            },
            error: () => {
              this.selectedSprintId.set(null);
            }
          });
        }
        this.loading.set(false);
      },
      error: () => {
        this.notificationService.error('Impossible de charger le cours');
        this.loading.set(false);
        this.router.navigate(['/admin/cours']);
      }
    });
  }

  private loadAllSprints(): void {
    this.sprintService.getAllSprints().subscribe({
      next: (data) => {
        this.sprints.set(data);
      },
      error: () => {
        this.notificationService.warning('Impossible de charger les sprints');
      }
    });
  }

  private loadAllModules(): void {
    this.moduleService.getAllModules().subscribe({
      next: (data) => {
        this.modules.set(data);
        this.filteredModules.set(data);
        this.filterModulesBySprint(); // Appliquer filtre initial
      },
      error: () => {
        this.notificationService.warning('Impossible de charger les modules');
      }
    });
  }

  onSprintSearch(term: string): void {
    this.sprintSearchTerm.set(term);
  }

  onModuleSearch(term: string): void {
    this.moduleSearchTerm.set(term);
    this.filterModulesBySprint();
  }

  onSprintChange(sprintId: string | null): void {
    this.selectedSprintId.set(sprintId);
    this.cours.moduleId = null; // Réinitialiser le module
    this.filterModulesBySprint();
  }

  filterModulesBySprint(): void {
    let filtered = this.modules();

    const sprintId = this.selectedSprintId();
    if (sprintId) {
      filtered = filtered.filter(m => m.sprintId === sprintId);
    }

    const term = this.moduleSearchTerm().toLowerCase();
    if (term) {
      filtered = filtered.filter(m =>
        m.nom.toLowerCase().includes(term) ||
        m.description?.toLowerCase().includes(term)
      );
    }

    this.filteredModules.set(filtered);
  }

  onSubmit(): void {
    if (!this.cours.titre || !this.cours.ordre || this.cours.ordre < 1) {
      this.notificationService.error('Veuillez remplir les champs obligatoires.');
      return;
    }

    if (!this.cours.moduleId) {
      this.notificationService.error('Veuillez sélectionner un module.');
      return;
    }

    this.loading.set(true);

    const payload: CreateCoursDto = {
      titre: this.cours.titre,
      description: this.cours.description || undefined,
      ordre: this.cours.ordre,
      moduleId: this.cours.moduleId
    };

    let request$;
    if (this.isEditMode() && this.cours.id) {
      request$ = this.coursService.updateCours(this.cours.id, payload);
    } else {
      request$ = this.coursService.createCours(payload);
    }

    request$.subscribe({
      next: (saved) => {
        this.notificationService.success(
          `Cours "${saved.titre}" ${this.isEditMode() ? 'mis à jour' : 'créé'} avec succès`
        );
        this.router.navigate(['/admin/cours']);
      },
      error: () => {
        this.notificationService.error(
          `Échec ${this.isEditMode() ? 'de la mise à jour' : 'de la création'} du cours`
        );
        this.loading.set(false);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/cours']);
  }

  // Pour afficher le nom du sprint dans le select
  getSprintName(sprintId: string): string {
    return this.sprints().find(s => s.id === sprintId)?.nom || 'Sprint inconnu';
  }
}