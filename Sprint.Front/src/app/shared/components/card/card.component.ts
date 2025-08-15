import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { EnumService, NiveauEnum, TypeExercice } from '../../../core/services/enum.service';

@Component({
  selector: 'app-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './card.component.html',
  styleUrl: './card.component.scss'
})
export class CardComponent {
  @Input() title: string = '';
  @Input() subtitle: string = '';
  @Input() icon: string = '';
  @Input() footer: string = '';
  @Input() progress: number | null = null;
  @Input() clickable: boolean = false;
  @Input() disabled: boolean = false;

  // 🔹 Nouveaux inputs pour le support EnumService
  @Input() niveau: NiveauEnum | null = null;
  @Input() typeExercice: TypeExercice | null = null;
  @Input() difficulte: number | null = null;

  // Métadonnées dynamiques
  niveauMeta: any;
  typeMeta: any;
  difficulteMeta: any;

  @Output() cardClick = new EventEmitter<MouseEvent>();

  constructor(private enumService: EnumService) {}

  ngOnInit(): void {
    if (this.niveau !== null) {
      this.niveauMeta = this.enumService.getNiveauMetadata(this.niveau);
    }
    if (this.typeExercice !== null) {
      this.typeMeta = this.enumService.getTypeExerciceMetadata(this.typeExercice);
    }
    if (this.difficulte !== null) {
      this.difficulteMeta = this.enumService.getDifficulteMetadata(this.difficulte);
    }
  }

  handleClick(event: MouseEvent): void {
    if (this.clickable && !this.disabled) {
      this.cardClick.emit(event);
    }
  }
}
