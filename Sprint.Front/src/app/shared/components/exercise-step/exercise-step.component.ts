import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { EtapeResolutionDto } from '../../../core/models/exercice.model';

@Component({
  selector: 'app-exercise-step',
   standalone: true,
  imports: [CommonModule],
  templateUrl: './exercise-step.component.html',
  styleUrl: './exercise-step.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExerciseStepComponent {
@Input() step!: EtapeResolutionDto;
@Input() showNumber = true;
}
