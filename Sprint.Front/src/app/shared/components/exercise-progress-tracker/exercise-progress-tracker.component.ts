import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { EnumService, NiveauEnum } from '../../../core/services/enum.service';
import { CommonModule } from '@angular/common';


interface ProgressItem {
  id: string;
  titre: string;
  type: number; // NiveauEnum
  total: number;
  maitrises: number;
}

@Component({
  selector: 'app-exercise-progress-tracker',
   standalone: true,
  imports: [CommonModule],
  templateUrl: './exercise-progress-tracker.component.html',
  styleUrl: './exercise-progress-tracker.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ExerciseProgressTrackerComponent {
  @Input() items: ProgressItem[] = [];

  constructor(public enumService: EnumService) {}

  getProgress(item: ProgressItem): number {
    return item.total > 0 ? (item.maitrises / item.total) * 100 : 0;
  }

  getProgressColor(item: ProgressItem): string {
    const p = this.getProgress(item);
    if (p < 50) return '#e83e8c'; // rouge
    if (p < 80) return '#f57a00'; // orange
    return '#22b24c'; // vert
  }

  // ✅ Nouvelle méthode : retourne l'icône du niveau
  getIcon(type: NiveauEnum): string {
    return this.enumService.getNiveauMetadata(type)?.icon || 'help';
  }
}
