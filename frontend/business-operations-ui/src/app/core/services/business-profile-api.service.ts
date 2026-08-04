import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { BusinessProfileResponse } from '../../features/business-profile/models/business-profile-response.model';
import { UpdateBusinessProfileRequest } from '../../features/business-profile/models/update-business-profile-request.model';

@Injectable({
  providedIn: 'root'
})
export class BusinessProfileApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/business-profile`;

  constructor(private http: HttpClient) {}

  getProfile(): Observable<ApiResponse<BusinessProfileResponse>> {
    return this.http.get<ApiResponse<BusinessProfileResponse>>(this.baseUrl);
  }

  updateProfile(request: UpdateBusinessProfileRequest): Observable<ApiResponse<BusinessProfileResponse>> {
    return this.http.put<ApiResponse<BusinessProfileResponse>>(this.baseUrl, request);
  }
}
