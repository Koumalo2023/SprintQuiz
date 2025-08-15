import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  StatistiquesGlobales, 
  ProgressionUtilisateurDto 
} from '../models/profil.model';
import { environment } from '../../../environments/environment';
import { ChangePasswordDto, UpdateUtilisateurDto, UtilisateurDto } from '../models/auth.model';


//src/app/core/services/profil.service.ts
@Injectable({
  providedIn: 'root'
})
export class ProfilService {
  private readonly API_URL = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // GET /api/profil
  getProfil(): Observable<UtilisateurDto> {
    return this.http.get<UtilisateurDto>(`${this.API_URL}/profil`);
  }

  // PUT /api/profil
  updateProfil(data: UpdateUtilisateurDto): Observable<UtilisateurDto> {
    return this.http.put<UtilisateurDto>(`${this.API_URL}/profil`, data);
  }

  // POST /api/profil/photo
  uploadPhoto(image: string): Observable<{ photoUrl: string }> {
    return this.http.post<{ photoUrl: string }>(`${this.API_URL}/profil/photo`, { image });
  }

  // PUT /api/profil/password
  changePassword(dto: ChangePasswordDto): Observable<void> {
    return this.http.put<void>(`${this.API_URL}/profil/password`, dto);
  }

  // GET /api/utilisateur/mes-statistiques
  getMyStatistics(): Observable<StatistiquesGlobales> {
    return this.http.get<StatistiquesGlobales>(`${this.API_URL}/utilisateur/mes-statistiques`);
  }

  // GET /api/utilisateur/ma-progression
  getMyProgression(): Observable<ProgressionUtilisateurDto[]> {
    return this.http.get<ProgressionUtilisateurDto[]>(`${this.API_URL}/utilisateur/ma-progression`);
  }

  // GET /api/utilisateur/{id}/statistiques
  getUtilisateurDtoStatistics(UtilisateurDtoId: string): Observable<StatistiquesGlobales> {
    return this.http.get<StatistiquesGlobales>(`${this.API_URL}/utilisateur/${UtilisateurDtoId}/statistiques`);
  }

  // GET /api/utilisateur/{id}/progression
  getUtilisateurDtoProgression(UtilisateurDtoId: string): Observable<ProgressionUtilisateurDto[]> {
    return this.http.get<ProgressionUtilisateurDto[]>(`${this.API_URL}/utilisateur/${UtilisateurDtoId}/progression`);
  }
}

