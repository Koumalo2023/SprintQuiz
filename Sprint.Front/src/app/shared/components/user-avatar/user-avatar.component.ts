// src/app/shared/components/user-avatar/user-avatar.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-user-avatar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-avatar.component.html',
  styleUrl: './user-avatar.component.scss'
})
export class UserAvatarComponent {
 @Input() name: string | undefined;
  @Input() imageUrl: string | null | undefined; 
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() rounded: 'none' | 'sm' | 'md' | 'full' = 'full';

  get initials(): string {
    if (!this.name) return '?';
    return this.name
      .split(' ')
      .map(part => part[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  }

  get avatarClass(): string {
    return `
      user-avatar
      user-avatar--${this.size}
      user-avatar--${this.rounded}
      ${this.imageUrl ? 'user-avatar--image' : 'user-avatar--placeholder'}
    `.trim();
  }
}
