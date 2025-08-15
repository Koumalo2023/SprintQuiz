import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CoursDto, CreateCoursDto, UpdateCoursDto } from '../models/cours.model';
import { environment } from '../../../environments/environment';

//src/app/core/services/cours.service.ts
@Injectable({
  providedIn: 'root'
})
export class CoursService {
  private readonly API_URL = `${environment.apiUrl}/cours`;

  constructor(private http: HttpClient) {}

  getAllCours(): Observable<CoursDto[]> {
    return this.http.get<CoursDto[]>(this.API_URL);
  }

  getCoursById(id: string): Observable<CoursDto> {
    return this.http.get<CoursDto>(`${this.API_URL}/${id}`);
  }

  getCoursByModuleId(moduleId: string): Observable<CoursDto[]> {
    return this.http.get<CoursDto[]>(`${this.API_URL}/module/${moduleId}`);
  }

  createCours(cours: CreateCoursDto): Observable<CoursDto> {
    return this.http.post<CoursDto>(this.API_URL, cours);
  }

  updateCours(id: string, cours: UpdateCoursDto): Observable<CoursDto> {
    return this.http.put<CoursDto>(`${this.API_URL}/${id}`, cours);
  }

  deleteCours(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }
}

