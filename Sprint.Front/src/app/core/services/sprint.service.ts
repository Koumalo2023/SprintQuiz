import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SprintDto, CreateSprintDto, UpdateSprintDto } from '../models/sprint.model';
import { environment } from '../../../environments/environment';


//src/app/core/services/sprint.service.ts
@Injectable({
  providedIn: 'root'
})
export class SprintService {
  private readonly API_URL = `${environment.apiUrl}/sprint`;

  constructor(private http: HttpClient) {}

  getAllSprints(): Observable<SprintDto[]> {
    return this.http.get<SprintDto[]>(this.API_URL);
  }

  getSprintById(id: string): Observable<SprintDto> {
    return this.http.get<SprintDto>(`${this.API_URL}/${id}`);
  }

  getSprintWithModules(id: string): Observable<SprintDto> {
    return this.http.get<SprintDto>(`${this.API_URL}/${id}/modules`);
  }

  createSprint(sprint: CreateSprintDto): Observable<SprintDto> {
    return this.http.post<SprintDto>(this.API_URL, sprint);
  }

  updateSprint(id: string, sprint: UpdateSprintDto): Observable<SprintDto> {
    return this.http.put<SprintDto>(`${this.API_URL}/${id}`, sprint);
  }

  deleteSprint(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }
}