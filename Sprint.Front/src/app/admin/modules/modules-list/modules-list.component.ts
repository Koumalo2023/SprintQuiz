// src/app/admin/modules/modules-list.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { StatsCardComponent } from '../../../shared/components/stats-card/stats-card.component';

import { ModuleService } from '../../../core/services/module.service';
import { ModuleDto } from '../../../core/models/module.model';
import { AuthService } from '../../../core/services/auth.service';  
import { NiveauEnum } from '../../../core/models/enums.models';


@Component({
  selector: 'app-modules-list',
  standalone: true,
  imports: [
    AdminLayoutComponent,
    CardComponent,
    SearchBarComponent,
    PaginationComponent,
    StatsCardComponent,
    RouterLink,
  ],
  templateUrl: './modules-list.component.html',
  styleUrls: ['./modules-list.component.scss']
})
export class ModulesListComponent implements OnInit {
  modules = signal<ModuleDto[]>([]);
  filteredModules = signal<ModuleDto[]>([]);
  currentPage = signal(1);
  itemsPerPage = 10;
  totalItems = 0;
  totalPages = signal(1);
  searchTerm = signal('');
  currentUserId: string | null = null;

  constructor(
    private moduleService: ModuleService,
    private router: Router,
    private authService: AuthService
  ) {
    const user = this.authService.getCurrentUser();
    this.currentUserId = user?.id || null;
  }

  ngOnInit(): void {
    this.loadModules();
  }

  loadModules(page: number = 1): void {
    this.moduleService.getAllModules().subscribe({
      next: (data) => {
        this.modules.set(data);
        this.applyFilter();
        this.currentPage.set(page);
        this.totalItems = data.length;
        this.totalPages.set(Math.ceil(this.totalItems / this.itemsPerPage));
      },
      error: (err) => {
        console.error('Erreur lors du chargement des modules', err);
      }
    });
  }

  onSearch(term: string): void {
    this.searchTerm.set(term);
    this.applyFilter();
  }

  applyFilter(): void {
    const term = this.searchTerm().toLowerCase();
    const filtered = this.modules().filter(module =>
      module.nom.toLowerCase().includes(term) ||
      module.description?.toLowerCase().includes(term)
    );
    this.filteredModules.set(filtered);
    this.totalItems = filtered.length;
    this.totalPages.set(Math.ceil(this.totalItems / this.itemsPerPage));
    this.currentPage.set(1);
  }

  onCardClick(module: ModuleDto): void {
    this.router.navigate(['/admin/modules', module.id]);
  }

  getProgressForUser(module: ModuleDto): number | null {
    if (!this.currentUserId) return null;

    const progression = module.progressions?.find(p =>
      p.utilisateurId === this.currentUserId &&
      p.niveau === NiveauEnum.Module &&
      p.niveauId === module.id
    );

    return progression ? progression.pourcentageComplet : 0;
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
  }

  getPaginatedModules(): ModuleDto[] {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    return this.filteredModules().slice(start, start + this.itemsPerPage);
  }
}