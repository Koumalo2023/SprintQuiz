import { NiveauEnum, NiveauDifficulte, TypeExercice } from './enums.models';

// --- DTOs ---
export interface ExerciceDto {
  id: string;
  enonce: string;
  solution: string;
  solutionResume?: string;
  niveau: NiveauEnum;
  niveauId: string;
  niveauDifficulte: NiveauDifficulte;
  type: TypeExercice;
  tags: string[] | null;
  dateCreation: string; // ISO date

  indices: IndiceDto[] | null;
  etapesResolution: EtapeResolutionDto[] | null;
}

export interface CreateExerciceDto {
  enonce: string;
  solution: string;
  solutionResume?: string;
  niveau: NiveauEnum;
  niveauId: string;
  niveauDifficulte: NiveauDifficulte;
  type: TypeExercice;
  tags: string[] | null;
  indices: CreateIndiceDto[] | null;
  etapesResolution: CreateEtapeResolutionDto[] | null;
}

export interface UpdateExerciceDto {
  enonce?: string;
  solution?: string;
  solutionResume?: string;
  niveau?: NiveauEnum;
  niveauId?: string;
  niveauDifficulte?: NiveauDifficulte;
  type?: TypeExercice;
  tags?: string[] | null;
}

// --- Consultation ---
export interface ConsultationExerciceDto {
  id: string;
  utilisateurId: string;
  exerciceId: string;
  dateConsultation: string;
  marqueeCompris: boolean | null;
  aConsulteSolution: boolean;
  aUtiliseIndices: boolean;
  tentativesAvantSolution: number;
}

export interface CreateConsultationExerciceDto {
  exerciceId: string;
  marqueeCompris?: boolean | null;
}

// --- Indice ---
export interface IndiceDto {
  id: string;
  ordre: number;
  texte: string;
}

export interface CreateIndiceDto {
  ordre: number;
  texte: string;
}

export interface UpdateIndiceDto {
  ordre?: number;
  texte?: string;
}

// --- Étape de Résolution ---
export interface EtapeResolutionDto {
  id: string;
  ordre: number;
  description: string;
}

export interface CreateEtapeResolutionDto {
  ordre: number;
  description: string;
}

export interface UpdateEtapeResolutionDto {
  ordre?: number;
  description?: string;
}