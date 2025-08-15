import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { EnumService } from '../../../core/services/enum.service';
import { ExerciceDto } from '../../../core/models/exercice.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-exercise-card',
   standalone: true,
  imports: [CommonModule],
  templateUrl: './exercise-card.component.html',
  styleUrl: './exercise-card.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExerciseCardComponent {
@Input() exercice!: ExerciceDto;
  @Input() showStatus = true;

  niveauMeta: any;
  typeMeta: any;
  difficulteMeta: any;

  constructor(private enumService: EnumService) {}

  ngOnInit(): void {
    this.niveauMeta = this.enumService.getNiveauMetadata(this.exercice.niveau);
    this.typeMeta = this.enumService.getTypeExerciceMetadata(this.exercice.type);
    this.difficulteMeta = this.enumService.getDifficulteMetadata(this.exercice.niveauDifficulte);
  }

  get truncatedEnonce(): string {
    return this.exercice.enonce.length > 80
      ? this.exercice.enonce.substring(0, 80) + '...'
      : this.exercice.enonce;
  }

  // ✅ Correction : gère le cas où tags est null
get tagsDisplay(): string {
  return this.exercice.tags?.join(', ') || '';
}
}
