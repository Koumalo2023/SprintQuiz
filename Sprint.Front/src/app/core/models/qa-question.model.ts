
import { NiveauDifficulte, NiveauEnum } from './enums.models';
import { ConsultationQADto } from './profil.model';
//src/app/core/models/qa-question.model.ts
export interface QAQuestionDto {
    id: string;
    question: string;
    reponse: string;
    niveau: NiveauEnum;
    niveauId: string;
    niveauDifficulte: NiveauDifficulte;
    tags?: string[];
    dateCreation: Date;
    consultations : ConsultationQADto[]
}

export interface CreateQAQuestionDto {
    question: string;
    reponse: string;
    niveau: NiveauEnum;
    niveauId: string;
    niveauDifficulte: NiveauDifficulte;
    tags?: string[];
}

export interface UpdateQAQuestionDto {
    question?: string;
    reponse?: string;
    niveau?: NiveauEnum;
    niveauId?: string;
    niveauDifficulte?: NiveauDifficulte;
    tags?: string[];
}
