import { Component, OnInit } from '@angular/core';
import { AdminBooking } from 'src/_models/admin-booking';
import { BookingService } from 'src/_services/booking.service';

@Component({
  selector: 'app-booking-monitoring',
  templateUrl: './booking-monitoring.component.html',
  styleUrls: ['./booking-monitoring.component.css']
})
export class BookingMonitoringComponent implements OnInit {
  bookings: AdminBooking[] = [];
  liveBookings: AdminBooking[] = [];

  constructor(private bookingService: BookingService) { }

  ngOnInit(): void {
    this.getAllBookings();
    this.listenToApprovedBookings();
    this.listenToDeclinedBookings();
    this.listenToNewBookings();
  }

  getAllBookings() {
    this.bookingService.getAdminBookings().subscribe(bookings => {
      this.bookings = bookings;
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
}
