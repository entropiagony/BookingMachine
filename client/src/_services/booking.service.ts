import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { BehaviorSubject, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AdminBooking } from 'src/_models/admin-booking';
import { EmployeeBooking } from 'src/_models/employee-booking';
import { ManagerBooking } from 'src/_models/manager-booking';
import { User } from 'src/_models/user';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private exceptionMessageRegularExpression = /(?<=HubException: ).+/;
  private hubConnection!: HubConnection;
  private employeeBookingsSource = new BehaviorSubject<EmployeeBooking[]>([]);
  private managerBookingsSource = new BehaviorSubject<ManagerBooking[]>([]);
  private adminBookingsSource = new BehaviorSubject<AdminBooking[]>([]);
  private approvedBookingIdsSource = new BehaviorSubject<number>(-1);
  private declinedBookingIdsSource = new BehaviorSubject<number>(-1);
  employeeBookings$ = this.employeeBookingsSource.asObservable();
  managerBookings$ = this.managerBookingsSource.asObservable();
  adminBookings$ = this.adminBookingsSource.asObservable();
  declinedBookingIds$ = this.declinedBookingIdsSource.asObservable();
  approvedBookingIds$ = this.approvedBookingIdsSource.asObservable();

  constructor(private http: HttpClient, private toastr: ToastrService, private router: Router) { }

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'booking', {
      accessTokenFactory: () => user.token
    }).withAutomaticReconnect().build();

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on("EmployeeNewBooking", (booking: EmployeeBooking) => {
      this.employeeBookings$.pipe(take(1)).subscribe(bookings => {
        this.employeeBookingsSource.next([...bookings, booking]);
      })
      this.toastr.success("You successfully created a new booking!").
        onTap.pipe(take(1)).subscribe(() => this.router.navigateByUrl('/book'));
    })

    this.hubConnection.on("AdminNewBooking", (booking: AdminBooking) => {
      this.adminBookings$.pipe(take(1)).subscribe(bookings => {
        this.adminBookingsSource.next([...bookings, booking]);
      })
      this.toastr.info(`${booking.employeeFirstName} ${booking.employeeLastName} has created a new booking.`).
        onTap.pipe(take(1)).subscribe(() => this.router.navigateByUrl('/admin'));
    })

    this.hubConnection.on("ManagerNewBooking", (booking: ManagerBooking) => {
      this.managerBookings$.pipe(take(1)).subscribe(bookings => {
        this.managerBookingsSource.next([...bookings, booking]);
      })
      this.toastr.info(`${booking.employeeFirstName} ${booking.employeeLastName} has created a new booking.`).
        onTap.pipe(take(1)).subscribe(() => this.router.navigateByUrl('/manage'));
    })

    this.hubConnection.on("AdminBookingApproved", (response) => {
      this.approvedBookingIds$.pipe(take(1)).subscribe(() => {
        this.approvedBookingIdsSource.next(response.bookingId);
      })
      this.toastr.success(`Booking Id ${response.bookingId} of employee with Id ${response.employeeId} got approved`);
    })

    this.hubConnection.on("AdminBookingDeclined", (response) => {
      this.declinedBookingIds$.pipe(take(1)).subscribe(() => {
        this.declinedBookingIdsSource.next(response.bookingId);
      })
      this.toastr.warning(`Booking Id ${response.bookingId} of employee with Id ${response.employeeId} got declined`);
    })

    this.hubConnection.on("EmployeeBookingApproved", (bookingId: number) => {
      this.approvedBookingIds$.pipe(take(1)).subscribe(() => {
        this.approvedBookingIdsSource.next(bookingId);
      })
      this.toastr.success(`Your booking with Id ${bookingId} got approved!`);
    })

    this.hubConnection.on("EmployeeBookingDeclined", (response) => {
      this.declinedBookingIds$.pipe(take(1)).subscribe(() => {
        this.declinedBookingIdsSource.next(response.bookingId);
      })
      this.toastr.warning(`Your booking with Id ${response.bookingId} got declined! Reason: ${response.reason}`);
    })


  }

  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  async createBooking(bookingModel: any) {
    return this.hubConnection.invoke("CreateBooking", bookingModel).catch(error => this.displayError(error));
  }

  async approveBooking(bookingId: number) {
    return this.hubConnection.invoke("ApproveBooking", bookingId).catch(error => this.displayError(error));
  }

  async declineBooking(bookingId: number, reason: string) {
    return this.hubConnection.invoke("DeclineBooking", bookingId, reason).catch(error => this.displayError(error));
  }

  getEmployeeBookings() {
    return this.http.get<EmployeeBooking[]>(this.baseUrl + "booking/employee");
  }

  getManagerBookings() {
    return this.http.get<ManagerBooking[]>(this.baseUrl + "booking/manager");
  }

  getAdminBookings() {
    return this.http.get<AdminBooking[]>(this.baseUrl + "booking/admin");
  }

  private displayError(error: any) {
    let serverError = error.toString();
    let message = serverError.match(this.exceptionMessageRegularExpression);
    this.toastr.error(message);
  }
}
