import { CommonModule } from '@angular/common';
import { Component } from '@angular/core'; 
import { CardComponent } from '../../shared/components/card/card.component'; 
import { StatsCardComponent } from '../../shared/components/stats-card/stats-card.component';
import { ProgressRingComponent } from '../../shared/components/progress-ring/progress-ring.component';
import { SearchBarComponent } from '../../shared/components/search-bar/search-bar.component';
import { UserAvatarComponent } from '../../shared/components/user-avatar/user-avatar.component';
import { UserNavigationLayoutComponent } from '../../shared/layouts/user-navigation-layout/user-navigation-layout.component';


interface SprintProgress {
  id: number;
  title: string;
  moduleCount: number;
  progress: number; // 0-100
  lastAccess: string;
}

interface Stat {
  value: string | number;
  label: string;
  icon: string;
  trend?: 'up' | 'down'; // ✅ Type exact attendu
  trendValue?: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, UserNavigationLayoutComponent, StatsCardComponent, CardComponent, ProgressRingComponent, SearchBarComponent, UserAvatarComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  

  onCardClicked() {
  console.log('Carte cliquée !');
  }

  // Statistiques principales
 stats: Stat[] = [
    { value: 15, label: 'Sprints actifs', icon: 'sprint', trend: 'up', trendValue: '+2' },
    { value: 47, label: 'Quiz terminés', icon: 'quiz', trend: 'up', trendValue: '+5' },
    { value: '82%', label: 'Moyenne générale', icon: 'grade', trend: 'up', trendValue: '+3%' },
    { value: 126, label: 'Flashcards maîtrisées', icon: 'flashcard', trend: 'up', trendValue: '+15' }
  ];

  // Sprints en cours
  sprints: SprintProgress[] = [
    { id: 1, title: 'Sprint 1 - Biologie cellulaire', moduleCount: 4, progress: 75, lastAccess: 'Aujourd’hui' },
    { id: 2, title: 'Sprint 2 - Génétique', moduleCount: 3, progress: 40, lastAccess: 'Hier' },
    { id: 3, title: 'Sprint 3 - Physiologie', moduleCount: 5, progress: 10, lastAccess: 'Il y a 3 jours' }
  ];

  // Prochaines actions
  nextActions = [
    { title: 'Quiz - Mitose', subtitle: 'Module 2 - Biologie cellulaire', type: 'quiz', due: 'Aujourd’hui' },
    { title: 'Réviser flashcards', subtitle: 'Génétique - Chromosomes', type: 'flashcard', due: 'Demain' },
    { title: 'Compléter module 3', subtitle: 'Physiologie - Système nerveux', type: 'module', due: '7 jours' }
  ];

  // Données utilisateur
  user = {
  name: 'Marie Dubois',
  avatar: 'https://example.com/avatar.jpg' // optionnel
};

  // Recherche
  onSearch(term: string) {
    console.log('Recherche:', term);
    // À connecter à un service plus tard
  }

  getSubtitle(sprint: SprintProgress): string {
  return `${sprint.moduleCount} modules`;
}
  onFilterToggle() {
    console.log('Ouvrir les filtres');
  }
}


