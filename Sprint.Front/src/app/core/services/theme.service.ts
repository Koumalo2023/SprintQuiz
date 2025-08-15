// src/app/core/services/theme.service.ts

import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly THEME_KEY = 'sprintquiz-theme';
  private readonly DARK_THEME = 'dark';
  private readonly LIGHT_THEME = 'light';
  private readonly SYSTEM_THEME = 'system';

  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  init() {
    if (!isPlatformBrowser(this.platformId)) return;

    const savedTheme = localStorage.getItem(this.THEME_KEY) || this.SYSTEM_THEME;

    if (this.isValidTheme(savedTheme)) {
      this.setTheme(savedTheme);
    } else {
      this.setTheme(this.SYSTEM_THEME);
    }

    this.watchSystemTheme();
  }

  private isValidTheme(theme: string | null): theme is 'light' | 'dark' | 'system' {
    return ['light', 'dark', 'system'].includes(theme as any);
  }

  setTheme(theme: 'light' | 'dark' | 'system') {
    if (!isPlatformBrowser(this.platformId)) return;

    const html = document.documentElement;

    // On ne touche plus aux classes, uniquement à data-theme
    if (theme === 'system') {
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      html.setAttribute('data-theme', prefersDark ? this.DARK_THEME : this.LIGHT_THEME);
    } else {
      html.setAttribute('data-theme', theme);
    }

    localStorage.setItem(this.THEME_KEY, theme);
  }

  getCurrentTheme(): 'light' | 'dark' | 'system' {
    return (localStorage.getItem(this.THEME_KEY) as 'light' | 'dark' | 'system') || 'system';
  }

  toggleTheme(): 'light' | 'dark' | 'system' {
    const current = this.getCurrentTheme();
    let next: 'light' | 'dark' | 'system' = 'light';

    if (current === 'light') next = 'dark';
    else if (current === 'dark') next = 'system';
    else if (current === 'system') next = 'light';

    this.setTheme(next);
    return next;
  }

  watchSystemTheme() {
    if (!isPlatformBrowser(this.platformId)) return;

    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    const handleChange = () => {
      if (this.getCurrentTheme() === 'system') {
        const prefersDark = mediaQuery.matches;
        document.documentElement.setAttribute('data-theme', prefersDark ? 'dark' : 'light');
      }
    };

    handleChange(); // Appliquer au démarrage
    mediaQuery.addEventListener('change', handleChange);
  }
}