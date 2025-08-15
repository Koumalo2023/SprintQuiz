import { Component, OnInit, signal } from '@angular/core';
import { SprintService } from '../../../core/services/sprint.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { QuizService } from '../../../core/services/quiz.service';
import { ModuleService } from '../../../core/services/module.service';
import { QuizDto } from '../../../core/models/quiz.model';
import { CoursService } from '../../../core/services/cours.service';
import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { StatsCardComponent } from '../../../shared/components/stats-card/stats-card.component'; 
import { CardComponent } from '../../../shared/components/card/card.component';
import { CommonModule } from '@angular/common';
import { NiveauEnum } from '../../../core/models/enums.models';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-quiz-detail',
  standalone: true,
  imports: [ AdminLayoutComponent,
    CommonModule,
    CardComponent,
    StatsCardComponent,
    RouterLink],
  templateUrl: './quiz-detail.component.html',
  styleUrl: './quiz-detail.component.scss'
})
export class QuizDetailComponent implements OnInit {
  quiz = signal<QuizDto | null>(null);
  contextNom = signal<string>('Chargement...');
  contextType = signal<'sprint' | 'module' | 'cours' | 'inconnu'>('inconnu');
  tauxReussiteGlobal = signal<number | null>(null);
  loading = signal(true);

  private quizId: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private quizService: QuizService,
    private moduleService: ModuleService,
    private coursService: CoursService,
    private sprintService: SprintService,
     private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.quizId = this.route.snapshot.paramMap.get('id')!;
    if (!this.quizId) {
      this.router.navigate(['/admin/quiz']);
      return;
    }
    this.loadQuiz();
  }

  private loadQuiz(): void {
    this.quizService.getQuizWithQuestions(this.quizId).subscribe({
      next: (quiz) => {
        this.quiz.set(quiz);
        this.tauxReussiteGlobal.set(this.calculateTauxReussite(quiz));
        this.loadContext(quiz);
      },
      error: () => {
        this.router.navigate(['/admin/quiz']);
      },
      complete: () => {
        this.loading.set(false);
      }
    });
  }

  private loadContext(quiz: QuizDto): void {
    const niveauId = quiz.niveauId;
    if (!niveauId) {
      this.contextNom.set('Aucun contexte');
      return;
    }

    switch (quiz.niveau) {
      case NiveauEnum.Sprint:
        this.sprintService.getSprintById(niveauId).subscribe({
          next: (sprint) => {
            this.contextNom.set(sprint.nom);
            this.contextType.set('sprint');
          },
          error: () => this.contextNom.set('Sprint inconnu')
        });
        break;
      case NiveauEnum.Module:
        this.moduleService.getModuleById(niveauId).subscribe({
          next: (module) => {
            this.contextNom.set(module.nom);
            this.contextType.set('module');
          },
          error: () => this.contextNom.set('Module inconnu')
        });
        break;
      case NiveauEnum.Cours:
        this.coursService.getCoursById(niveauId).subscribe({
          next: (cours) => {
            this.contextNom.set(cours.titre);
            this.contextType.set('cours');
          },
          error: () => this.contextNom.set('Cours inconnu')
        });
        break;
      default:
        this.contextNom.set('Contexte inconnu');
    }
  }

  private calculateTauxReussite(quiz: QuizDto): number | null {
    const tentatives = quiz.tentativesQuiz;
    if (!tentatives?.length) return null;
    const moyenne = tentatives.reduce((sum, t) => sum + t.score, 0) / tentatives.length;
    return Math.round(moyenne);
  }

  getNbQuestions(): number {
    return this.quiz()?.questions?.length || 0;
  }

  getNbTentatives(): number {
    return this.quiz()?.tentativesQuiz?.length || 0;
  }

  onEdit(): void {
    this.router.navigate(['/admin/quiz/edit', this.quizId]);
  }

  onDelete(): void {
    if (!this.quizId) return;

    if (confirm('Êtes-vous sûr de vouloir supprimer ce quiz ? Cette action est irréversible.')) {
      this.quizService.deleteQuiz(this.quizId).subscribe({
        next: () => {
          this.notificationService.success('Quiz supprimé avec succès.');
          this.router.navigate(['/admin/quiz']);
        },
        error: (err) => {
          console.error('Erreur lors de la suppression du quiz', err);
          this.notificationService.error('Échec de la suppression du quiz.');
        }
      });
    }
  }
}
