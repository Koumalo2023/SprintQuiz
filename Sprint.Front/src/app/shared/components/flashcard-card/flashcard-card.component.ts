import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { EnumService, NiveauEnum, TypeExercice } from '../../../core/services/enum.service';

@Component({
  selector: 'app-flashcard-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './flashcard-card.component.html',
  styleUrl: './flashcard-card.component.scss'
})
export class FlashcardCardComponent implements OnInit {
  @Input() title: string = 'Flashcards';
  @Input() total: number = 0;
  @Input() mastered: number = 0;
  @Input() inProgress: number = 0;
  @Input() status: 'in-progress' | 'completed' | 'locked' = 'in-progress';
  @Input() disabled: boolean = false;

  

  // 🔹 Ajout des métadonnées dynamiques
  @Input() niveau: NiveauEnum | null = null;
  @Input() typeExercice: TypeExercice | null = null;

  niveauMeta: any;
  typeMeta: any;

  // Pour les ID uniques dans les aria-labels
  uniqueId = Math.random().toString(36).substring(9);

  constructor(private enumService: EnumService) {}

  ngOnInit(): void {
    if (this.niveau !== null) {
      this.niveauMeta = this.enumService.getNiveauMetadata(this.niveau);
    }
    if (this.typeExercice !== null) {
      this.typeMeta = this.enumService.getTypeExerciceMetadata(this.typeExercice);
    }
  }

  get progressPercentage(): number {
    return this.total > 0 ? Math.round((this.mastered / this.total) * 100) : 0;
  }
}