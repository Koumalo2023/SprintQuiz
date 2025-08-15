// src/app/admin/exercises/exercise-list.component.ts
import { Component, computed, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// Layouts & Shared
import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { ExerciseCardComponent } from '../../../shared/components/exercise-card/exercise-card.component';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { LoaderComponent } from '../../../shared/components/loader/loader.component';
import { StatsCardComponent } from '../../../shared/components/stats-card/stats-card.component';

// Services
import { ExerciceService } from '../../../core/services/exercice.service';
import { EnumService, NiveauDifficulte, NiveauEnum, TypeExercice } from '../../../core/services/enum.service';
import { NotificationService } from '../../../core/services/notification.service';

import { ExerciceDto } from '../../../core/models/exercice.model';


// Router
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-exercise-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    AdminLayoutComponent,
    ExerciseCardComponent,
    SearchBarComponent,
    PaginationComponent,
    LoaderComponent,
    StatsCardComponent,
    RouterLink
  ],
  templateUrl: './exercise-list.component.html',
  styleUrls: ['./exercise-list.component.scss']
})
export class ExerciseListComponent implements OnInit {
  loading = signal(true);
  exercises = signal<ExerciceDto[]>([]);

  // Filtres
  searchTerm = signal('');
  selectedNiveau = signal<NiveauEnum | null>(null);
  selectedDifficulte = signal<NiveauDifficulte | null>(null);
  selectedType = signal<TypeExercice | null>(null);

  // Pagination
  currentPage = signal(1);
  itemsPerPage = 10;

  // Stats
  totalExercises = signal(0);
  statsByNiveau = signal<Record<number, number>>({});
  statsByDifficulte = signal<Record<number, number>>({});

  // Énumérations
  NiveauEnum = NiveauEnum;
  NiveauDifficulte = NiveauDifficulte;
  TypeExercice = TypeExercice;

  // ✅ filteredExercises est un computed → mise à jour automatique
  filteredExercises = computed(() => {
    const term = this.searchTerm().toLowerCase();
    const niveau = this.selectedNiveau();
    const difficulte = this.selectedDifficulte();
    const type = this.selectedType();

    return this.exercises().filter(ex => {
      const matchesSearch = !term ||
        ex.enonce.toLowerCase().includes(term) ||
        (ex.tags?.some(t => t.toLowerCase().includes(term)) ?? false);

      const matchesNiveau = !niveau || ex.niveau === niveau;
      const matchesDifficulte = !difficulte || ex.niveauDifficulte === difficulte;
      const matchesType = !type || ex.type === type;

      return matchesSearch && matchesNiveau && matchesDifficulte && matchesType;
    });
  });


  constructor(
    private exerciceService: ExerciceService,
    public enumService: EnumService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadExercises();
  }

  loadExercises(): void {
    this.exerciceService.getAllExercices().subscribe({
      next: (data) => {
        console.log('Données reçues:', data);
        this.exercises.set(data);
        this.totalExercises.set(data.length);
        this.computeStats(data);
        this.loading.set(false); // ✅ Mettre à false ici
      },
      error: (err) => {
        console.error('Erreur chargement exercices:', err);
        this.notificationService.error('Impossible de charger les exercices');
        this.loading.set(false);
      }
    });
  }

  onSearchChange(term: string): void {
    console.log('Recherche :', term);
    this.searchTerm.set(term);
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
  }

  private computeStats(exercises: ExerciceDto[]): void {
    const byNiveau: Record<number, number> = {};
    const byDifficulte: Record<number, number> = {};

    exercises.forEach(ex => {
      byNiveau[ex.niveau] = (byNiveau[ex.niveau] || 0) + 1;
      byDifficulte[ex.niveauDifficulte] = (byDifficulte[ex.niveauDifficulte] || 0) + 1;
    });

    this.statsByNiveau.set(byNiveau);
    this.statsByDifficulte.set(byDifficulte);
  }

  paginatedExercises(): ExerciceDto[] {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    return this.filteredExercises().slice(start, start + this.itemsPerPage);
  }

  getTotalPages(): number {
    return Math.ceil(this.filteredExercises().length / this.itemsPerPage);
  }

  onCreateNew(): void {
    this.router.navigate(['/admin/exercises/create']);
  }

  getAriaLabel(ex: ExerciceDto): string {
    const excerpt = ex.enonce.length > 50 ? ex.enonce.slice(0, 50) + '...' : ex.enonce;
    return `Voir l’exercice : ${excerpt}`;
  }

  onCardClick(exercice: ExerciceDto): void {
   this.router.navigate([`/admin/exercices/${exercice.id}`]);
  }

  resetFilters(): void {
    this.searchTerm.set('');
    this.selectedNiveau.set(null);
    this.selectedDifficulte.set(null);
    this.selectedType.set(null);
  }


trackById(index: number, ex: ExerciceDto): string {
  return ex.id; // si c'est un string
}

}