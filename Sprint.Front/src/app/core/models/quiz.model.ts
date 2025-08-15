

import { NiveauDifficulte, NiveauEnum } from "./enums.models";
//src/app/core/models/quiz.model.ts
// Interfaces pour les Options QCM
export interface QCMOptionDto {
    id: string;
    questionId: string;
    texte: string;
    estCorrecte: boolean;
    reponses: ReponseUtilisateurDto[]
}

export interface CreateQCMOptionDto {
    texte: string;
    estCorrecte: boolean;
}

export interface UpdateQCMOptionDto {
    texte?: string;
    estCorrecte?: boolean;
}

// Interfaces pour les Questions QCM
export interface QCMQuestionDto {
    id: string;
    quizId: string;
    intitule: string;
    niveauDifficulte: NiveauDifficulte;
    explication?: string;
    options: QCMOptionDto[];
    reponses: ReponseUtilisateurDto[]
}

export interface CreateQCMQuestionDto {
    // quizId: string;
    intitule: string;
    niveauDifficulte: NiveauDifficulte;
    explication?: string;
    options: CreateQCMOptionDto[];
}

export interface UpdateQCMQuestionDto {
    intitule?: string;
    niveauDifficulte?: NiveauDifficulte;
    explication?: string;
}


// Interfaces pour les Quiz
export interface QuizDto {
    id: string;
    titre: string;
    description?: string;
    niveau: NiveauEnum;
    niveauId: string | undefined;
    dateCreation: Date; 
    version: number[];
    questions : QCMQuestionDto[];
    tentativesQuiz : TentativeQuizDto[]
}

export interface CreateQuizDto {
    titre: string;
    description?: string;
    niveau: NiveauEnum;
    niveauId: string | undefined;
    version: number[];
    questions: CreateQCMQuestionDto[];
}

export interface UpdateQuizDto {
    titre?: string;
    description?: string;
    niveau?: NiveauEnum;
    niveauId?: string;
    questions?: CreateQCMQuestionDto[];
}

// Interfaces pour les Résultats
export interface ReponseQuestionDto {
    questionId: string;
    questionIntitule: string;
    optionChoisieId: string;
    optionChoisieTexte: string;
    estCorrecte: boolean;
    explication?: string;
}

export interface QuizResultDto {
    quizId: string;
    quizTitre: string;
    score: number;
    reussi: boolean;
    tempsPasse: string;  // TimeSpan converti en string
    date: Date;
    reponses: ReponseQuestionDto[];
}

export interface TentativeQuizDto {
    id: string;
    utilisateurId: string;
    quizId: string;
    quizTitre: string;
    score: number;
    reussi: boolean;
    date: Date;
    tempsPasse: string; // TimeSpan converti en string
    reponses: ReponseUtilisateurDto[];
}

export interface ReponseUtilisateurDto {
    questionId: string;
    optionId: string;
}

export interface CreateTentativeQuizDto {
    quizId: string;
    reponses: ReponseUtilisateurDto[];
    tempsPasse: string; // TimeSpan converti en string
}

// Dans un fichier dédié, ex: create-qcm-option.dto.ts
export interface CreateQCMOptionDto {
  texte: string;
  estCorrecte: boolean;
}

  