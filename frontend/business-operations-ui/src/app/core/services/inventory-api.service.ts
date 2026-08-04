import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '../config/api-config';
import { ApiResponse } from '../models/api-response.model';
import { PagedResponse } from '../models/paged-response.model';
import { InventoryMovementType } from '../enums/inventory-movement-type.enum';
import { CreateInventoryAdjustmentRequest } from '../../features/inventory/models/create-inventory-adjustment-request.model';
import { InventoryAdjustmentResponse } from '../../features/inventory/models/inventory-adjustment-response.model';
import { InventoryMovementResponse } from '../../features/inventory/models/inventory-movement-response.model';
import { StockLevelResponse } from '../../features/inventory/models/stock-level-response.model';

@Injectable({
  providedIn: 'root'
})
export class InventoryApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}/inventory`;

  constructor(private http: HttpClient) {}

  createAdjustment(request: CreateInventoryAdjustmentRequest): Observable<ApiResponse<InventoryAdjustmentResponse>> {
    return this.http.post<ApiResponse<InventoryAdjustmentResponse>>(`${this.baseUrl}/adjustments`, request);
  }

  getAdjustments(productId?: string, pageNumber: number = 1, pageSize: number = 10): Observable<PagedResponse<InventoryAdjustmentResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (productId) params = params.set('productId', productId);

    return this.http.get<PagedResponse<InventoryAdjustmentResponse>>(`${this.baseUrl}/adjustments`, { params });
  }

  getMovements(
    productId?: string,
    movementType?: InventoryMovementType,
    pageNumber: number = 1,
    pageSize: number = 10
  ): Observable<PagedResponse<InventoryMovementResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (productId) params = params.set('productId', productId);
    if (movementType) params = params.set('movementType', movementType);

    return this.http.get<PagedResponse<InventoryMovementResponse>>(`${this.baseUrl}/movements`, { params });
  }

  getStockLevels(categoryId?: string): Observable<ApiResponse<StockLevelResponse[]>> {
    let params = new HttpParams();
    if (categoryId) params = params.set('categoryId', categoryId);

    return this.http.get<ApiResponse<StockLevelResponse[]>>(`${this.baseUrl}/stock-levels`, { params });
  }

  getLowStockAlerts(): Observable<ApiResponse<StockLevelResponse[]>> {
    return this.http.get<ApiResponse<StockLevelResponse[]>>(`${this.baseUrl}/low-stock-alerts`);
  }
}
