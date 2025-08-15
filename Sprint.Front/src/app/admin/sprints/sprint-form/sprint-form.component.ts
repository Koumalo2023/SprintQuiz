import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TabsComponent } from '../../../shared/components/tabs/tabs.component';
import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { FormsModule } from '@angular/forms';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { CreateSprintDto, SprintDto, UpdateSprintDto } from '../../../core/models/sprint.model';
import { ModuleDto } from '../../../core/models/module.model';
import { Subscription } from 'rxjs';
import { SprintService } from '../../../core/services/sprint.service';
import { ModuleService } from '../../../core/services/module.service';
import { NotificationService } from '../../../core/services/notification.service';
import { LoaderComponent } from '../../../shared/components/loader/loader.component';
import { OrderService } from '../../../core/services/order.services';



@Component({
  selector: 'app-sprint-form',
  standalone: true,
  imports: [CommonModule, AdminLayoutComponent, FormsModule, SearchBarComponent, TabsComponent, LoaderComponent],
  templateUrl: './sprint-form.component.html',
  styleUrl: './sprint-form.component.scss'
})
export class SprintFormComponent implements OnInit, OnDestroy {
  isEditMode = false;
  loading = false;

  // Données du sprint
  sprint: SprintDto = {
    id: '',
    nom: '',
    description: '',
    ordre: 1,
    modules: [],
    cours: [],
    quizzes: [],
    qaQuestions:[],
    progressions:[]
  };

  // Gestion des modules (uniquement en édition)
  allModules: ModuleDto[] = [];
  filteredModules: ModuleDto[] = [];
  selectedModuleIds: string[] = [];

  // Onglets
  tabs = [
    { label: 'Informations', disabled: false },
    { label: 'Modules', disabled: false },
    { label: 'Contenus', disabled: false }
  ];
  activeTab = 0;

  private subs = new Subscription();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private sprintService: SprintService,
    private moduleService: ModuleService,
    private orderService: OrderService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!id;

    if (this.isEditMode && id) {
      this.loadSprint(id);
    } else {
      this.initEmptySprint();
    }

    // Charger les modules uniquement en mode édition
    if (this.isEditMode) {
      this.loadAllModules();
    }
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

private initEmptySprint(): void {
  this.loading = true;

  // ✅ Objet conforme à OrderRequest
  this.orderService.getNextOrder({
    entityType: 'sprint'
  }).subscribe({
    next: (nextOrder) => {
      this.sprint = {
        id: '',
        nom: '',
        description: '',
        ordre: nextOrder,
        modules: [],
        cours: [],
        quizzes: [],
        qaQuestions: [],
        progressions: []
      };
      this.loading = false;
    },
    error: () => {
      this.sprint = {
        id: '',
        nom: '',
        description: '',
        ordre: 1,
        modules: [],
        cours: [],
        quizzes: [],
        qaQuestions: [],
        progressions: []
      };
      this.loading = false;
    }
  });
}


 private loadSprint(id: string): void {
  this.loading = true;
  const sub = this.sprintService.getSprintById(id).subscribe({
    next: (sprint: SprintDto) => { // Typage explicite ici aussi (optionnel mais recommandé)
      this.sprint = { ...sprint };
      this.selectedModuleIds = sprint.modules?.map((m: ModuleDto) => m.id) || [];
      this.loading = false;
    },
    error: (err) => {
      this.notificationService.error('Impossible de charger le sprint');
      this.loading = false;
      this.router.navigate(['/admin/sprints']);
    }
  });
  this.subs.add(sub);
}
  private loadAllModules(): void {
    const sub = this.moduleService.getAllModules().subscribe({
      next: (modules) => {
        this.allModules = modules;
        this.filteredModules = modules;
      },
      error: () => {
        this.notificationService.warning('Impossible de charger les modules');
      }
    });
    this.subs.add(sub);
  }

  onTabChange(index: number): void {
    this.activeTab = index;
  }

  onModuleSearch(term: string): void {
    if (!term.trim()) {
      this.filteredModules = this.allModules;
    } else {
      this.filteredModules = this.allModules.filter(m =>
        m.nom.toLowerCase().includes(term.toLowerCase()) ||
        m.description?.toLowerCase().includes(term.toLowerCase())
      );
    }
  }

  isModuleSelected(moduleId: string): boolean {
    return this.selectedModuleIds.includes(moduleId);
  }

  toggleModule(moduleId: string): void {
    const index = this.selectedModuleIds.indexOf(moduleId);
    if (index === -1) {
      this.selectedModuleIds.push(moduleId);
    } else {
      this.selectedModuleIds.splice(index, 1);
    }
  }
  

  onSubmit(): void {
    if (!this.sprint.nom || !this.sprint.ordre || this.sprint.ordre < 1) {
      this.notificationService.error('Veuillez remplir tous les champs obligatoires.');
      return;
    }

    this.loading = true;

    const payload: CreateSprintDto | UpdateSprintDto = {
      nom: this.sprint.nom,
      description: this.sprint.description || undefined,
      ordre: this.sprint.ordre
    };

    let request$;
    if (this.isEditMode && this.sprint.id) {
      request$ = this.sprintService.updateSprint(this.sprint.id, payload as UpdateSprintDto);
    } else {
      request$ = this.sprintService.createSprint(payload as CreateSprintDto);
    }

    const sub = request$.subscribe({
      next: (savedSprint) => {
        this.notificationService.success(
          `Sprint "${savedSprint.nom}" ${this.isEditMode ? 'mis à jour' : 'créé'} avec succès`
        );
        this.router.navigate(['/admin/sprints']);
      },
      error: (err) => {
        this.notificationService.error(
          `Échec ${this.isEditMode ? 'de la mise à jour' : 'de la création'} du sprint`
        );
        this.loading = false;
      }
    });

    this.subs.add(sub);
  }

  onCancel(): void {
    this.router.navigate(['/admin/sprints']);
  }
}