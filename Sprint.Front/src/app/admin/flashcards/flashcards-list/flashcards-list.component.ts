// src/app/admin/flashcards/flashcards-list.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { SearchBarComponent } from '../../../shared/components/search-bar/search-bar.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

import { QaService } from '../../../core/services/qa.service';

import { Subscription } from 'rxjs';
import { QAQuestionDto } from '../../../core/models/qa-question.model';
import { NiveauDifficulte, NiveauEnum } from '../../../core/models/enums.models';

@Component({
  selector: 'app-flashcards-list',
  standalone: true,
  imports: [
    CommonModule,
    AdminLayoutComponent,
    CardComponent,
    SearchBarComponent,
    PaginationComponent,
    RouterLink
  ],
  templateUrl: './flashcards-list.component.html',
  styleUrls: ['./flashcards-list.component.scss']
})
export class FlashcardsListComponent implements OnInit {
  flashcards = signal<QAQuestionDto[]>([]);
  filteredFlashcards = signal<QAQuestionDto[]>([]);
  loading = signal(true);
  currentPage = signal(1);
  itemsPerPage = 10;
  totalPages = signal(1);
  searchTerm = signal('');

  private sub: Subscription = new Subscription();

  constructor(private qaService: QaService, private router: Router, ) {}

  ngOnInit(): void {
    this.loadFlashcards();
  }

  loadFlashcards(): void {
    this.loading.set(true);
    this.sub.add(
      this.qaService.getAllQAQuestions().subscribe({
        next: (data) => {
          this.flashcards.set(data);
          this.applyFilter();
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Erreur lors du chargement des flashcards', err);
          this.loading.set(false);
        }
      })
    );
  }

  onSearch(term: string): void {
    this.searchTerm.set(term);
    this.applyFilter();
  }

  applyFilter(): void {
    const term = this.searchTerm().toLowerCase();
    const filtered = this.flashcards().filter(f =>
      f.question.toLowerCase().includes(term) ||
      f.reponse.toLowerCase().includes(term) ||
      (f.tags?.some(tag => tag.toLowerCase().includes(term)) ?? false)
    );
    this.filteredFlashcards.set(filtered);
    this.totalPages.set(Math.ceil(filtered.length / this.itemsPerPage));
    this.currentPage.set(1);
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
  }

  getPaginatedFlashcards(): QAQuestionDto[] {
    const start = (this.currentPage() - 1) * this.itemsPerPage;
    return this.filteredFlashcards().slice(start, start + this.itemsPerPage);
  }

  // Helper pour afficher le niveau
  getNiveauLabel(niveau: NiveauEnum): string {
    return {
      [NiveauEnum.Cours]: 'Cours',
      [NiveauEnum.Module]: 'Module',
      [NiveauEnum.Sprint]: 'Sprint'
    }[niveau];
  }

  // Helper pour afficher la difficulté
  getDifficulteLabel(difficulte: NiveauDifficulte): string {
    return {
      [NiveauDifficulte.Facile]: 'Facile',
      [NiveauDifficulte.Moyen]: 'Moyen',
      [NiveauDifficulte.Difficile]: 'Difficile'
    }[difficulte];
  }

  // Couleur selon la difficulté
  getDifficulteClass(difficulte: NiveauDifficulte): string {
    return {
      [NiveauDifficulte.Facile]: 'badge-success',
      [NiveauDifficulte.Moyen]: 'badge-warning',
      [NiveauDifficulte.Difficile]: 'badge-danger'
    }[difficulte];
  }

  onCardClick(flashcard: QAQuestionDto): void {
    // Tu peux rediriger vers l'édition
    this.router.navigate(['/admin/flashcards/', flashcard.id]); 
  }
  // ✅ Méthode sécurisée pour les tags
  getTags(flashcard: QAQuestionDto): string[] {
    return flashcard.tags || [];
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}