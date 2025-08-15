import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { ExerciceDto } from '../../../core/models/exercice.model';
import { CommonModule } from '@angular/common';
import { EnumService } from '../../../core/services/enum.service';

@Component({
  selector: 'app-exercise-solution',
   standalone: true,
  imports: [CommonModule],
  templateUrl: './exercise-solution.component.html',
  styleUrl: './exercise-solution.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExerciseSolutionComponent {
@Input() exercice!: ExerciceDto;

 constructor(private enumService: EnumService) {}

  // Retourne la classe CSS du type d'exercice
  getTypeClass(): string {
    return this.enumService.getTypeExerciceMetadata(this.exercice.type)?.class || 'badge-secondary';
  }

  // Retourne le label du type d'exercice
  getTypeLabel(): string {
    return this.enumService.getTypeExerciceMetadata(this.exercice.type)?.label || 'Inconnu';
  }
}
