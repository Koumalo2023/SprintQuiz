
import { CoursDto } from "./cours.model";
import { ModuleDto } from "./module.model";
import { ProgressionUtilisateurDto } from "./profil.model";
import { QAQuestionDto } from "./qa-question.model";
import { QuizDto } from "./quiz.model";
//src/app/core/models/sprint.model.ts
export interface SprintDto {
  id: string;
  nom: string;
  description: string;
  ordre?: number; 
  modules?: ModuleDto[];
  cours: CoursDto[];
  quizzes: QuizDto[];
  qaQuestions: QAQuestionDto[];
  progressions:ProgressionUtilisateurDto[]
}

export interface CreateSprintDto {
  nom: string;
  description: string;
  ordre: number;
}

export interface UpdateSprintDto {
  nom?: string;
  description?: string;
  ordre?: number;
}


