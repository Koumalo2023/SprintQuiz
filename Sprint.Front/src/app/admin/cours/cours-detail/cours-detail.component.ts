// src/app/admin/cours/cours-detail.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
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
import { CoursService } from '../../../core/services/cours.service';
import { QuizService } from '../../../core/services/quiz.service';
import { QaService } from '../../../core/services/qa.service';
import { ProgressionService } from '../../../core/services/progression.service';
import { CoursDto } from '../../../core/models/cours.model';
import { NiveauEnum } from '../../../core/models/enums.models';



@Component({
  selector: 'app-cours-detail',
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
  templateUrl: './cours-detail.component.html',
  styleUrls: ['./cours-detail.component.scss']
})
export class CoursDetailComponent implements OnInit {
  cours = signal<CoursDto | null | undefined>(null);
  breadcrumbItems = signal<BreadcrumbItem[]>([]);
  activeTab = 0;
  searchTerm = signal('');
  coursProgress = signal<number>(0);

  constructor(
    private route: ActivatedRoute,
    private coursService: CoursService,
    private quizService: QuizService,
    private qaService: QaService,
    private progressionService: ProgressionService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadCours(id);
      this.loadQuizzesAndQARelatedToCours(id);
    }
  }

  loadCours(id: string): void {
    this.coursService.getCoursById(id).subscribe({
      next: (data) => {
        this.cours.set(data);

        // Récupère la progression de l'utilisateur
        this.progressionService.getProgress(NiveauEnum.Cours, id, data.titre).subscribe({
          next: (progress) => {
            this.coursProgress.set(progress);
          }
        });

        // Mise à jour du fil d’Ariane
        this.updateBreadcrumb(data.titre);
      },
      error: (err) => {
        console.error('Erreur lors du chargement du cours', err);
      }
    });
  }

  loadQuizzesAndQARelatedToCours(coursId: string): void {
    this.quizService.getQuizzesByNiveau(NiveauEnum.Cours, coursId).subscribe({
      next: (quizzes) => {
        this.cours.update(c => {
          if (!c) return c;
          return { ...c, quizzes };
        });
      },
      error: (err) => {
        console.error('Erreur lors du chargement des quiz', err);
      }
    });

    this.qaService.getQAQuestionsByNiveau(NiveauEnum.Cours, coursId).subscribe({
      next: (qaQuestions) => {
        this.cours.update(c => {
          if (!c) return c;
          return { ...c, qaQuestions };
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
      { label: 'Cours', route: '/admin/cours' },
      { label: title }
    ]);
  }

  onTabChange(index: number): void {
    this.activeTab = index;
  }

  onSearch(term: string): void {
    this.searchTerm.set(term.toLowerCase());
  }

  get filteredQuizzes() {
    const term = this.searchTerm();
    return (this.cours()?.quizzes || []).filter(q =>
      q.titre.toLowerCase().includes(term)
    );
  }

  get filteredFlashcards() {
    const term = this.searchTerm();
    return (this.cours()?.qaQuestions || []).filter(f =>
      f.question.toLowerCase().includes(term)
    );
  }
}