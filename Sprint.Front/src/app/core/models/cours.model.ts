import { NiveauPedagogiqueDto } from "./niveauPedagogique.model";
import { ProgressionUtilisateurDto } from "./profil.model";
import { QAQuestionDto } from "./qa-question.model";
import { QuizDto } from "./quiz.model";

// src/app/core/models/cours.model.ts
 

export interface CoursDto extends NiveauPedagogiqueDto {
  moduleId: string;
  moduleNom: string | null;
  quizzes: QuizDto[] | null;
  qaQuestions: QAQuestionDto[] | null;
  progressions: ProgressionUtilisateurDto[] | null;
}

export interface CreateCoursDto {
  nom: string;  // Changé de 'titre' à 'nom' pour cohérence
  description?: string;
  ordre: number;

  estActif?: boolean;
  dateOuverture?: string | null;
  objectifs?: string | null;
  resume?: string | null;
  notionsCles?: string | null;
  tags?: string[];

  moduleId: string;
}

export interface UpdateCoursDto {
  nom?: string;  // Changé de 'titre' à 'nom'
  description?: string;
  ordre?: number;

  estActif?: boolean;
  dateOuverture?: string | null;
  objectifs?: string | null;
  resume?: string | null;
  notionsCles?: string | null;
  tags?: string[];

  moduleId?: string;
}
