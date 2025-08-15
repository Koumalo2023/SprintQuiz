
import { RoleUtilisateur } from "./enums.models";
import { ConsultationQADto, ProgressionUtilisateurDto, StatistiquesGlobales } from "./profil.model";
import { TentativeQuizDto } from "./quiz.model";

//src/app/core/models/auth.model.ts
export interface UtilisateurDto {
    id: string;
    nom: string;
    email: string;
    photoUrl?: string;
    role: RoleUtilisateur;
    dateInscription: Date;
    tentativesQuiz : TentativeQuizDto[];
    consultationsQA : ConsultationQADto[];
    progressions : ProgressionUtilisateurDto[];
    statistiquesGlobales: StatistiquesGlobales[]
}

export interface CreateUtilisateurDto {
    nom: string;
    email: string;
    motDePasse: string;
    role: RoleUtilisateur;
}

export interface UpdateUtilisateurDto {
    nom?: string;
    email?: string;
    photoUrl?: string;
    role?: RoleUtilisateur;
}

// Authentication Interfaces
export interface LoginDto {
    email: string;
    motDePasse: string;
}

export interface AuthResponseDto {
    token: string;
    utilisateur: UtilisateurDto;
    expiresAt: Date;
}

// Password Management
export interface ChangePasswordDto {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
}

