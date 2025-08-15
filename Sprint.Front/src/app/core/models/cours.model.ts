import { ProgressionUtilisateurDto } from "./profil.model";
import { QAQuestionDto } from "./qa-question.model";
import { QuizDto } from "./quiz.model";

//src/app/core/models/cours.model.ts
export interface CoursDto {
  id: string;
  titre: string;
  description?: string;
  ordre: number;
  moduleId: string | null ;
  moduleNom?: string;
  quizzes: QuizDto[]
  qaQuestions: QAQuestionDto[]
  progressions: ProgressionUtilisateurDto[]
}

export interface CreateCoursDto {
  titre: string;
  description?: string;
  ordre: number;
  moduleId: string;
}

export interface UpdateCoursDto {
  titre?: string;
  description?: string;
  ordre?: number;
  moduleId?: string;
}

