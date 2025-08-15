// src/app/admin/flashcards/flashcard-detail.component.ts
import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';

import { AdminLayoutComponent } from '../../../shared/layouts/admin-layout/admin-layout.component';
import { CardComponent } from '../../../shared/components/card/card.component';
import { LoaderComponent } from '../../../shared/components/loader/loader.component';

import { QaService } from '../../../core/services/qa.service';
import { NotificationService } from '../../../core/services/notification.service';

import { QAQuestionDto,} from '../../../core/models/qa-question.model';
import { NiveauDifficulte, NiveauEnum } from '../../../core/models/enums.models';

@Component({
  selector: 'app-flashcard-detail',
  standalone: true,
  imports: [
    CommonModule,
    AdminLayoutComponent,
    CardComponent,
    
  ],
  templateUrl: './flashcard-detail.component.html',
  styleUrls: ['./flashcard-detail.component.scss']
})
export class FlashcardDetailComponent implements OnInit {
  flashcard = signal<QAQuestionDto | null>(null);
  loading = signal(true);

  // Énumérations pour le template
  NiveauEnum = NiveauEnum;
  NiveauDifficulte = NiveauDifficulte;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private qaService: QaService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.router.navigate(['/admin/flashcards']);
      return;
    }
    this.loadFlashcard(id);
  }

  loadFlashcard(id: string): void {
    this.loading.set(true);
    this.qaService.getQAQuestionById(id).subscribe({
      next: (data) => {
        this.flashcard.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.notificationService.error('Impossible de charger la flashcard');
        this.loading.set(false);
        this.router.navigate(['/admin/flashcards']);
      }
    });
  }

  // Helper pour afficher le niveau
  getNiveauLabel(niveau: NiveauEnum): string {
    const labels: Record<NiveauEnum, string> = {
      [NiveauEnum.Cours]: 'Cours',
      [NiveauEnum.Module]: 'Module',
      [NiveauEnum.Sprint]: 'Sprint'
    };
    return labels[niveau];
  }

  // Helper pour afficher la difficulté
  getDifficulteLabel(difficulte: NiveauDifficulte): string {
    const labels: Record<NiveauDifficulte, string> = {
      [NiveauDifficulte.Facile]: 'Facile',
      [NiveauDifficulte.Moyen]: 'Moyen',
      [NiveauDifficulte.Difficile]: 'Difficile'
    };
    return labels[difficulte];
  }

  // Couleur selon la difficulté
  getDifficulteClass(difficulte: NiveauDifficulte): string {
    const classes: Record<NiveauDifficulte, string> = {
      [NiveauDifficulte.Facile]: 'badge-success',
      [NiveauDifficulte.Moyen]: 'badge-warning',
      [NiveauDifficulte.Difficile]: 'badge-danger'
    };
    return classes[difficulte];
  }

  // Navigation
  onEdit(): void {
    this.router.navigate(['/admin/flashcards/edit', this.flashcard()?.id]);
  }

  onDelete(): void {
    if (!this.flashcard()) return;

    const confirmed = window.confirm(
      `Êtes-vous sûr de vouloir supprimer la flashcard : "${this.flashcard()?.question}" ?`
    );

    if (confirmed) {
      this.qaService.deleteQAQuestion(this.flashcard()!.id).subscribe({
        next: () => {
          this.notificationService.success('Flashcard supprimée avec succès');
          this.router.navigate(['/admin/flashcards']);
        },
        error: () => {
          this.notificationService.error('Échec de la suppression de la flashcard');
        }
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/admin/flashcards']);
  }
}