import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { WorkPlace } from 'src/_models/workplace';

@Injectable({
  providedIn: 'root'
})
export class WorkplaceService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createWorkPlaces(quantity: number, floorId: number) {
    return this.http.post<WorkPlace[]>(this.baseUrl + `workplaces?quantity=${quantity}&floorId=${floorId}`, {});
  }

  deleteWorkPlace(id: number) {
    return this.http.delete(this.baseUrl + `WorkPlaces?id=${id}`);
  }
}
