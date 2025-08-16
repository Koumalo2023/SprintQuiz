import { NiveauDifficulte } from "../services/enum.service";


// models/niveauPedagogique.model.ts
export interface NiveauPedagogiqueDto {
  id: string;
  nom: string;
  description: string | null;
  ordre: number;

  estActif: boolean;
  dateOuverture: string | null; // ISO string
  objectifs: string | null;
  resume: string | null;
  notionsCles: string | null;
  dureeEstimee: number; // en minutes
  difficulteMoyenne: NiveauDifficulte;
  tags: string[];
  nombreQuiz: number;
  nombreFlashcards: number;
  nombreExercices: number;
  dateCreation: string; // ISO string
  derniereModification: string | null; // ISO string
  derniereActivite: string | null; // ISO string
}