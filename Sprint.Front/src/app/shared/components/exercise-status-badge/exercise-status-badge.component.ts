import { ChangeDetectionStrategy, Component, Input } from '@angular/core';


type Status = 'not-started' | 'in-progress' | 'mastered' | 'to-review';
@Component({
  selector: 'app-exercise-status-badge',
   standalone: true,
  imports: [],
  templateUrl: './exercise-status-badge.component.html',
  styleUrl: './exercise-status-badge.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExerciseStatusBadgeComponent {
@Input() status: Status = 'not-started';

  get config() {
    const map = {
      'not-started': { label: 'Non commencé', color: '#adb5bd', icon: 'circle' },
      'in-progress': { label: 'En cours', color: '#f57a00', icon: 'play-circle' },
      'mastered': { label: 'Maîtrisé', color: '#22b24c', icon: 'check-circle' },
      'to-review': { label: 'À revoir', color: '#e83e8c', icon: 'refresh' }
    };
    return map[this.status];
  }
}
