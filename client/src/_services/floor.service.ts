import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Floor } from 'src/_models/floor';

@Injectable({
  providedIn: 'root'
})
export class FloorService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getFloors() {
    return this.http.get<Floor[]>(this.baseUrl + 'floors');
  }

  deleteFloor(id: number) {
    return this.http.delete(this.baseUrl + `floors?id=${id}`);
  }

  createFloor(floorNumber: number) {
    return this.http.post<Floor>(this.baseUrl + `floors?floorNumber=${floorNumber}`, {});
  }
}
