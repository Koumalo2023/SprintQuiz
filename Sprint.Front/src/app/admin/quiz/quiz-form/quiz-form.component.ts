// src/app/admin/quiz/quiz-form.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

// Layouts & Shared
import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { LoaderComponent } from '../../../shared/components/loader/loader.component';

// Services
import { QuizService } from '../../../core/services/quiz.service';
import { CoursService } from '../../../core/services/cours.service';
import { ModuleService } from '../../../core/services/module.service';
import { SprintService } from '../../../core/services/sprint.service';
import { NotificationService } from '../../../core/services/notification.service';

// Models
import {
  QuizDto,
  CreateQuizDto,
  CreateQCMQuestionDto,
  QCMOptionDto,
  UpdateQuizDto,
  CreateQCMOptionDto,
} from '../../../core/models/quiz.model';

// RxJS
import { from, concatMap, reduce, of } from 'rxjs';
import { NiveauDifficulte, NiveauEnum } from '../../../core/models/enums.models';
import { SprintDto } from '../../../core/models/sprint.model';
import { ModuleDto } from '../../../core/models/module.model';
import { CoursDto } from '../../../core/models/cours.model';
import { EnumService } from '../../../core/services/enum.service';
import { QuizQuestionFormComponent } from '../../../shared/components/quiz-question-form/quiz-question-form.component';




@Component({
  selector: 'app-quiz-form',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AdminLayoutComponent,
    LoaderComponent,
    QuizQuestionFormComponent,
  ],
  templateUrl: './quiz-form.component.html',
  styleUrls: ['./quiz-form.component.scss'],
})
export class QuizFormComponent implements OnInit {
  isEditMode = signal(false);
  loading = signal(false);

  // Données du quiz
  quiz = signal<QuizDto>({
    id: '',
    titre: '',
    description: '',
    niveau: NiveauEnum.Cours,
    niveauId: undefined,
    dateCreation: new Date(),
    version:[],
    questions: [],
    tentativesQuiz: [],
  });

  // Liste des questions
  questions = signal<CreateQCMQuestionDto[]>([]);

  // Listes dynamiques
  sprints = signal<SprintDto[]>([]);
  modules = signal<ModuleDto[]>([]);
  cours = signal<CoursDto[]>([]);

  // Sélections
  selectedSprintId = signal<string>('');
  selectedModuleId = signal<string>('');

  // Gestion JSON
  jsonFile: File | null = null;
  isBulkMode = false;

  // Énumérations
  NiveauEnum = NiveauEnum;
  NiveauDifficulte = NiveauDifficulte;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private quizService: QuizService,
    private coursService: CoursService,
    private moduleService: ModuleService,
    private sprintService: SprintService,
    private notificationService: NotificationService,
    public enumService: EnumService
  ) { }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.isEditMode.set(!!id);
    this.loadDependencies();
    if (this.isEditMode() && id) {
      this.loadQuiz(id);
    }
  }

  loadDependencies(): void {
    this.sprintService.getAllSprints().subscribe({
      next: (data) => this.sprints.set(data),
      error: () => this.notificationService.warning('Impossible de charger les sprints'),
    });
  }

loadQuiz(id: string): void {
  this.loading.set(true);
  this.quizService.getQuizWithQuestions(id).subscribe({
    next: (data) => {
      this.quiz.set(data);

      // Peupler les selects Sprint/Module/Cours
      if (data.niveau === NiveauEnum.Sprint) {
        this.selectedSprintId.set(data.niveauId!);
        this.loadModules(data.niveauId!);
      } else if (data.niveau === NiveauEnum.Module) {
        this.moduleService.getModuleById(data.niveauId!).subscribe({
          next: (module) => {
            this.selectedSprintId.set(module.sprintId!);
            this.selectedModuleId.set(data.niveauId!);
            this.loadModules(module.sprintId!);
          },
        });
      } else if (data.niveau === NiveauEnum.Cours) {
        this.coursService.getCoursById(data.niveauId!).subscribe({
          next: (cours) => {
            this.moduleService.getModuleById(cours.moduleId!).subscribe({
              next: (module) => {
                this.selectedSprintId.set(module.sprintId!);
                this.selectedModuleId.set(cours.moduleId!);
                this.loadModules(module.sprintId!);
                this.loadCours(cours.moduleId!);
              },
            });
          },
        });
      }

      // ✅ Charger les questions directement depuis le quiz
      if (data.questions && data.questions.length > 0) {
        const createQuestions = data.questions.map(q => ({
          intitule: q.intitule,
          niveauDifficulte: q.niveauDifficulte,
          explication: q.explication || '',
          options: q.options && q.options.length > 0
            ? q.options.map(o => ({ texte: o.texte, estCorrecte: o.estCorrecte }))
            : [
                { texte: '', estCorrecte: false },
                { texte: '', estCorrecte: false }
              ]
        })) as CreateQCMQuestionDto[];

        this.questions.set(createQuestions);
      }
    },
    error: () => {
      this.notificationService.error('Impossible de charger le quiz');
      this.router.navigate(['/admin/quiz']);
    },
  }).add(() => this.loading.set(false));
}


  loadModules(sprintId: string): void {
    this.selectedSprintId.set(sprintId);
    this.moduleService.getModulesBySprintId(sprintId).subscribe({
      next: (modules) => {
        this.modules.set(modules);
        this.selectedModuleId.set('');
        this.cours.set([]);
      },
      error: () => this.notificationService.warning('Impossible de charger les modules'),
    });
  }

  loadCours(moduleId: string): void {
    this.selectedModuleId.set(moduleId);
    this.coursService.getCoursByModuleId(moduleId).subscribe({
      next: (cours) => this.cours.set(cours),
      error: () => this.notificationService.warning('Impossible de charger les cours'),
    });
  }

  onNiveauChange(): void {
    this.quiz.update(q => ({ ...q, niveauId: undefined }));
    this.selectedSprintId.set('');
    this.selectedModuleId.set('');
    this.modules.set([]);
    this.cours.set([]);
  }

  updateNiveauId(): void {
    const q = this.quiz();
    if (q.niveau === NiveauEnum.Sprint && this.selectedSprintId()) {
      this.quiz.update(quiz => ({ ...quiz, niveauId: this.selectedSprintId() }));
    } else if (q.niveau === NiveauEnum.Module && this.selectedModuleId()) {
      this.quiz.update(quiz => ({ ...quiz, niveauId: this.selectedModuleId() }));
    } else if (q.niveau === NiveauEnum.Cours && this.selectedModuleId() && this.cours().length > 0) {
      // Déjà géré par le select dans le HTML
    }
  }

  addQuestion(): void {
    const newQuestion: CreateQCMQuestionDto = {
      intitule: '',
      niveauDifficulte: NiveauDifficulte.Moyen,
      explication: '',
      options: [],
    };
    this.questions.update(qs => [...qs, newQuestion]);
  }

  removeQuestion(index: number): void {
    this.questions.update(qs => qs.filter((_, i) => i !== index));
  }

  addOption(questionIndex: number): void {
    const newOption: CreateQCMOptionDto = {
      texte: '',
      estCorrecte: false
    };

    this.questions.update(qs => {
      const newQuestions = [...qs];
      if (!newQuestions[questionIndex].options) {
        newQuestions[questionIndex].options = [];
      }
      newQuestions[questionIndex].options.push(newOption);
      return newQuestions;
    });
  }

  removeOption(questionIndex: number, optionIndex: number): void {
    this.questions.update(qs => {
      const newQuestions = [...qs];
      newQuestions[questionIndex].options = newQuestions[questionIndex].options.filter((_, i) => i !== optionIndex);
      return newQuestions;
    });
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

        // 🔹 Valider que le formulaire a un niveauId valide
        this.updateNiveauId(); // Met à jour quiz().niveauId depuis le formulaire
        const finalNiveauId = this.quiz().niveauId;

        if (!finalNiveauId) {
          this.notificationService.error('Veuillez sélectionner un élément dans le formulaire (Sprint/Module/Cours)');
          return;
        }

        const finalNiveau = this.quiz().niveau;

        // 🔹 Créer les payloads en ajoutant le niveau depuis le formulaire
        const payloads: CreateQuizDto[] = json.map(item => ({
          titre: item.titre,
          description: item.description || '',
          version: item.version,
          niveau: finalNiveau,
          niveauId: finalNiveauId,
          questions: (item.questions || []).map((q: any) => ({
            intitule: q.intitule,
            niveauDifficulte: q.niveauDifficulte || NiveauDifficulte.Moyen,
            explication: q.explication || '',
            options: (q.options || []).map((opt: any) => ({
              texte: opt.texte,
              estCorrecte: opt.estCorrecte
            }))
          }))
        }));

        // 🔹 Validation rapide
        const validPayloads = payloads.filter(p => {
          if (!p.titre) {
            this.notificationService.warning('Quiz sans titre ignoré');
            return false;
          }
          if (p.questions.length === 0) {
            this.notificationService.warning(`Quiz "${p.titre}" sans questions ignoré`);
            return false;
          }
          return true;
        });

        if (validPayloads.length === 0) {
          this.notificationService.error('Aucun quiz valide à importer');
          return;
        }

        // 🔹 Envoi
        from(validPayloads).pipe(
          concatMap(payload => this.quizService.createQuiz(payload)),
          reduce((acc, quiz) => [...acc, quiz], [] as QuizDto[])
        ).subscribe({
          next: (created) => {
            this.notificationService.success(`${created.length} quiz(s) importés`);
            this.clearFile();
          },
          error: (err) => {
            console.error('Erreur d’import', err);
            this.notificationService.error('Échec de l’import (vérifiez le format)');
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

    if (!this.quiz().titre) {
      this.notificationService.error('Veuillez remplir le titre du quiz.');
      return;
    }

    if (this.questions().length === 0) {
      this.notificationService.warning('Un quiz doit contenir au moins une question.');
      return;
    }

    this.updateNiveauId(); // 🔹 Même logique : le formulaire fournit le niveauId

    if (!this.quiz().niveauId) {
      this.notificationService.error('Veuillez sélectionner un élément selon le niveau.');
      return;
    }

    const payload: CreateQuizDto = {
      titre: this.quiz().titre,
      description: this.quiz().description,
      version: this.quiz().version,
      niveau: this.quiz().niveau,
      niveauId: this.quiz().niveauId,
      questions: this.questions().map(q => ({
        intitule: q.intitule,
        niveauDifficulte: q.niveauDifficulte,
        explication: q.explication || '',
        options: q.options.map(opt => ({
          texte: opt.texte,
          estCorrecte: opt.estCorrecte
        }))
      }))
    };

    let request$ = this.isEditMode() && this.quiz().id
      ? this.quizService.updateQuiz(this.quiz().id, payload as UpdateQuizDto)
      : this.quizService.createQuiz(payload);

    request$.subscribe({
      next: (savedQuiz) => {
        this.notificationService.success(`Quiz "${savedQuiz.titre}" ${this.isEditMode() ? 'mis à jour' : 'créé'}`);
        this.router.navigate(['/admin/quiz']);
      },
      error: (err) => {
        this.notificationService.error(`Échec ${this.isEditMode() ? 'de la mise à jour' : 'de la création'}`);
        console.error('Erreur lors de la création du quiz', err);
        this.loading.set(false);
      }
    });
  }
  // Remet à zéro le fichier sélectionné
  clearFile(): void {
    this.jsonFile = null;

    // Optionnel : remettre le champ fichier à zéro dans l'interface
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  onCancel(): void {
    this.router.navigate(['/admin/quiz']);
  }

  toggleBulkMode(): void {
    this.isBulkMode = !this.isBulkMode;
    if (!this.isBulkMode) {
      this.jsonFile = null;
    }
  }
}