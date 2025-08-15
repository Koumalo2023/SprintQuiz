import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { UserAvatarComponent } from '../../components/user-avatar/user-avatar.component';
import { RouterModule } from '@angular/router';


// Interface pour typer l'utilisateur
export interface User {
  id: number;
  name: string ;
  email: string | null;
  avatar: string | null; // ✅ uniformise avec le reste de l'app
}

@Component({
  selector: 'app-user-navigation-layout',
  imports: [CommonModule, RouterModule, UserAvatarComponent],
  templateUrl: './user-navigation-layout.component.html',
  styleUrl: './user-navigation-layout.component.scss'
})
export class UserNavigationLayoutComponent {
  @Input() fullWidth: boolean = false;
 sidebarOpen = false;
 // Déclare la propriété user avec @Input() pour la passer depuis l'extérieur
  @Input() user: User = {
    id:1,
    email:"user@poiuy.com",
    name: 'Utilisateur',
    avatar: null
  }; 

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebar(): void {
    this.sidebarOpen = false;
  }
}
