// src/app/admin/exercises/exercise-form.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

// Layouts & Shared
import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { LoaderComponent } from '../../../shared/components/loader/loader.component';

// Services
import { ExerciceService } from '../../../core/services/exercice.service';
import { CoursService } from '../../../core/services/cours.service';
import { ModuleService } from '../../../core/services/module.service';
import { SprintService } from '../../../core/services/sprint.service';
import { NotificationService } from '../../../core/services/notification.service';
import { EnumService, NiveauDifficulte, NiveauEnum, TypeExercice } from '../../../core/services/enum.service';



import { SprintDto } from '../../../core/models/sprint.model';
import { ModuleDto } from '../../../core/models/module.model';
import { CoursDto } from '../../../core/models/cours.model';

// RxJS
import { from, concatMap, reduce, of } from 'rxjs';

// Models
import { CreateEtapeResolutionDto, CreateExerciceDto, CreateIndiceDto, ExerciceDto, UpdateExerciceDto } from '../../../core/models/exercice.model';

@Component({
  selector: 'app-exercise-form',
  standalone: true,
  imports: [CommonModule, FormsModule, AdminLayoutComponent, LoaderComponent],
  templateUrl: './exercise-form.component.html',
  styleUrls: ['./exercise-form.component.scss']
})
export class ExerciseFormComponent implements OnInit {
  isEditMode = signal(false);
  loading = signal(false);

  // Données de l'exercice
  exercice = signal<ExerciceDto>({
    id: '',
    enonce: '',
    solution: '',
    solutionResume: '',
    niveau: NiveauEnum.Cours,
    niveauId: '',
    niveauDifficulte: NiveauDifficulte.Moyen,
    type: TypeExercice.Basique,
    tags: [],
    dateCreation: new Date().toISOString(),
    indices: [],
    etapesResolution: []
  });

  // Listes d'indices et étapes
  indices = signal<CreateIndiceDto[]>([]);
  etapes = signal<CreateEtapeResolutionDto[]>([]);

  // Listes dynamiques
  sprints = signal<SprintDto[]>([]);
  modules = signal<ModuleDto[]>([]);
  cours = signal<CoursDto[]>([]);

  // Sélections
  selectedSprintId = signal<string>('');
  selectedModuleId = signal<string>('');

  // Gestion JSON
  jsonFile: File | null = null;
  isBulkMode = false; // Mode import groupé

  // Énumérations
  NiveauEnum = NiveauEnum;
  NiveauDifficulte = NiveauDifficulte;
  TypeExercice = TypeExercice;

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private exerciceService: ExerciceService,
    private coursService: CoursService,
    private moduleService: ModuleService,
    private sprintService: SprintService,
    private notificationService: NotificationService,
    public enumService: EnumService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.isEditMode.set(!!id);
    this.loadDependencies();

    if (this.isEditMode() && id) {
      this.loadExercice(id);
    }
  }

  loadDependencies(): void {
    this.sprintService.getAllSprints().subscribe({
      next: (data) => this.sprints.set(data),
      error: () => this.notificationService.warning('Impossible de charger les sprints')
    });
  }

  loadExercice(id: string): void {
    this.loading.set(true);
    this.exerciceService.getExerciceById(id).subscribe({
      next: (data) => {
        this.exercice.set(data);

        // Charger les dépendances selon le niveau
        if (data.niveau === NiveauEnum.Sprint) {
          this.selectedSprintId.set(data.niveauId);
          this.loadModules(data.niveauId);
        } else if (data.niveau === NiveauEnum.Module) {
          this.moduleService.getModuleById(data.niveauId).subscribe({
            next: (module) => {
              this.selectedSprintId.set(module.sprintId!);
              this.selectedModuleId.set(data.niveauId!);
              this.loadModules(module.sprintId!);
            }
          });
        } else if (data.niveau === NiveauEnum.Cours) {
          this.coursService.getCoursById(data.niveauId).subscribe({
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

        // Charger indices et étapes
        this.loadIndices(id);
        this.loadEtapes(id);
      },
      error: () => {
        this.notificationService.error('Impossible de charger l’exercice');
        this.router.navigate(['/admin/exercises']);
      },
      complete: () => this.loading.set(false)
    });
  }

  loadIndices(exerciceId: string): void {
    this.exerciceService.getIndicesByExerciceId(exerciceId).subscribe({
      next: (indices) => this.indices.set(indices as any),
      error: () => this.notificationService.warning('Impossible de charger les indices')
    });
  }

  loadEtapes(exerciceId: string): void {
    this.exerciceService.getEtapesByExerciceId(exerciceId).subscribe({
      next: (etapes) => this.etapes.set(etapes as any),
      error: () => this.notificationService.warning('Impossible de charger les étapes')
    });
  }

  loadModules(sprintId: string): void {
    this.selectedSprintId.set(sprintId);
    this.moduleService.getModulesBySprintId(sprintId).subscribe({
      next: (modules) => {
        this.modules.set(modules);
        this.selectedModuleId.set('');
        this.cours.set([]);
      },
      error: () => this.notificationService.warning('Impossible de charger les modules')
    });
  }

  loadCours(moduleId: string): void {
    this.selectedModuleId.set(moduleId);
    this.coursService.getCoursByModuleId(moduleId).subscribe({
      next: (cours) => this.cours.set(cours),
      error: () => this.notificationService.warning('Impossible de charger les cours')
    });
  }

  updateTags(event: Event): void {
  const value = (event.target as HTMLInputElement).value;
  const tags = value
    ? value.split(',')
        .map(tag => tag.trim())
        .filter(tag => tag.length > 0) // Supprime les chaînes vides
    : [];
  this.exercice.update(e => ({ ...e, tags }));
}

 onNiveauChange(): void {
  // Réinitialise les sélections
  this.selectedSprintId.set('');
  this.selectedModuleId.set('');
  this.modules.set([]);
  this.cours.set([]);

  // Ne modifie PAS `niveauId` ici → il sera mis à jour dans `updateNiveauId()`
  // On garde le `niveau`, mais on laisse `niveauId` vide jusqu'à la sélection
  this.exercice.update(e => ({ ...e, niveauId: '' })); // ✅ ou même omettre temporairement
}

  


  updateNiveauId(): void {
    const e = this.exercice();
    if (e.niveau === NiveauEnum.Sprint && this.selectedSprintId()) {
      this.exercice.update(ex => ({ ...ex, niveauId: this.selectedSprintId() }));
    } else if (e.niveau === NiveauEnum.Module && this.selectedModuleId()) {
      this.exercice.update(ex => ({ ...ex, niveauId: this.selectedModuleId() }));
    } else if (e.niveau === NiveauEnum.Cours && this.selectedModuleId()) {
      // Déjà géré par le formulaire
    }
  }

  addIndice(): void {
    this.indices.update(list => [
      ...list,
      { ordre: list.length, texte: '' }
    ]);
  }

  removeIndice(index: number): void {
    this.indices.update(list => list.filter((_, i) => i !== index));
  }

  addEtape(): void {
    this.etapes.update(list => [
      ...list,
      { ordre: list.length, description: '' }
    ]);
  }

  removeEtape(index: number): void {
    this.etapes.update(list => list.filter((_, i) => i !== index));
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.jsonFile = input.files[0];
    }
  }

  importFromJson(): void {
    if (!this.jsonFile) return;

    const reader = new FileReader();
    reader.onload = () => {
      try {
        const json = JSON.parse(reader.result as string);
        if (!Array.isArray(json)) {
          throw new Error('Le JSON doit être un tableau');
        }

        this.updateNiveauId();
        const finalNiveauId = this.exercice().niveauId;
        const finalNiveau = this.exercice().niveau;

        if (!finalNiveauId) {
          this.notificationService.error('Veuillez sélectionner un élément dans le formulaire (Sprint/Module/Cours)');
          return;
        }

        const payloads: CreateExerciceDto[] = json.map(item => ({
          enonce: item.enonce,
          solution: item.solution,
          solutionResume: item.solutionResume || '',
          niveau: finalNiveau,
          niveauId: finalNiveauId,
          niveauDifficulte: item.niveauDifficulte || NiveauDifficulte.Moyen,
          type: item.type || TypeExercice.Basique,
          tags: item.tags || [],
          indices: (item.indices || []).map((i: any, idx: number) => ({ ordre: idx, texte: i.texte })),
          etapesResolution: (item.etapes || []).map((e: any, idx: number) => ({ ordre: idx, description: e.description }))
        }));

        const validPayloads = payloads.filter(p => {
          if (!p.enonce) {
            this.notificationService.warning('Exercice sans énoncé ignoré');
            return false;
          }
          return true;
        });

        if (validPayloads.length === 0) {
          this.notificationService.warning('Aucun exercice valide à importer');
          return;
        }

        // Création groupée
        from(validPayloads).pipe(
          concatMap(payload => this.exerciceService.createExercice(payload)),
          reduce((acc, current) => acc + 1, 0)
        ).subscribe({
          next: (count) => {
            this.notificationService.success(`${count} exercice(s) importé(s) avec succès`);
            this.router.navigate(['/admin/exercises']);
          },
          error: (err) => {
            this.notificationService.error('Échec de l’importation des exercices');
            console.error(err);
          }
        });

      } catch (err: any) {
        this.notificationService.error(`Erreur JSON : ${err.message}`);
      }
    };
    reader.readAsText(this.jsonFile);
  }

  onSubmit(): void {
    if (this.isBulkMode && this.jsonFile) {
      this.importFromJson();
      return;
    }

    if (!this.exercice().enonce) {
      this.notificationService.error('L’énoncé est obligatoire');
      return;
    }

    this.updateNiveauId();
    if (!this.exercice().niveauId) {
      this.notificationService.error('Veuillez sélectionner un élément selon le niveau');
      return;
    }

    const payload: CreateExerciceDto = {
      enonce: this.exercice().enonce,
      solution: this.exercice().solution,
      solutionResume: this.exercice().solutionResume || '',
      niveau: this.exercice().niveau,
      niveauId: this.exercice().niveauId,
      niveauDifficulte: this.exercice().niveauDifficulte,
      type: this.exercice().type,
      tags: this.exercice().tags || [],
      indices: this.indices(),
      etapesResolution: this.etapes()
    };

    let request$ = this.isEditMode() && this.exercice().id
      ? this.exerciceService.updateExercice(this.exercice().id, payload as UpdateExerciceDto)
      : this.exerciceService.createExercice(payload);

    this.loading.set(true);
    request$.subscribe({
      next: (saved) => {
        this.notificationService.success(`Exercice "${saved.id}" ${this.isEditMode() ? 'mis à jour' : 'créé'}`);
        this.router.navigate(['/admin/exercises']);
      },
      error: (err) => {
        this.notificationService.error(`Échec ${this.isEditMode() ? 'de la mise à jour' : 'de la création'}`);
        console.error('Erreur création exercice', err);
        this.loading.set(false);
      }
    });
  }

  clearFile(): void {
    this.jsonFile = null;
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    if (fileInput) fileInput.value = '';
  }
}