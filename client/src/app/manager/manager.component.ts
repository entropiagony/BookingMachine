import { Component, Input, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { ManagerBooking } from 'src/_models/manager-booking';
import { BookingService } from 'src/_services/booking.service';

@Component({
  selector: 'app-manager',
  templateUrl: './manager.component.html',
  styleUrls: ['./manager.component.css']
})
export class ManagerComponent implements OnInit {
  bookings!: ManagerBooking[];
  noBookings = false;
  @Input()
  reason!: string;

  constructor(private bookingService: BookingService) { }

  ngOnInit(): void {
    this.getPendingBookings();
    this.listenToNewBookings();
  }

  getPendingBookings() {
    this.bookingService.getManagerBookings().subscribe(bookings => {
      this.bookings = bookings;
      if (bookings.length == 0)
        this.noBookings = true;
    })
  }

  approveBooking(bookingId: number) {
    this.bookingService.approveBooking(bookingId).then(() => {
      this.bookings = this.bookings.filter(x => x.id != bookingId);
      if (this.bookings.length == 0)
        this.noBookings = true;
    })
  }

  declineBooking(bookingId: number) {
    this.bookingService.declineBooking(bookingId, this.reason).then(() => {
      this.bookings = this.bookings.filter(x => x.id != bookingId);
      if (this.bookings.length == 0)
        this.noBookings = true;
    })
  }

  listenToNewBookings() {
    this.bookingService.managerBookings$.subscribe(bookings => {
      if (bookings[bookings.length - 1])
        this.bookings.push(bookings[bookings.length - 1]);
      if (this.noBookings)
        this.noBookings = false;
    })
  }

}
