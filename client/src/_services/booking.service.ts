import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { EmployeeBooking } from 'src/_models/employee-booking';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  createBooking(bookingModel: any) {
    return this.http.post<EmployeeBooking>(this.baseUrl + "booking", bookingModel);
  }

  getEmployeeBookings(){
    return this.http.get<EmployeeBooking[]>(this.baseUrl + "booking/employee");
  }
}
