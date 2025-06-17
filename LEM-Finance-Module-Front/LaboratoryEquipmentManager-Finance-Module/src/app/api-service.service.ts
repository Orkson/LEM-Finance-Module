import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiServiceService {
  private apiUrl = environment.apiUrl + '/';

  constructor(private http: HttpClient) { }

  getDevices(query: PagedAndSortedQueryOfDevicesList): Observable<any[]> {
    let params = new HttpParams()

    if (query.SearchTerm) {
      params = params.set('searchTerm', query.SearchTerm);
    }

    params = params
    .set('sortColumn', query.SortColumn)
    .set('sortOrder', query.SortOrder)
    .set('page', query.Page?.toString())
    .set('pageSize', query.PageSize?.toString());

    return this.http.get<any[]>(this.apiUrl + 'devices', { params }).pipe(
      map((response: any) => response)
    );
  }

  getDeviceDetailsById(deviceId: number): Observable<Object> {
    return this.http.get(this.apiUrl + 'devices/' + deviceId);
  }

  getDevicesByModelName(searchPhrase: SearchPhraseDto): Observable<Object> {
    return this.http.post(this.apiUrl + 'devices/search', searchPhrase);
  }

  getModelDetails(modelId: number, modelName: string): Observable<Object> {
    let params = new HttpParams()
      .set('modelId', modelId)
      .set('modelName', modelName);

    return this.http.get(this.apiUrl + 'models', {params});
  }

  //createDevice(addDeviceDto: any): Observable<any> {
  //  return this.http.post(this.apiUrl + 'devices', addDeviceDto).pipe((response: any) => response);
  //}
  createDevice(formData: FormData): Observable<any> {
  return this.http.post<any>(this.apiUrl + 'devices', formData);
  }

  editDevice(deviceId: number, oldDeviceDto: AddDeviceDto, newDeviceDto: AddDeviceDto, modelCooperationsToBeRemoved: number[] | null): Observable<any> {
    const url = `${this.apiUrl+'devices'}/${deviceId}`;
    const body = {oldDevice: oldDeviceDto, newDevice: newDeviceDto, modelCooperationsToBeRemoved: modelCooperationsToBeRemoved,};
    return this.http.put(url, body).pipe((response: any) => response);
  }

  removeDevice(deviceId: number): Observable<any> {
    const url = `${this.apiUrl+'devices'}/${deviceId}`;
    return this.http.delete(url);
  }

  addDocuments(formData: FormData): Observable<any> {
    return this.http.post(this.apiUrl + 'files', formData);
  }

  removeDocuments(documentsId: number[]): Observable<any> {
    return this.http.request<any>('delete', this.apiUrl + 'files', {
      body: documentsId
    });
  }

  downloadFile(documentName: string, modelId?: string, deviceId?: string): Observable<any>{
    let params = new HttpParams().set('documentName', documentName);

    if (modelId !== undefined) {
      params = params.set('modelId', modelId);
    }

    if (deviceId !== undefined) {
      params = params.set('deviceId', deviceId);
    }

    return this.http.get(this.apiUrl + 'files', {
      responseType: 'blob',
      params,
    });
  }

  getExpensesByYear(year: number): Observable<any[]> {
    console.log('Api URL getExpensesByYear', this.apiUrl);
    return this.http.get<any[]>(this.apiUrl + `expenses-planner/` + year);
  }

  createExpense(expense: any): Observable<any> {
    return this.http.post<any>(this.apiUrl + `expenses-planner/`, expense);
  }

  updateExpense(id: number, expense: any): Observable<void> {
    console.log('updateExpense', this.apiUrl);
    return this.http.put<void>(`${this.apiUrl}expenses-planner/${id}`, expense);
  }
  
  deleteExpense(id: number): Observable<void> {
    console.log('deleteExpense', this.apiUrl);
    return this.http.delete<void>(`${this.apiUrl}expenses-planner/${id}`);
  }

  getServices(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}services`);
  }

  getDevice(): Observable<any> {
    return this.http.get<any>(this.apiUrl + 'devices');
  }

  getExchangeRate(currencyCode: string): Observable<{ currency: string, rate: number }> {
    return this.http.get<{ currency: string, rate: number }>(`${this.apiUrl}currency/${currencyCode}`);
  }
}

export class AddDeviceDto {
   identificationNumber: string;
   productionDate?: string;
   calibrationPeriodInYears?: number;
   lastCalibrationDate?: string;
   nextCalibrationDate?: string;
   isCalibrated?: boolean;
   isCalibrationCloseToExpire?: boolean;
   storageLocation?: string;
   model: string;
   serialNumber: string;
   company: { name: string } = { name: '' };
   cooperatedModelsIds?: number[];
}

export class DeviceDto {
  Id: number;
  DeviceIdentificationNumber: number;
  ModelName: string;
  ModelSerialNumber: string;
  MeasuredValues: MeasuredValueDto[];
  StorageLocation?: string;
  ProductionDate?: Date;
  LastCalibrationDate?: Date;
  CalibrationPeriodInYears?: number;
  IsCloseToExpire?: boolean;
}

export class MeasuredValueDto {
  PhysicalMagnitudeName: string;
  PhysicalMagnitudeUnit?: string;
  MeasuredRanges?: MeasuredRangesDto[];
}

export class MeasuredRangesDto {
  Range: string;
  AccuracyInPercent: number;
}

export class ModelDto {
  Name: string;
  SerialNumber: string;
  CompanyName?: string;
  CooperatedModelsIds?: number[];
  MeasuredValues?: MeasuredValueDto[];
}

export class SearchPhraseDto {
  DeviceModelName?: string;
  MeasuredValueName?: string;
}

export class PagedAndSortedQueryOfDevicesList {
  SearchTerm: string;
  SortColumn: string;
  SortOrder: string;
  Page: number;
  PageSize: number;
}
