import { CommonModule } from '@angular/common';
import { Component, ContentChild, Input, TemplateRef } from '@angular/core';
import { UserAvatarComponent } from '../../components/user-avatar/user-avatar.component';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from 'express';

@Component({
  selector: 'app-user-layout',
  imports: [CommonModule, RouterModule, UserAvatarComponent],
  standalone: true,
  templateUrl: './user-layout.component.html',
  styleUrl: './user-layout.component.scss'
})
export class UserLayoutComponent {
  @Input() title: string = 'Tableau de bord';
  @Input() subtitle: string = '';
  @Input() progress: number = 0; // 0 à 100
  @Input() showProgressSidebar: boolean = true;
  @Input() fullWidth: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  // Contrôle du sidebar mobile
  sidebarOpen = false;

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

}
