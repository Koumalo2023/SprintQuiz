// src/app/user/user.routes.ts
import { Routes } from '@angular/router';

export const userRoutes: Routes = [
  // 🔹 Layout de navigation (dashboard, sprints, modules, cours, profil, stats...)
  {
    path: '',
    loadComponent: () => import('../shared/layouts/user-navigation-layout/user-navigation-layout.component').then(c => c.UserNavigationLayoutComponent),
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./dashboard/dashboard.component').then(c => c.DashboardComponent)
      },
      {
        path: 'sprints',
        loadComponent: () => import('./sprints/sprints-list/sprints-list.component').then(c => c.SprintsListComponent)
      },
      {
        path: 'sprints/:id',
        loadComponent: () => import('./sprints/sprint-detail/sprint-detail.component').then(c => c.SprintDetailComponent)
      },
      {
        path: 'modules',
        loadComponent: () => import('./modules/modules-list/modules-list.component').then(c => c.ModulesListComponent)
      },
      {
        path: 'modules/:id',
        loadComponent: () => import('./modules/module-detail/module-detail.component').then(c => c.ModuleDetailComponent)
      },
      {
        path: 'cours',
        loadComponent: () => import('./cours/cours-list/cours-list.component').then(c => c.CoursListComponent)
      },
      {
        path: 'cours/:id',
        loadComponent: () => import('./cours/cours-detail/cours-detail.component').then(c => c.CoursDetailComponent)
      },
      {
        path: 'stats',
        loadComponent: () => import('./stats/stats-dashboard/stats-dashboard.component').then(c => c.StatsDashboardComponent)
      },
      {
        path: 'progression',
        loadComponent: () => import('./progression/progression-tracker/progression-tracker.component').then(c => c.ProgressionTrackerComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./profile/profile-view/profile-view.component').then(c => c.ProfileViewComponent)
      },
      {
        path: 'profile/edit',
        loadComponent: () => import('./profile/profile-edit/profile-edit.component').then(c => c.ProfileEditComponent)
      },
      {
        path: 'profile/change-password',
        loadComponent: () => import('./profile/change-password/change-password.component').then(c => c.ChangePasswordComponent)
      },
      {
        path: 'profile/upload-avatar',
        loadComponent: () => import('./profile/upload-avatar/upload-avatar.component').then(c => c.UploadAvatarComponent)
      },
      { 
        path: 'exercices', loadComponent: () => import('./exercices/exercise-list/exercise-list.component').then(c => c.ExerciseListComponent) },
      {
        path: 'exercices/detail/:id',
        loadComponent: () => import('./exercices/exercise-detail/exercise-detail.component').then(c => c.ExerciseDetailComponent)
      },
      {
        path: 'exercices/ma-revision',
        loadComponent: () => import('./exercices/my-revision/my-revision.component').then(c => c.MyRevisionComponent)
      },
      {
        path: 'exercices/consultations',
        loadComponent: () => import('./exercices/exercise-consultations/exercise-consultations.component').then(c => c.ExerciseConsultationsComponent)
      },

      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },

  // 🔹 Layout de révision (quiz, flashcards, exercices)
  {
    path: '',
    loadComponent: () => import('../shared/layouts/user-layout/user-layout.component').then(c => c.UserLayoutComponent),
    children: [
      {
        path: 'quiz/:id',
        loadComponent: () => import('./quiz/quiz-attempt/quiz-attempt.component').then(c => c.QuizAttemptComponent)
      },
      {
        path: 'flashcards/:id',
        loadComponent: () => import('./flashcards/flashcards-practice/flashcards-practice.component').then(c => c.FlashcardsPracticeComponent)
      },
      // 🔹 NOUVEAU : Mode pratique Exercice
      {
        path: 'exercices/practice/:id',
        loadComponent: () => import('./exercices/exercise-practice/exercise-practice.component').then(c => c.ExercisePracticeComponent)
      }
    ]
  },

  // 🔹 Redirection racine
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: 'dashboard' }
];