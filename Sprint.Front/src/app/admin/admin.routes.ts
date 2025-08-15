// src/app/admin/admin.routes.ts
import { Routes } from '@angular/router';

export const adminRoutes: Routes = [
  {
    path: 'dashboard',
    loadComponent: () => import('./dashboard/dashboard.component').then(c => c.DashboardComponent)
  },

  // Gestion des contenus
  {
    path: 'sprints',
    loadComponent: () => import('./sprints/sprints-list/sprints-list.component').then(c => c.SprintsListComponent)
  },
  {
    path: 'sprints/create',
    loadComponent: () => import('./sprints/sprint-form/sprint-form.component').then(c => c.SprintFormComponent)
  },
  {
    path: 'sprints/edit/:id',
    loadComponent: () => import('./sprints/sprint-form/sprint-form.component').then(c => c.SprintFormComponent)
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
    path: 'modules/create',
    loadComponent: () => import('./modules/module-form/module-form.component').then(c => c.ModuleFormComponent)
  },
  {
    path: 'modules/edit/:id',
    loadComponent: () => import('./modules/module-form/module-form.component').then(c => c.ModuleFormComponent)
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
    path: 'cours/create',
    loadComponent: () => import('./cours/cours-form/cours-form.component').then(c => c.CoursFormComponent)
  },
  {
    path: 'cours/edit/:id',
    loadComponent: () => import('./cours/cours-form/cours-form.component').then(c => c.CoursFormComponent)
  },
  {
    path: 'cours/:id',
    loadComponent: () => import('./cours/cours-detail/cours-detail.component').then(c => c.CoursDetailComponent)
  },

  {
    path: 'quiz',
    loadComponent: () => import('./quiz/quiz-list/quiz-list.component').then(c => c.QuizListComponent)
  },
  {
    path: 'quiz/create',
    loadComponent: () => import('./quiz/quiz-form/quiz-form.component').then(c => c.QuizFormComponent)
  },
  {
    path: 'quiz/edit/:id',
    loadComponent: () => import('./quiz/quiz-form/quiz-form.component').then(c => c.QuizFormComponent)
  },
  {
    path: 'quiz/:id',
    loadComponent: () => import('./quiz/quiz-detail/quiz-detail.component').then(c => c.QuizDetailComponent)
  },

  {
    path: 'flashcards',
    loadComponent: () => import('./flashcards/flashcards-list/flashcards-list.component').then(c => c.FlashcardsListComponent)
  },
  {
    path: 'flashcards/create',
    loadComponent: () => import('./flashcards/flashcard-form/flashcard-form.component').then(c => c.FlashcardFormComponent)
  },
  {
    path: 'flashcards/edit/:id',
    loadComponent: () => import('./flashcards/flashcard-form/flashcard-form.component').then(c => c.FlashcardFormComponent)
  },
  {
    path: 'flashcards/:id',
    loadComponent: () => import('./flashcards/flashcard-detail/flashcard-detail.component').then(c => c.FlashcardDetailComponent)
  },

  // 🔹 NOUVEAU : Gestion des Exercices
  {
    path: 'exercices',
    loadComponent: () => import('./exercices/exercise-list/exercise-list.component').then(c => c.ExerciseListComponent)
  },
  {
    path: 'exercices/create',
    loadComponent: () => import('./exercices/exercise-form/exercise-form.component').then(c => c.ExerciseFormComponent)
  },
  {
    path: 'exercices/edit/:id',
    loadComponent: () => import('./exercices/exercise-form/exercise-form.component').then(c => c.ExerciseFormComponent)
  },
  {
    path: 'exercices/:id',
    loadComponent: () => import('./exercices/exercise-detail/exercise-detail.component').then(c => c.ExerciseDetailComponent)
  },
  {
    path: 'exercices/:id/hints',
    loadComponent: () => import('./exercices/hint-management/hint-management.component').then(c => c.HintManagementComponent)
  },
  {
    path: 'exercices/:id/steps',
    loadComponent: () => import('./exercices/step-management/step-management.component').then(c => c.StepManagementComponent)
  },

 // Suivi des utilisateurs
  {
    path: 'exercices/user/:userId/revision',
    loadComponent: () => import('./exercices/user-exercise-revision/user-exercise-revision.component').then(c => c.UserExerciseRevisionComponent)
  },
  {
    path: 'exercices/user/:userId/consultations',
    loadComponent: () => import('./exercices/user-exercise-consultations/user-exercise-consultations.component').then(c => c.UserExerciseConsultationsComponent)
  },
 
  {
    path: 'user-tracking',
    loadComponent: () => import('./user-tracking/stats-global/stats-global.component').then(c => c.StatsGlobalComponent)
  },
  {
    path: 'user-tracking/progression',
    loadComponent: () => import('./user-tracking/progression-global/progression-global.component').then(c => c.ProgressionGlobalComponent)
  },
  {
    path: 'user-tracking/user/:id/stats',
    loadComponent: () => import('./user-tracking/user-stats-detail/user-stats-detail.component').then(c => c.UserStatsDetailComponent)
  },
  {
    path: 'user-tracking/user/:id/progression',
    loadComponent: () => import('./user-tracking/user-progression-detail/user-progression-detail.component').then(c => c.UserProgressionDetailComponent)
  },
  {
    path: 'user-tracking/user/:id/quiz-attempts',
    loadComponent: () => import('./user-tracking/user-quiz-attempts/user-quiz-attempts.component').then(c => c.UserQuizAttemptsComponent)
  },
  {
    path: 'user-tracking/user/:id/qa-consultations',
    loadComponent: () => import('./user-tracking/user-qa-consultations/user-qa-consultations.component').then(c => c.UserQaConsultationsComponent)
  },

  // Autres
  {
    path: 'settings',
    loadComponent: () => import('./settings/settings-general/settings-general.component').then(c => c.SettingsGeneralComponent)
  },
  {
    path: 'my-space',
    loadComponent: () => import('./my-space/my-space-overview/my-space-overview.component').then(c => c.MySpaceOverviewComponent)
  },

  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];