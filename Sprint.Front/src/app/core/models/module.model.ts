import { CoursDto } from "./cours.model";
import { ProgressionUtilisateurDto } from "./profil.model";
import { QAQuestionDto } from "./qa-question.model";
import { QuizDto } from "./quiz.model";

//src/app/core/models/module.model.ts
export interface ModuleDto {
  id: string;
  nom: string;
  description: string;
  sprintId: string | null; 
  sprintNom?: string;
  cours?: CoursDto[];
  quizzes: QuizDto[]
  ordre?: number;  
  qaQuestions: QAQuestionDto[]
  progressions:ProgressionUtilisateurDto[]
}

export interface CreateModuleDto {
  nom: string;
  description?: string;
  ordre: number;
  sprintId: string;
}

export interface UpdateModuleDto {
  nom?: string;
  description?: string;
  ordre?: number;
  sprintId?: string;
}

