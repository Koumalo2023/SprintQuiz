
import { CoursDto } from "./cours.model";
import { NiveauPedagogiqueDto } from "./niveauPedagogique.model";
import { ModuleDto } from "./module.model";
import { ProgressionUtilisateurDto } from "./profil.model";
import { QAQuestionDto } from "./qa-question.model";
import { QuizDto } from "./quiz.model";

// src/app/core/models/sprint.model.ts
export interface SprintDto extends NiveauPedagogiqueDto {
  formationId: string | null;
  formationNom: string | null;
  modules: ModuleDto[] | null;
  // Facultatif : agrégation pour cohérence frontend
  cours: CoursDto[] | null;
  quizzes: QuizDto[] | null;
  qaQuestions: QAQuestionDto[] | null;
  progressions: ProgressionUtilisateurDto[] | null;
}

export interface CreateSprintDto {
  nom: string;
  description?: string;
  ordre: number;

  estActif?: boolean;
  dateOuverture?: string | null;
  objectifs?: string | null;
  resume?: string | null;
  notionsCles?: string | null;
  tags?: string[];

  formationId?: string | null;
}

export interface UpdateSprintDto {
  nom?: string;
  description?: string;
  ordre?: number;

  estActif?: boolean;
  dateOuverture?: string | null;
  objectifs?: string | null;
  resume?: string | null;
  notionsCles?: string | null;
  tags?: string[];

  formationId?: string | null;
}


