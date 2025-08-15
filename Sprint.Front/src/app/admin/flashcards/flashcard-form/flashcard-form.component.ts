// src/app/admin/flashcards/flashcard-form.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { LoaderComponent } from '../../../shared/components/loader/loader.component';

import { QaService } from '../../../core/services/qa.service';
import { NotificationService } from '../../../core/services/notification.service';
import { CoursService } from '../../../core/services/cours.service';
import { ModuleService } from '../../../core/services/module.service';
import { SprintService } from '../../../core/services/sprint.service';

import { QAQuestionDto, CreateQAQuestionDto, UpdateQAQuestionDto,  } from '../../../core/models/qa-question.model';
import { CoursDto } from '../../../core/models/cours.model';
import { ModuleDto } from '../../../core/models/module.model';
import { SprintDto } from '../../../core/models/sprint.model';
import { NiveauDifficulte, NiveauEnum } from '../../../core/models/enums.models';
import { concatMap, from, reduce } from 'rxjs';

@Component({
  selector: 'app-flashcard-form',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    FormsModule,
    AdminLayoutComponent,
    LoaderComponent
  ],
  templateUrl: './flashcard-form.component.html',
  styleUrls: ['./flashcard-form.component.scss']
})
export class FlashcardFormComponent implements OnInit {
  isEditMode = signal(false);
  loading = signal(false);

  // Données de la flashcard
  flashcard: QAQuestionDto = {
    id: '',
    question: '',
    reponse: '',
    niveau: NiveauEnum.Cours,
    niveauId: '',
    niveauDifficulte: NiveauDifficulte.Moyen,
    tags: [],
    dateCreation: new Date(),
    consultations: []
  };

  // Listes dynamiques
  sprints: SprintDto[] = [];
  modules: ModuleDto[] = [];
  cours: CoursDto[] = [];

  // Valeurs temporaires pour le formulaire
  selectedSprintId = signal<string>('');
  selectedModuleId = signal<string>('');

  // Gestion des tags
  tagInput = ''; // ✅ Déjà corrigé, mais confirmé

  // Énumérations
  NiveauEnum = NiveauEnum;
  NiveauDifficulte = NiveauDifficulte;

  // Gestion du fichier JSON
  jsonFile: File | null = null;
  isBulkMode = false; // Mode import groupé

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private qaService: QaService,
    private coursService: CoursService,
    private moduleService: ModuleService,
    private sprintService: SprintService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.isEditMode.set(!!id);

    this.loadDependencies();

    if (this.isEditMode() && id) {
      this.loadFlashcard(id);
    }
  }

  loadDependencies(): void {
    this.sprintService.getAllSprints().subscribe({
      next: (data) => {
        this.sprints = data;
      },
      error: () => this.notificationService.warning('Impossible de charger les sprints')
    });
  }

  loadFlashcard(id: string): void {
    this.loading.set(true);
    this.qaService.getQAQuestionById(id).subscribe({
      next: (data) => {
        this.flashcard = { ...data };

        // Restauration de la hiérarchie
        if (this.flashcard.niveau === NiveauEnum.Sprint) {
          this.selectedSprintId.set(this.flashcard.niveauId);
        } else if (this.flashcard.niveau === NiveauEnum.Module) {
          this.moduleService.getModuleById(this.flashcard.niveauId).subscribe({
            next: (module) => {
              this.selectedSprintId.set(module.sprintId!);
              this.selectedModuleId.set(this.flashcard.niveauId);
              this.loadModules(module.sprintId!);
            }
          });
        } else if (this.flashcard.niveau === NiveauEnum.Cours) {
          this.coursService.getCoursById(this.flashcard.niveauId).subscribe({
            next: (cours) => {
              this.moduleService.getModuleById(cours.moduleId!).subscribe({
                next: (module) => {
                  this.selectedSprintId.set(module.sprintId!);
                  this.selectedModuleId.set(cours.moduleId!);
                  this.loadModules(module.sprintId!);
                  this.loadCours(cours.moduleId!);
                }
              });
            }
          });
        }

        this.loading.set(false);
      },
      error: () => {
        this.notificationService.error('Impossible de charger la flashcard');
        this.loading.set(false);
        this.router.navigate(['/admin/flashcards']);
      }
    });
  }

  // Charger les modules d'un sprint
  loadModules(sprintId: string): void {
    this.selectedSprintId.set(sprintId);
    this.moduleService.getModulesBySprintId(sprintId).subscribe({
      next: (modules) => {
        this.modules = modules;
        this.selectedModuleId.set('');
        this.cours = [];
      },
      error: () => this.notificationService.warning('Impossible de charger les modules')
    });
  }

  // Charger les cours d'un module
  loadCours(moduleId: string): void {
    this.selectedModuleId.set(moduleId);
    this.coursService.getCoursByModuleId(moduleId).subscribe({
      next: (cours) => {
        this.cours = cours;
      },
      error: () => this.notificationService.warning('Impossible de charger les cours')
    });
  }

  // Changement de niveau
  onNiveauChange(): void {
    this.flashcard.niveauId = '';
    this.selectedSprintId.set('');
    this.selectedModuleId.set('');
    this.modules = [];
    this.cours = [];
  }

  // Mise à jour du niveauId en fonction du niveau et de la sélection
  updateNiveauId(): void {
    if (this.flashcard.niveau === NiveauEnum.Sprint && this.selectedSprintId()) {
      this.flashcard.niveauId = this.selectedSprintId();
    } else if (this.flashcard.niveau === NiveauEnum.Module && this.selectedModuleId()) {
      this.flashcard.niveauId = this.selectedModuleId();
    } else if (this.flashcard.niveau === NiveauEnum.Cours && this.selectedModuleId() && this.cours.length > 0) {
      // Le `niveauId` est déjà géré par le select cours (voir HTML)
    }
  }

  // Gestion des tags
  addTag(): void {
    const trimmed = this.tagInput.trim();
    if (trimmed && !this.flashcard.tags?.includes(trimmed)) {
      if (!this.flashcard.tags) this.flashcard.tags = [];
      this.flashcard.tags.push(trimmed);
      this.tagInput = '';
    }
  }

  removeTag(tag: string): void {
    this.flashcard.tags = this.flashcard.tags?.filter(t => t !== tag) || [];
  }

  // Gestion du fichier JSON
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.jsonFile = input.files[0];
    }
  }

  importFromJson(): void {
  if (!this.jsonFile) return;

  const reader = new FileReader();
  reader.onload = (e) => {
    try {
      const json = JSON.parse(e.target?.result as string);

      // Validation du format
      if (!Array.isArray(json) || !json.every(q => q.question && q.reponse)) {
        this.notificationService.error('Format JSON invalide');
        return;
      }

      // Création des payloads
      const payloads: CreateQAQuestionDto[] = json.map(item => ({
        question: item.question,
        reponse: item.reponse,
        niveau: this.flashcard.niveau,
        niveauId: this.flashcard.niveauId,
        niveauDifficulte: item.niveauDifficulte || NiveauDifficulte.Moyen,
        tags: item.tags || []
      }));

      // Conversion en Observable (exécution séquentielle)
      from(payloads).pipe(
        concatMap(payload => this.qaService.createQAQuestion(payload)),
        reduce((acc, current) => [...acc, current], [] as QAQuestionDto[])
      ).subscribe({
        next: (created) => {
          this.notificationService.success(`${created.length} flashcards créées`);
          this.router.navigate(['/admin/flashcards']);
        },
        error: (err) => {
          this.notificationService.error('Erreur lors de la création groupée');
        }
      });

    } catch (err) {
      this.notificationService.error('Erreur de lecture du fichier JSON');
    }
  };
  reader.readAsText(this.jsonFile);
}

  onSubmit(): void {
    if (this.isBulkMode && this.jsonFile) {
      this.importFromJson();
      return;
    }

    if (!this.flashcard.question || !this.flashcard.reponse) {
      this.notificationService.error('Veuillez remplir la question et la réponse.');
      return;
    }

    // Mise à jour du niveauId selon le niveau
    this.updateNiveauId();

    if (!this.flashcard.niveauId) {
      this.notificationService.error('Veuillez sélectionner un élément selon le niveau.');
      return;
    }

    this.loading.set(true);

    const payload: CreateQAQuestionDto = {
      question: this.flashcard.question,
      reponse: this.flashcard.reponse,
      niveau: this.flashcard.niveau,
      niveauId: this.flashcard.niveauId,
      niveauDifficulte: Number(this.flashcard.niveauDifficulte),
      tags: this.flashcard.tags?.length ? this.flashcard.tags : undefined
    };

    let request$;
    if (this.isEditMode() && this.flashcard.id) {
      request$ = this.qaService.updateQAQuestion(this.flashcard.id, payload);
    } else {
      request$ = this.qaService.createQAQuestion(payload);
    }

    request$.subscribe({
      next: (saved) => {
        this.notificationService.success(
          `Flashcard "${saved.question.substring(0, 50)}..." ${this.isEditMode() ? 'mise à jour' : 'créée'}`
        );
        this.router.navigate(['/admin/flashcards']);
      },
      error: (err) => {
        this.notificationService.error(`Échec ${this.isEditMode() ? 'de la mise à jour' : 'de la création'}`);
        this.loading.set(false);
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/flashcards']);
  }

  toggleBulkMode(): void {
    this.isBulkMode = !this.isBulkMode;
    if (!this.isBulkMode) {
      this.jsonFile = null;
    }
  }
}
 