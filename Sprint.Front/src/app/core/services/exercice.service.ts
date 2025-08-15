import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ConsultationExerciceDto, CreateConsultationExerciceDto, CreateEtapeResolutionDto, CreateExerciceDto, CreateIndiceDto, EtapeResolutionDto, ExerciceDto, IndiceDto, UpdateEtapeResolutionDto, UpdateExerciceDto, UpdateIndiceDto } from '../models/exercice.model';
import { NiveauEnum } from '../models/enums.models';


@Injectable({
  providedIn: 'root'
})
export class ExerciceService {
  private readonly API_URL = `${environment.apiUrl}/exercice`;

  constructor(private http: HttpClient) {}

  // --- Gestion des Exercices ---
  getAllExercices(): Observable<ExerciceDto[]> {
    return this.http.get<ExerciceDto[]>(this.API_URL);
  }

  getExerciceById(id: string): Observable<ExerciceDto> {
    return this.http.get<ExerciceDto>(`${this.API_URL}/${id}`);
  }

  getExercicesByNiveau(niveau: NiveauEnum, niveauId: string): Observable<ExerciceDto[]> {
    return this.http.get<ExerciceDto[]>(`${this.API_URL}/niveau/${niveau}/${niveauId}`);
  }

  getMyExercicesForRevision(niveau: NiveauEnum, niveauId: string): Observable<ExerciceDto[]> {
    return this.http.get<ExerciceDto[]>(`${this.API_URL}/ma-revision/niveau/${niveau}/${niveauId}`);
  }

  getExercicesForRevision(userId: string, niveau: NiveauEnum, niveauId: string): Observable<ExerciceDto[]> {
    return this.http.get<ExerciceDto[]>(`${this.API_URL}/revision/${userId}/niveau/${niveau}/${niveauId}`);
  }

  createExercice(exercice: CreateExerciceDto): Observable<ExerciceDto> {
    return this.http.post<ExerciceDto>(this.API_URL, exercice);
  }

  updateExercice(id: string, exercice: UpdateExerciceDto): Observable<ExerciceDto> {
    return this.http.put<ExerciceDto>(`${this.API_URL}/${id}`, exercice);
  }

  deleteExercice(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  // --- Consultations ---
  consultExercice(consultation: CreateConsultationExerciceDto): Observable<ConsultationExerciceDto> {
    return this.http.post<ConsultationExerciceDto>(`${this.API_URL}/consult`, consultation);
  }

  getMyConsultations(): Observable<ConsultationExerciceDto[]> {
    return this.http.get<ConsultationExerciceDto[]>(`${this.API_URL}/mes-consultations`);
  }

  getUserConsultations(userId: string): Observable<ConsultationExerciceDto[]> {
    return this.http.get<ConsultationExerciceDto[]>(`${this.API_URL}/utilisateur/${userId}/consultations`);
  }

  // --- Gestion des Indices ---
  getIndicesByExerciceId(exerciceId: string): Observable<IndiceDto[]> {
    return this.http.get<IndiceDto[]>(`${this.API_URL}/${exerciceId}/indices`);
  }

  getIndiceById(id: string): Observable<IndiceDto> {
    return this.http.get<IndiceDto>(`${this.API_URL}/indice/${id}`);
  }

  createIndice(indice: CreateIndiceDto): Observable<IndiceDto> {
    return this.http.post<IndiceDto>(`${this.API_URL}/indice`, indice);
  }

  updateIndice(id: string, indice: UpdateIndiceDto): Observable<IndiceDto> {
    return this.http.put<IndiceDto>(`${this.API_URL}/indice/${id}`, indice);
  }

  deleteIndice(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/indice/${id}`);
  }

  // --- Gestion des Étapes de Résolution ---
  getEtapesByExerciceId(exerciceId: string): Observable<EtapeResolutionDto[]> {
    return this.http.get<EtapeResolutionDto[]>(`${this.API_URL}/${exerciceId}/etapes`);
  }

  getEtapeById(id: string): Observable<EtapeResolutionDto> {
    return this.http.get<EtapeResolutionDto>(`${this.API_URL}/etape/${id}`);
  }

  createEtape(etape: CreateEtapeResolutionDto): Observable<EtapeResolutionDto> {
    return this.http.post<EtapeResolutionDto>(`${this.API_URL}/etape`, etape);
  }

  updateEtape(id: string, etape: UpdateEtapeResolutionDto): Observable<EtapeResolutionDto> {
    return this.http.put<EtapeResolutionDto>(`${this.API_URL}/etape/${id}`, etape);
  }

  deleteEtape(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/etape/${id}`);
  }

  // --- Actions utilisateur ---
  markSolutionViewed(exerciceId: string): Observable<any> {
    return this.http.post(`${this.API_URL}/${exerciceId}/consult-solution`, {});
  }

  markIndicesUsed(exerciceId: string): Observable<any> {
    return this.http.post(`${this.API_URL}/${exerciceId}/use-indices`, {});
  }
}