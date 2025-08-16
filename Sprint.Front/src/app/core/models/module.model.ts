import { CoursDto } from "./cours.model";
import { NiveauPedagogiqueDto } from "./niveauPedagogique.model";
import { ProgressionUtilisateurDto } from "./profil.model";
import { QAQuestionDto } from "./qa-question.model";
import { QuizDto } from "./quiz.model";

// src/app/core/models/module.model.ts

export interface ModuleDto extends NiveauPedagogiqueDto {
  sprintId: string;
  sprintNom: string | null;
  cours: CoursDto[] | null;
  quizzes: QuizDto[] | null;
  qaQuestions: QAQuestionDto[] | null;
  progressions: ProgressionUtilisateurDto[] | null;
}

export interface CreateModuleDto {
  nom: string;
  description?: string;
  ordre: number;

  estActif?: boolean;
  dateOuverture?: string | null;
  objectifs?: string | null;
  resume?: string | null;
  notionsCles?: string | null;
  tags?: string[];

  sprintId: string;
}

export interface UpdateModuleDto {
  nom?: string;
  description?: string;
  ordre?: number;

  estActif?: boolean;
  dateOuverture?: string | null;
  objectifs?: string | null;
  resume?: string | null;
  notionsCles?: string | null;
  tags?: string[];

  sprintId?: string;
}