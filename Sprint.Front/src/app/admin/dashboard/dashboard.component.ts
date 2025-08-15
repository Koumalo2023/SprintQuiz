//src/app/admin/dashboard/dashboard.component.ts
import { Component } from '@angular/core';
import { AdminLayoutComponent } from '../../shared/layouts/admin-layout/admin-layout.component';
import { BreadcrumbComponent } from '../../shared/components/breadcrumb/breadcrumb.component';
import { StatsCardComponent } from '../../shared/components/stats-card/stats-card.component';
import { SearchBarComponent } from '../../shared/components/search-bar/search-bar.component';


interface Activity {
  avatar: string;
  user: string;
  action: string;
  time: string;
  status: 'completed' | 'pending' | 'failed';
}

interface Notification {
  type: 'alert' | 'info' | 'success';
  message: string;
  time: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [AdminLayoutComponent, BreadcrumbComponent,
    StatsCardComponent,
    SearchBarComponent ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
// Statistiques
  stats = [
    { title: 'Utilisateurs actifs', value: 1247, icon: 'users', trend: '+12%', status: 'positive' },
    { title: 'Quiz non corrigés', value: 23, icon: 'quiz', trend: '+5', status: 'neutral' },
    { title: 'Progression moyenne', value: '73%', icon: 'progress', trend: '+8%', status: 'positive' },
    { title: 'Contenus publiés', value: 156, icon: 'content', trend: 'stable', status: 'neutral' }
  ];

   breadcrumbItems = [
    { label: 'Accueil', route: '/admin' },
    { label: 'Tableau de bord', route: '' } // ou sans route pour le dernier
  ];

  // Accès rapide
  quickActions = [
    { icon: 'users', label: 'Gestion utilisateurs', description: 'Ajouter, modifier, supprimer', link: '/admin/users' },
    { icon: 'content', label: 'Gestion contenus', description: 'Sprints, modules, cours', link: '/admin/content' },
    { icon: 'reports', label: 'Rapports', description: 'Statistiques détaillées', link: '/admin/reports' },
    { icon: 'settings', label: 'Paramètres', description: 'Configuration système', link: '/admin/settings' }
  ];

  // Dernières activités
  activities = [
    { avatar: 'NU', text: 'Quiz terminé avec succès', time: 'À l’instant', status: 'completed' },
    { avatar: 'NU', text: 'Un nouvel utilisateur s’est inscrit', time: 'À l’instant', status: 'new' },
    { avatar: 'NU', text: 'Nouveau contenu publié', time: 'À l’instant', status: 'new' },
    { avatar: 'NU', text: 'Badge obtenu par un étudiant', time: 'À l’instant', status: 'completed' }
  ];
  // Notifications (ex: pour le search bar)
  onSearch(query: string) {
    console.log('Recherche:', query);
  }

  // Notifications
  notifications = [
    { type: 'alert', message: '5 quiz en attente de validation', time: 'Il y a 10 minutes' },
    { type: 'info', message: 'Mise à jour système disponible', time: 'Il y a 2 heures' },
    { type: 'success', message: 'Sauvegarde automatique réussie', time: 'Il y a 6 heures' }
  ];

  // Animation des valeurs (simulée)
  ngOnInit(): void {
    this.animateStats();
  }

  private animateStats(): void {
    // En vrai, tu pourrais animer avec une directive ou un service
    console.log('Animation des stats démarrée');
  }

 
}
