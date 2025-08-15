// src/app/admin/cours/cours-list.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { StatsCardComponent } from '../../../shared/components/stats-card/stats-card.component';

import { CoursService } from '../../../core/services/cours.service';
import { CoursDto } from '../../../core/models/cours.model';
import { ModuleService } from '../../../core/services/module.service';
import { ModuleDto } from '../../../core/models/module.model';
import { AuthService } from '../../../core/services/auth.service'; 
import { NiveauEnum } from '../../../core/models/enums.models';

@Component({
  selector: 'app-cours-list',
  standalone: true,
  imports: [
    AdminLayoutComponent,
    CardComponent,
    SearchBarComponent,
    PaginationComponent,
    StatsCardComponent,
    RouterLink,
  ],
  templateUrl: './cours-list.component.html',
  styleUrls: ['./cours-list.component.scss']
})
export class CoursListComponent implements OnInit {
  cours = signal<CoursDto[]>([]);
  filteredCours = signal<CoursDto[]>([]);
  currentPage = signal(1);
  itemsPerPage = 10;
  totalItems = 0;
  totalPages = signal(1);
  searchTerm = signal('');
  currentUserId: string | null = null;

  // Pour afficher le nom du module
  modulesMap = new Map<string, string>();

  constructor(
    private coursService: CoursService,
    private moduleService: ModuleService,
    private router: Router,
    private authService: AuthService
  ) {
    const user = this.authService.getCurrentUser();
    this.currentUserId = user?.id || null;
  }

  ngOnInit(): void {
    this.loadCours();
    this.loadAllModules();
  }

  loadCours(page: number = 1): void {
    this.coursService.getAllCours().subscribe({
      next: (data) => {
        this.cours.set(data);
        this.applyFilter();
        this.currentPage.set(page);
        this.totalItems = data.length;
        this.totalPages.set(Math.ceil(this.totalItems / this.itemsPerPage));
      },
      error: (err) => {
        console.error('Erreur lors du chargement des cours', err);
      }
    });
  }

  loadAllModules(): void {
    this.moduleService.getAllModules().subscribe({
      next: (modules) => {
        modules.forEach(m => this.modulesMap.set(m.id, m.nom));
      },
      error: () => {
        console.warn('Impossible de charger les modules pour les labels');
      }
    });
  }

  onSearch(term: string): void {
    this.searchTerm.set(term);
    this.applyFilter();
  }

  applyFilter(): void {
    const term = this.searchTerm().toLowerCase();
    const filtered = this.cours().filter(c =>
      c.titre.toLowerCase().includes(term) ||
      c.description?.toLowerCase().includes(term) ||
      (this.modulesMap.get(c.moduleId || '') || '').toLowerCase().includes(term)
    );
    this.filteredCours.set(filtered);
    this.totalItems = filtered.length;
    this.totalPages.set(Math.ceil(this.totalItems / this.itemsPerPage));
    this.currentPage.set(1);
  }

  onCardClick(cours: CoursDto): void {
    this.router.navigate(['/admin/cours', cours.id]);
  }

  getModuleName(moduleId: string | null): string {
    return moduleId ? this.modulesMap.get(moduleId) || 'Module inconnu' : 'Aucun module';
  }

  getProgressForUser(cours: CoursDto): number | null {
    if (!this.currentUserId) return null;

    const progression = cours.progressions?.find(p =>
      p.utilisateurId === this.currentUserId &&
      p.niveau === NiveauEnum.Cours &&
      p.niveauId === cours.id
    );

    return progression ? progression.pourcentageComplet : 0;
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
  }

  getPaginatedCours(): CoursDto[] {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    return this.filteredCours().slice(start, start + this.itemsPerPage);
  }
}