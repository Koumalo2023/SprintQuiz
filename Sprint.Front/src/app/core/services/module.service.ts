import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ModuleDto, CreateModuleDto, UpdateModuleDto } from '../models/module.model';
import { environment } from '../../../environments/environment';

//src/app/core/services/module.service.ts
@Injectable({
  providedIn: 'root'
})
export class ModuleService {
  private readonly API_URL = `${environment.apiUrl}/module`;

  constructor(private http: HttpClient) {}

  getAllModules(): Observable<ModuleDto[]> {
    return this.http.get<ModuleDto[]>(this.API_URL);
  }

  getModuleById(id: string): Observable<ModuleDto> {
    return this.http.get<ModuleDto>(`${this.API_URL}/${id}`);
  }

  getModuleWithCours(id: string): Observable<ModuleDto> {
    return this.http.get<ModuleDto>(`${this.API_URL}/${id}/cours`);
  }

  getModulesBySprintId(sprintId: string): Observable<ModuleDto[]> {
    return this.http.get<ModuleDto[]>(`${this.API_URL}/sprint/${sprintId}`);
  }

  createModule(module: CreateModuleDto): Observable<ModuleDto> {
    return this.http.post<ModuleDto>(this.API_URL, module);
  }

  updateModule(id: string, module: UpdateModuleDto): Observable<ModuleDto> {
    return this.http.put<ModuleDto>(`${this.API_URL}/${id}`, module);
  }

  deleteModule(id: string): Observable<void> {
    return this.http.delete<void>(`${this.API_URL}/${id}`);
  }
}
