import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Report } from 'src/_models/report';
import { getPaginatedResult, getPaginationHeader } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  baseUrl = environment.reportUrl;
  constructor(private http: HttpClient) { }

  getReports(pageNumber: number, pageSize: number, formValue: any) {
    let params = getPaginationHeader(pageNumber, pageSize);
    if (formValue.dateRange) {
      params = params.append("DateFrom", formValue.dateRange[0].toUTCString())
      params = params.append("DateTo", formValue.dateRange[1].toUTCString())
    }
    if (formValue.floorNumber) {
      params = params.append("FloorNumber", formValue.floorNumber)
    }
    return getPaginatedResult<Report[]>(this.baseUrl + "reports", params, this.http);
  }
}
