import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  QuizDto, 
  CreateQuizDto, 
  UpdateQuizDto, 
  TentativeQuizDto, 
  CreateTentativeQuizDto, 
  QuizResultDto,
  QCMQuestionDto,
  CreateQCMQuestionDto,
  UpdateQCMQuestionDto,
  QCMOptionDto,
  CreateQCMOptionDto,
  UpdateQCMOptionDto, 
} from '../models/quiz.model';
import { environment } from '../../../environments/environment';
import { NiveauEnum } from '../models/enums.models';

//src/app/core/services/quiz.service.ts
@Injectable({
  providedIn: 'root'
})
export class QuizService {
  private readonly API_URL = `${environment.apiUrl}/quiz`;

  constructor(private http: HttpClient) {}

  // --- Gestion des Quiz ---
  getAllQuizzes(): Observable<QuizDto[]> {
    return this.http.get<QuizDto[]>(this.API_URL);
  }

  getQuizById(id: string): Observable<QuizDto> {
    return this.http.get<QuizDto>(`${this.API_URL}/${id}`);
  }

  getQuizWithQuestions(id: string): Observable<QuizDto> {
    return this.http.get<QuizDto>(`${this.API_URL}/${id}/questions`);
  }

  getQuizzesByNiveau(niveau: NiveauEnum, niveauId: string): Observable<QuizDto[]> {
    return this.http.get<QuizDto[]>(`${this.API_URL}/niveau/${niveau}/${niveauId}`);
  }

  createQuiz(quiz: CreateQuizDto): Observable<QuizDto> {
    return this.http.post<QuizDto>(this.API_URL, quiz);
  }

  updateQuiz(id: string, quiz: UpdateQuizDto): Observable<QuizDto> {
    return this.http.put<QuizDto>(`${this.API_URL}/${id}`, quiz);
  }

  deleteQuiz(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }

  // --- Soumission des tentatives ---
  submitQuiz(tentative: CreateTentativeQuizDto): Observable<QuizResultDto> {
    return this.http.post<QuizResultDto>(`${this.API_URL}/submit`, tentative);
  }

  getMyQuizAttempts(): Observable<TentativeQuizDto[]> {
    return this.http.get<TentativeQuizDto[]>(`${this.API_URL}/mes-tentatives`);
  }

  getUserQuizAttempts(userId: string): Observable<TentativeQuizDto[]> {
    return this.http.get<TentativeQuizDto[]>(`${this.API_URL}/utilisateur/${userId}/tentatives`);
  }

  // --- Gestion des Questions ---
  getQuestionsByQuizId(quizId: string): Observable<QCMQuestionDto[]> {
    return this.http.get<QCMQuestionDto[]>(`${this.API_URL}/${quizId}/questions`);
  }

  getQuestionById(questionId: string): Observable<QCMQuestionDto> {
    return this.http.get<QCMQuestionDto>(`${this.API_URL}/question/${questionId}`);
  }

  createQuizQuestion(question: CreateQCMQuestionDto): Observable<QCMQuestionDto> {
    return this.http.post<QCMQuestionDto>(`${this.API_URL}/question`, question);
  }

  updateQuizQuestion(questionId: string, question: UpdateQCMQuestionDto): Observable<QCMQuestionDto> {
    return this.http.put<QCMQuestionDto>(`${this.API_URL}/question/${questionId}`, question);
  }

  deleteQuizQuestion(questionId: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/question/${questionId}`);
  }

  // --- Gestion des Options ---
  getOptionsByQuestionId(questionId: string): Observable<QCMOptionDto[]> {
    return this.http.get<QCMOptionDto[]>(`${this.API_URL}/question/${questionId}/options`);
  }

  getOptionById(optionId: string): Observable<QCMOptionDto> {
    return this.http.get<QCMOptionDto>(`${this.API_URL}/option/${optionId}`);
  }

  createOption(option: CreateQCMOptionDto): Observable<QCMOptionDto> {
    return this.http.post<QCMOptionDto>(`${this.API_URL}/option`, option);
  }

  updateOption(optionId: string, option: UpdateQCMOptionDto): Observable<QCMOptionDto> {
    return this.http.put<QCMOptionDto>(`${this.API_URL}/option/${optionId}`, option);
  }

  deleteOption(optionId: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/option/${optionId}`);
  }
}

