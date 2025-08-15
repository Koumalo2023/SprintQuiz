import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { IndiceDto } from '../../../core/models/exercice.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-exercise-hint',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './exercise-hint.component.html',
  styleUrl: './exercise-hint.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExerciseHintComponent {
@Input() hint!: IndiceDto;
  isVisible = false;

  toggleVisibility(): void {
    this.isVisible = !this.isVisible;
  }
}
