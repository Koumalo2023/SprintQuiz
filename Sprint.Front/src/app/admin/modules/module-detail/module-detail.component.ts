// src/app/admin/modules/module-detail.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule, NgIf } from '@angular/common';
import { RouterLink } from '@angular/router';

// Layouts & Shared Components
import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { BreadcrumbComponent, BreadcrumbItem } from '../../../shared/components/breadcrumb/breadcrumb.component';
import { TabsComponent } from '../../../shared/components/tabs/tabs.component';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { StatsCardComponent } from '../../../shared/components/stats-card/stats-card.component';
import { ProgressRingComponent } from '../../../shared/components/progress-ring/progress-ring.component';

// Services
import { ModuleService } from '../../../core/services/module.service';
import { CoursService } from '../../../core/services/cours.service';
import { QuizService } from '../../../core/services/quiz.service';
import { QaService } from '../../../core/services/qa.service';
import { ProgressionService } from '../../../core/services/progression.service';
import { ModuleDto } from '../../../core/models/module.model';
import { NiveauEnum } from '../../../core/models/enums.models';

// Models

@Component({
  selector: 'app-module-detail',
  standalone: true,
  imports: [
    CommonModule,
    AdminLayoutComponent,
    BreadcrumbComponent,
    TabsComponent,
    SearchBarComponent,
    CardComponent,
    StatsCardComponent,
    ProgressRingComponent,
    RouterLink,
  ],
  templateUrl: './module-detail.component.html',
  styleUrls: ['./module-detail.component.scss']
})
export class ModuleDetailComponent implements OnInit {
  module = signal<ModuleDto | null | undefined>(null);
  breadcrumbItems = signal<BreadcrumbItem[]>([]);
  activeTab = 0;
  searchTerm = signal('');
  moduleProgress = signal<number>(0);

  constructor(
    private route: ActivatedRoute,
    private moduleService: ModuleService,
    private coursService: CoursService,
    private quizService: QuizService,
    private qaService: QaService,
    private progressionService: ProgressionService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadModule(id);
      this.loadQuizzesAndQARelatedToModule(id);
    }
  }

  loadModule(id: string): void {
    this.moduleService.getModuleWithCours(id).subscribe({
      next: (data) => {
        this.module.set(data);

        // Récupère la progression de l'utilisateur
        this.progressionService.getProgress(NiveauEnum.Module, id, data.nom).subscribe({
          next: (progress) => {
            this.moduleProgress.set(progress);
          }
        });

        // Mise à jour du fil d'Ariane
        this.updateBreadcrumb(data.nom);
      },
      error: (err) => {
        console.error('Erreur lors du chargement du module', err);
      }
    });
  }

  loadQuizzesAndQARelatedToModule(moduleId: string): void {
    this.quizService.getQuizzesByNiveau(NiveauEnum.Module, moduleId).subscribe({
      next: (quizzes) => {
        this.module.update(m => {
          if (!m) return m;
          return { ...m, quizzes };
        });
      },
      error: (err) => {
        console.error('Erreur lors du chargement des quiz', err);
      }
    });

    this.qaService.getQAQuestionsByNiveau(NiveauEnum.Module, moduleId).subscribe({
      next: (qaQuestions) => {
        this.module.update(m => {
          if (!m) return m;
          return { ...m, qaQuestions };
        });
      },
      error: (err) => {
        console.error('Erreur lors du chargement des questions-réponses', err);
      }
    });
  }

  updateBreadcrumb(title: string): void {
    this.breadcrumbItems.set([
      { label: 'Accueil', route: '/admin' },
      { label: 'Modules', route: '/admin/modules' },
      { label: title }
    ]);
  }

  onTabChange(index: number): void {
    this.activeTab = index;
  }

  onSearch(term: string): void {
    this.searchTerm.set(term.toLowerCase());
  }

  get filteredCours() {
    const term = this.searchTerm();
    return this.module()?.cours?.filter(cours =>
      cours.titre.toLowerCase().includes(term) ||
      cours.description?.toLowerCase().includes(term)
    ) || [];
  }

  get filteredQuizzes() {
    const term = this.searchTerm();
    return (this.module()?.quizzes || []).filter(q =>
      q.titre.toLowerCase().includes(term)
    );
  }

  get filteredFlashcards() {
    const term = this.searchTerm();
    return (this.module()?.qaQuestions || []).filter(f =>
      f.question.toLowerCase().includes(term)
    );
  }
}