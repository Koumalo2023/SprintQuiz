import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  QAQuestionDto, 
  CreateQAQuestionDto, 
  UpdateQAQuestionDto, 
} from '../models/qa-question.model'; 
import { environment } from '../../../environments/environment';
import { NiveauEnum } from '../models/enums.models';
import { ConsultationQADto, CreateConsultationQADto } from '../models/profil.model';


//src/app/core/services/qa.service.ts
@Injectable({
  providedIn: 'root'
})
export class QaService {
  private readonly API_URL = `${environment.apiUrl}/qaquestion`;

  constructor(private http: HttpClient) {}

  getAllQAQuestions(): Observable<QAQuestionDto[]> {
    return this.http.get<QAQuestionDto[]>(this.API_URL);
  }

  getQAQuestionById(id: string): Observable<QAQuestionDto> {
    return this.http.get<QAQuestionDto>(`${this.API_URL}/${id}`);
  }

  getQAQuestionsByNiveau(niveau: NiveauEnum, niveauId: string): Observable<QAQuestionDto[]> {
    return this.http.get<QAQuestionDto[]>(`${this.API_URL}/niveau/${niveau}/${niveauId}`);
  }

  getMyQAQuestionsForRevision(niveau: NiveauEnum, niveauId: string): Observable<QAQuestionDto[]> {
    return this.http.get<QAQuestionDto[]>(`${this.API_URL}/ma-revision/niveau/${niveau}/${niveauId}`);
  }

  getQAQuestionsForRevision(userId: string, niveau: NiveauEnum, niveauId: string): Observable<QAQuestionDto[]> {
    return this.http.get<QAQuestionDto[]>(`${this.API_URL}/revision/${userId}/niveau/${niveau}/${niveauId}`);
  }

  createQAQuestion(question: CreateQAQuestionDto): Observable<QAQuestionDto> {
    return this.http.post<QAQuestionDto>(this.API_URL, question);
  }

  updateQAQuestion(id: string, question: UpdateQAQuestionDto): Observable<QAQuestionDto> {
    return this.http.put<QAQuestionDto>(`${this.API_URL}/${id}`, question);
  }

  deleteQAQuestion(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  consultQAQuestion(consultation: CreateConsultationQADto): Observable<ConsultationQADto> {
    return this.http.post<ConsultationQADto>(`${this.API_URL}/consult`, consultation);
  }

  getMyConsultations(): Observable<ConsultationQADto[]> {
    return this.http.get<ConsultationQADto[]>(`${this.API_URL}/mes-consultations`);
  }

  getUserConsultations(userId: string): Observable<ConsultationQADto[]> {
    return this.http.get<ConsultationQADto[]>(`${this.API_URL}/utilisateur/${userId}/consultations`);
  }
}

