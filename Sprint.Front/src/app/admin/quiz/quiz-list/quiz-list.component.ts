import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { QuizDto } from '../../../core/models/quiz.model';
import { QuizService } from '../../../core/services/quiz.service';
import { ModuleService } from '../../../core/services/module.service';
import { CoursService } from '../../../core/services/cours.service';
import { SprintService } from '../../../core/services/sprint.service';


interface QuizViewModel {
  quiz: QuizDto;
  contextNom: string;
  contextType: 'sprint' | 'module' | 'cours' | 'autre';
  tauxReussiteGlobal: number | null;
}
@Component({
  selector: 'app-quiz-list',
  standalone: true,
  imports: [
    AdminLayoutComponent,
    CardComponent,
    SearchBarComponent,
    PaginationComponent,
    RouterLink
  ],
  templateUrl: './quiz-list.component.html',
  styleUrls: ['./quiz-list.component.scss']
})
 export class QuizListComponent implements OnInit {
  quizzes = signal<QuizViewModel[]>([]);
  filteredQuizzes = signal<QuizViewModel[]>([]);
  currentPage = signal(1);
  itemsPerPage = 10;
  totalItems = 0;
  totalPages = signal(1);
  searchTerm = signal('');

  // Services
  constructor(
    
    private router: Router, 
    private quizService: QuizService,
    private moduleService: ModuleService,
    private coursService: CoursService,
    private sprintService: SprintService
  ) {}
   

  private contextMap = new Map<string, { nom: string; type: 'sprint' | 'module' | 'cours' }>();

  ngOnInit(): void {
    this.loadAllContexts().then(() => {
      this.loadQuizzes();
    });
  }

  private async loadAllContexts(): Promise<void> {
    try {
      const [sprints, modules, cours] = await Promise.all([
        this.sprintService.getAllSprints().toPromise() || [],
        this.moduleService.getAllModules().toPromise() || [],
        this.coursService.getAllCours().toPromise() || []
      ]);

      // Remplir la map
      sprints!.forEach(s => this.contextMap.set(s.id, { nom: s.nom, type: 'sprint' }));
      modules!.forEach(m => this.contextMap.set(m.id, { nom: m.nom, type: 'module' }));
      cours!.forEach(c => this.contextMap.set(c.id, { nom: c.titre, type: 'cours' }));
    } catch (err) {
      console.error('Erreur lors du chargement des contextes', err);
    }
  }

  loadQuizzes(): void {
    this.quizService.getAllQuizzes().subscribe({
      next: (quizDtos) => {
        const viewModels = quizDtos.map(quiz => {
          const context = this.contextMap.get(quiz.niveauId!) || { nom: 'Inconnu', type: 'autre' };
          const taux = this.calculateTauxReussite(quiz);

          return {
            quiz,
            contextNom: context.nom,
            contextType: context.type,
            tauxReussiteGlobal: taux
          };
        });

        this.quizzes.set(viewModels);
        this.applyFilter();
      },
      error: (err) => {
        console.error('Erreur lors du chargement des quiz', err);
      }
    });
  }

  private calculateTauxReussite(quiz: QuizDto): number | null {
    const tentatives = quiz.tentativesQuiz;
    if (!tentatives?.length) return null;

    const moyenne = tentatives.reduce((sum, t) => sum + t.score, 0) / tentatives.length;
    return Math.round(moyenne); // ex: 75
  }

  onSearch(term: string): void {
    this.searchTerm.set(term);
    this.applyFilter();
  }

  applyFilter(): void {
    const term = this.searchTerm().toLowerCase();
    const filtered = this.quizzes().filter(vm =>
      vm.quiz.titre.toLowerCase().includes(term) ||
      vm.quiz.description?.toLowerCase().includes(term) ||
      vm.contextNom.toLowerCase().includes(term)
    );

    this.filteredQuizzes.set(filtered);
    this.totalItems = filtered.length;
    this.totalPages.set(Math.ceil(this.totalItems / this.itemsPerPage));
    this.currentPage.set(1);
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
  }

  getPaginatedQuizzes(): QuizViewModel[] {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    return this.filteredQuizzes().slice(start, start + this.itemsPerPage);
  }

  // ✅ Méthode ajoutée pour gérer le clic sur la carte
  onCardClick(quiz: QuizDto): void {
    this.router.navigate(['/admin/quiz/', quiz.id]);
  }
}
