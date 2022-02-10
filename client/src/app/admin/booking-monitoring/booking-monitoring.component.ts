import { Component, OnInit } from '@angular/core';
import { AdminBooking } from 'src/_models/admin-booking';
import { Pagination } from 'src/_models/pagination';
import { BookingService } from 'src/_services/booking.service';

@Component({
  selector: 'app-booking-monitoring',
  templateUrl: './booking-monitoring.component.html',
  styleUrls: ['./booking-monitoring.component.css']
})
export class BookingMonitoringComponent implements OnInit {
  bookings: AdminBooking[] = [];
  liveBookings: AdminBooking[] = [];
  pageNumber = 1;
  pageSize = 10;
  pagination!: Pagination;

  constructor(private bookingService: BookingService) { }

  ngOnInit(): void {
    this.getBookings();
    this.listenToApprovedBookings();
    this.listenToDeclinedBookings();
    this.listenToNewBookings();
  }

  getBookings() {
    this.bookingService.getAdminBookings(this.pageNumber, this.pageSize).subscribe(bookings => {
      this.bookings = bookings.result;
      this.pagination = bookings.pagination;
    })
  }

  listenToApprovedBookings() {
    this.bookingService.approvedBookingIds$.subscribe(id => {
      this.bookings.forEach(element => {
        if (element.id == id)
          element.status = "Approved";
      })
      this.liveBookings.forEach(element => {
        if (element.id == id)
          element.status = "Approved";
      })
    })
  }

  listenToDeclinedBookings() {
    this.bookingService.declinedBookingIds$.subscribe(id => {
      this.bookings.forEach(element => {
        if (element.id == id)
          element.status = "Declined";
      })
      this.liveBookings.forEach(element => {
        if (element.id == id)
          element.status = "Declined";
      })
    })
  }

  listenToNewBookings() {
    this.bookingService.adminBookings$.subscribe(bookings => {
      if (bookings[bookings.length - 1])
        this.liveBookings.push(bookings[bookings.length - 1]);
    });
  }

  pageChanges(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.getBookings();
    }
  }
}
