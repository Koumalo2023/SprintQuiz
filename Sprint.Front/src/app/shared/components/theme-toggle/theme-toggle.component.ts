import { Component, inject } from '@angular/core';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-theme-toggle',
  standalone: true,
  imports: [],
  templateUrl: './theme-toggle.component.html',
  styleUrl: './theme-toggle.component.scss'
})
export class ThemeToggleComponent {
  private themeService = inject(ThemeService);

  icon(): string {
    const theme = this.themeService.getCurrentTheme();
    return theme === 'dark' ? '🌙' : theme === 'light' ? '☀️' : '🖥️';
  }

  nextThemeLabel(): string {
    const theme = this.themeService.getCurrentTheme();
    return theme === 'light' ? 'mode sombre' :
           theme === 'dark' ? 'mode système' : 'mode clair';
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
