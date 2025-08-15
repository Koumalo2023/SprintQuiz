import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-stats-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stats-card.component.html',
  styleUrl: './stats-card.component.scss'
})
export class StatsCardComponent {
  @Input() value: string | number = 0;
  @Input() label: string = '';
  @Input() icon: string = ''; // Nom de l'icône (ex: "quiz", "flashcard", "user")
  @Input() trend?: 'up' | 'down'; // Optionnel : tendance
  @Input() trendValue?: string; // Ex: "+12%"

  // 🔹 Ajout des inputs manquants
  @Input() bgColor: string = '#f8f9fa';   // Couleur de fond
  @Input() textColor: string = '#212529'; // Couleur du texte
}
