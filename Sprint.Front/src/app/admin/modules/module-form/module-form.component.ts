// src/app/admin/modules/module-form.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { LoaderComponent } from '../../../shared/components/loader/loader.component';
import { TabsComponent } from '../../../shared/components/tabs/tabs.component';

import { ModuleService } from '../../../core/services/module.service';
import { SprintService } from '../../../core/services/sprint.service';
import { NotificationService } from '../../../core/services/notification.service';

import { ModuleDto, CreateModuleDto, UpdateModuleDto } from '../../../core/models/module.model';
import { SprintDto } from '../../../core/models/sprint.model';

import { Subscription } from 'rxjs';
import { OrderService } from '../../../core/services/order.services';

interface Tab {
  label: string;
  disabled: boolean;
}

@Component({
  selector: 'app-module-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AdminLayoutComponent,
    SearchBarComponent,
    TabsComponent,
    LoaderComponent
  ],
  templateUrl: './module-form.component.html',
  styleUrls: ['./module-form.component.scss']
})
export class ModuleFormComponent implements OnInit, OnDestroy {
  isEditMode = false;
  loading = false;

  // Données du module
  module: ModuleDto = {
    id: '',
    nom: '',
    description: '',
    ordre: 1,
    sprintId: '',
    cours: [],
    quizzes: [],
    qaQuestions: [],
    progressions: []
  };

  // Pour la sélection du sprint
  allSprints: SprintDto[] = [];
  filteredSprints: SprintDto[] = [];
  searchTermSprint = '';

  // Onglets
  tabs: Tab[] = [
    { label: 'Informations', disabled: false },
    { label: 'Import JSON', disabled: false }
  ];
  activeTab = 0;

  private subs = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private moduleService: ModuleService,
    private sprintService: SprintService,
    private orderService: OrderService,
    private notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!id;

    if (this.isEditMode && id) {
      this.loadModule(id);
    } else {
      this.initEmptyModule();
    }

    this.loadAllSprints();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  private initEmptyModule(): void {
    this.loading = true;

    // Si pas de sprintId, on ne peut pas calculer l'ordre → on met 1 par défaut
    this.module = {
      id: '',
      nom: '',
      description: '',
      ordre: 1,
      sprintId: null,
      cours: [],
      quizzes: [],
      qaQuestions: [],
      progressions: []
    };

    this.loading = false;
  }

  private loadModule(id: string): void {
    this.loading = true;
    const sub = this.moduleService.getModuleById(id).subscribe({
      next: (module) => {
        this.module = { ...module };
        this.loading = false;
      },
      error: () => {
        this.notificationService.error('Impossible de charger le module');
        this.loading = false;
        this.router.navigate(['/admin/modules']);
      }
    });
    this.subs.add(sub);
  }

  private loadAllSprints(): void {
    const sub = this.sprintService.getAllSprints().subscribe({
      next: (sprints) => {
        this.allSprints = sprints;
        this.filteredSprints = sprints;
      },
      error: () => {
        this.notificationService.warning('Impossible de charger les sprints');
      }
    });
    this.subs.add(sub);
  }

  onTabChange(index: number): void {
    this.activeTab = index;
  }

  onSprintSearch(term: string): void {
    this.searchTermSprint = term;
    if (!term.trim()) {
      this.filteredSprints = this.allSprints;
    } else {
      this.filteredSprints = this.allSprints.filter(s =>
        s.nom.toLowerCase().includes(term.toLowerCase()) ||
        s.description?.toLowerCase().includes(term.toLowerCase())
      );
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0];
    if (file.type !== 'application/json' && !file.name.endsWith('.json')) {
      this.notificationService.error('Veuillez sélectionner un fichier JSON valide.');
      return;
    }

    const reader = new FileReader();
    reader.onload = () => {
      try {
        const jsonData = JSON.parse(reader.result as string);
        this.processBulkCreate(jsonData);
      } catch (e) {
        this.notificationService.error('Erreur de lecture du fichier JSON.');
      }
    };
    reader.readAsText(file);
  }

  private processBulkCreate(data: any[]): void {
    if (!Array.isArray(data)) {
      this.notificationService.error('Le fichier JSON doit contenir un tableau de modules.');
      return;
    }

    const validModules = data.filter(item =>
      item && typeof item === 'object' &&
      typeof item.nom === 'string' &&
      item.nom.trim().length > 0
    );

    if (validModules.length === 0) {
      this.notificationService.error('Aucun module valide trouvé dans le fichier.');
      return;
    }

    this.loading = true;
    let completed = 0;
    const total = validModules.length;

    validModules.forEach(moduleData => {
      const payload: CreateModuleDto = {
        nom: moduleData.nom,
        description: moduleData.description || '',
        ordre: moduleData.ordre || 1,
        sprintId: moduleData.sprintId || null
      };

      const sub = this.moduleService.createModule(payload).subscribe({
        next: () => {
          completed++;
          if (completed === total) {
            this.loading = false;
            this.notificationService.success(`${total} modules créés avec succès.`);
            // Optionnel : rediriger ou rester sur la page
          }
        },
        error: (err) => {
          this.notificationService.warning(`Échec de création pour "${payload.nom}"`);
          completed++;
          if (completed === total) {
            this.loading = false;
          }
        }
      });
      this.subs.add(sub);
    });
  }

  onSubmit(): void {
    if (!this.module.nom || !this.module.ordre || this.module.ordre < 1) {
      this.notificationService.error('Veuillez remplir les champs obligatoires.');
      return;
    }

    this.loading = true;

    const payload: CreateModuleDto | UpdateModuleDto = {
      nom: this.module.nom,
      description: this.module.description || undefined,
      ordre: this.module.ordre,
      sprintId: this.module.sprintId || undefined
    };

    let request$;
    if (this.isEditMode && this.module.id) {
      request$ = this.moduleService.updateModule(this.module.id, payload as UpdateModuleDto);
    } else {
      request$ = this.moduleService.createModule(payload as CreateModuleDto);
    }

    const sub = request$.subscribe({
      next: (savedModule) => {
        this.notificationService.success(
          `Module "${savedModule.nom}" ${this.isEditMode ? 'mis à jour' : 'créé'} avec succès`
        );
        this.router.navigate(['/admin/modules']);
      },
      error: () => {
        this.notificationService.error(
          `Échec ${this.isEditMode ? 'de la mise à jour' : 'de la création'} du module`
        );
        this.loading = false;
      }
    });
    this.subs.add(sub);
  }

  onCancel(): void {
    this.router.navigate(['/admin/modules']);
  }
}