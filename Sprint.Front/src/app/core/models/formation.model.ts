import { NiveauPedagogiqueDto } from "./niveauPedagogique.model";
import { SprintDto } from "./sprint.model";




export interface FormationDto extends NiveauPedagogiqueDto {
  sprints: SprintDto[] | null;
}

export interface CreateFormationDto {
  nom: string;
  description?: string;
  ordre: number;
  estActif?: boolean;
  dateOuverture?: string | null;
  objectifs?: string | null;
  resume?: string | null;
  notionsCles?: string | null;
  tags?: string[];
}

export interface UpdateFormationDto {
  nom?: string;
  description?: string;
  ordre?: number;
  estActif?: boolean;
  dateOuverture?: string | null;
  objectifs?: string | null;
  resume?: string | null;
  notionsCles?: string | null;
  tags?: string[];
}