import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Manager } from 'src/_models/manager';

@Injectable({
  providedIn: 'root'
})
export class ManagerService {
  
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getManagers() {
    return this.http.get<Manager[]>(this.baseUrl + 'managers');
  }
}
