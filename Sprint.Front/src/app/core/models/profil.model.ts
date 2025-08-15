
import { NiveauEnum, RoleUtilisateur } from "./enums.models";

//src/app/core/models/profil.model.ts
export interface Profil {
  id: string;
    nom: string;
    email: string;
    photoUrl?: string;
    role: RoleUtilisateur;
    dateInscription: Date;
    statistiquesGlobales?: StatistiquesGlobales;
    progressionGlobale: number;
    dernieresActivites?: DernieresActivitesDto;
}

export interface UpdateProfilDto {
  nom?: string;
  email?: string;
  photoUrl?: string;
}

export interface UploadPhotoDto {
  photoBase64: string;
  fileName: string;
  contentType: string;
}


export interface StatistiquesGlobales {
  id: string;
  utilisateurId: string;
  totalQuizTentes: number;
  totalQuizReussis: number;
  scoreGlobalMoyen: number;
  totalQAConsultees: number;
  totalQAComprises: number;
  tempsEtudeTotal: number;
  derniereActivite: Date;
}


// Interfaces pour les sous-objets
export interface DernieresActivitesDto {
    derniereTentativeQuiz?: Date;
    dernierQuizTitre?: string;
    derniereConsultationQA?: Date;
    derniereQuestionQA?: string;
    derniereActiviteGlobale?: Date;
}


export interface ProgressionUtilisateurDto {
    id: string;
    utilisateurId: string;
    niveau: NiveauEnum;
    niveauId: string;
    niveauNom: string;
    pourcentageComplet: number;
    derniereActivite: Date;
}

export interface StatistiquesGlobalesDto {
    utilisateurId: string;
    totalQuiz: number;
    moyenneScore: number;
    tauxReussite: number;
    tempsTotalRevision: string; // TimeSpan est converti en string car TypeScript n'a pas de type équivalent
    questionsQRConsultees: number;
    coursTermines: number;
    modulesTermines: number;
}

export interface ConsultationQADto {
    id: string;
    utilisateurId: string;
    qaQuestionId: string;
    dateConsultation: Date;
    marqueeComprise?: boolean;
}

export interface CreateConsultationQADto {
    qaQuestionId: string;
    marqueeComprise?: boolean;
}
