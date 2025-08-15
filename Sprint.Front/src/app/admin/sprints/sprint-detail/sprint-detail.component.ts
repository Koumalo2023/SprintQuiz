// src/app/admin/sprints/sprint-detail.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { BreadcrumbComponent } from '../../../shared/components/breadcrumb/breadcrumb.component';
import { TabsComponent } from '../../../shared/components/tabs/tabs.component';
import { StatsCardComponent } from '../../../shared/components/stats-card/stats-card.component';
import { ProgressRingComponent } from '../../../shared/components/progress-ring/progress-ring.component';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { CardComponent } from '../../../shared/components/card/card.component';

import { SprintService } from '../../../core/services/sprint.service';
import { SprintDto } from '../../../core/models/sprint.model';
import { AuthService } from '../../../core/services/auth.service';
import { ProgressionService } from '../../../core/services/progression.service';
import { NiveauEnum } from '../../../core/models/enums.models';
import { CoursService } from '../../../core/services/cours.service';
import { QuizService } from '../../../core/services/quiz.service';
import { QaService } from '../../../core/services/qa.service';
import { CommonModule } from '@angular/common';

interface BreadcrumbItem {
  label: string;
  route?: string;
}

@Component({
  selector: 'app-sprint-detail',
  standalone: true,
  imports: [CommonModule,
    AdminLayoutComponent,
    BreadcrumbComponent,
    TabsComponent,
    StatsCardComponent,
    ProgressRingComponent,
    SearchBarComponent,
    CardComponent,
    RouterLink,
  ],
  templateUrl: './sprint-detail.component.html',
  styleUrl: './sprint-detail.component.scss'
})
export class SprintDetailComponent implements OnInit {
  sprint = signal<SprintDto | null | undefined>(null);
  breadcrumbItems = signal<BreadcrumbItem[]>([]);
  activeTab = 0;
  searchTerm = signal('');
  sprintProgress = signal<number>(0);
 

  constructor(
    private route: ActivatedRoute,
    private sprintService: SprintService,
    private coursService: CoursService,
    private quizService: QuizService,
    private qaService: QaService,
    private progressionService: ProgressionService
  ) {
   
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadSprint(id);
      this.loadQuizzesAndQARelatedToSprint(id);
    }
  }

 loadSprint(id: string): void {
  this.sprintService.getSprintWithModules(id).subscribe({
    next: (data) => {
      this.sprint.set(data);

      // Charger les cours pour chaque module
      const modules = this.sprint()?.modules;
      if (modules) {
        modules.forEach(module => {
          this.coursService.getCoursByModuleId(module.id).subscribe({
            next: (cours) => {
              module.cours = cours;
            },
            error: (err) => {
              console.error('Erreur lors du chargement des cours', err);
            }
          });
        });
      }

      // Récupère la progression de l'utilisateur
      this.progressionService.getProgress(NiveauEnum.Sprint, id, data.nom).subscribe({
        next: (progress) => {
          this.sprintProgress.set(progress);
        }
      });
    },
    error: (err) => {
      console.error('Erreur lors du chargement du sprint', err);
    }
  });
}

  loadQuizzesAndQARelatedToSprint(sprintId: string): void {
  this.quizService.getQuizzesByNiveau(NiveauEnum.Sprint, sprintId).subscribe({
    next: (quizzes) => {
      this.sprint.update(s => {
        if (!s) return s;
        return { ...s, quizzes };
      });
    },
    error: (err) => {
      console.error('Erreur lors du chargement des quiz', err);
    }
  });

  this.qaService.getQAQuestionsByNiveau(NiveauEnum.Sprint, sprintId).subscribe({
    next: (qaQuestions) => {
      this.sprint.update(s => {
        if (!s) return s;
        return { ...s, qaQuestions };
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
      { label: 'Sprints', route: '/admin/sprints' },
      { label: title }
    ]);
  }

  onTabChange(index: number): void {
    this.activeTab = index;
  }

  onSearch(term: string): void {
    this.searchTerm.set(term.toLowerCase());
  }

  get filteredModules() {
    const term = this.searchTerm();
    return this.sprint()?.modules?.filter(mod =>
      mod.nom.toLowerCase().includes(term) ||
      mod.description?.toLowerCase().includes(term) ||
      mod.cours?.some(c => c.titre.toLowerCase().includes(term))
    ) || [];
  }

  get filteredQuizzes() {
    const term = this.searchTerm();
    return (this.sprint()?.quizzes || []).filter(q =>
      q.titre.toLowerCase().includes(term)
    );
  }

  get filteredFlashcards() {
    const term = this.searchTerm();
    return (this.sprint()?.qaQuestions || []).filter(f =>
      f.question.toLowerCase().includes(term)
    );
  }
}