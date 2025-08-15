// src/app/admin/sprints/sprints-list.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { StatsCardComponent } from '../../../shared/components/stats-card/stats-card.component';

import { SprintService } from '../../../core/services/sprint.service';
import { SprintDto } from '../../../core/models/sprint.model';
import { AuthService } from '../../../core/services/auth.service';
import { NiveauEnum } from '../../../core/models/enums.models';

@Component({
  selector: 'app-sprints-list',
  standalone: true,
  imports: [
    AdminLayoutComponent,
    CardComponent,
    SearchBarComponent,
    PaginationComponent,
    StatsCardComponent,
    RouterLink,
  ],
  templateUrl: './sprints-list.component.html',
  styleUrl: './sprints-list.component.scss'
})
export class SprintsListComponent implements OnInit {
  sprints = signal<SprintDto[]>([]);
  filteredSprints = signal<SprintDto[]>([]);
  currentPage = signal(1);
  itemsPerPage = 10;
  totalItems = 0;
  totalPages = signal(1);
  searchTerm = signal('');
  currentUserId: string | null = null;

  constructor(private sprintsService: SprintService, private router:Router, private authService: AuthService) {
     this.currentUserId = this.authService.getCurrentUser()?.id || null;
  }

  ngOnInit(): void {
    this.loadSprints();
  }

  loadSprints(page: number = 1): void {
    this.sprintsService.getAllSprints().subscribe({
      next: (data) => {
        this.sprints.set(data);
        this.applyFilter(); // Appliquer filtre après chargement
        this.currentPage.set(page);
        this.totalItems = data.length;
        this.totalPages.set(Math.ceil(this.totalItems / this.itemsPerPage));
      },
      error: (err) => {
        console.error('Erreur lors du chargement des sprints', err);
      }
    });
  }

  onSearch(term: string): void {
    this.searchTerm.set(term);
    this.applyFilter();
  }

  applyFilter(): void {
    const term = this.searchTerm().toLowerCase();
    const filtered = this.sprints().filter(sprint =>
      sprint.nom.toLowerCase().includes(term) ||
      sprint.description?.toLowerCase().includes(term)
    );
    this.filteredSprints.set(filtered);
    this.totalItems = filtered.length;
    this.totalPages.set(Math.ceil(this.totalItems / this.itemsPerPage));
    this.currentPage.set(1); // Reset à la première page
  }
  onCardClick(sprint: SprintDto): void {
  // Rediriger vers la page de détail du sprint
  this.router.navigate(['/admin/sprints', sprint.id]);
}

// Méthode pour obtenir la progression de l'utilisateur dans un sprint
  getProgressForUser(sprint: SprintDto): number | null {
    if (!this.currentUserId) return null;

    const progression = sprint.progressions?.find(p =>
      p.utilisateurId === this.currentUserId &&
      p.niveau === NiveauEnum.Sprint &&
      p.niveauId === sprint.id
    );

    return progression ? progression.pourcentageComplet : 0; // 0 si pas encore commencé
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
  }

  getPaginatedSprints(): SprintDto[] {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    return this.filteredSprints().slice(start, start + this.itemsPerPage);
  }
}