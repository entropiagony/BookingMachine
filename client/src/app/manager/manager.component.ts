import { Component, Input, OnInit } from '@angular/core';

import { ManagerBooking } from 'src/_models/manager-booking';
import { Pagination } from 'src/_models/pagination';
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
  pageNumber = 1;
  pageSize = 10;
  pagination!: Pagination;

  constructor(private bookingService: BookingService) { }

  ngOnInit(): void {
    this.getBookings();
    this.listenToNewBookings();
  }

  getBookings() {
    this.bookingService.getManagerBookings(this.pageNumber, this.pageSize).subscribe(bookings => {
      this.bookings = bookings.result;
      this.pagination = bookings.pagination;
      if (bookings.result.length == 0)
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
      if (bookings[bookings.length - 1]) {
        this.pagination.totalItems++;
        this.bookings.unshift(bookings[bookings.length - 1]);
        this.noBookings ? false : true;
      }
      if (this.bookings?.length > this.pageSize)
        this.bookings.pop();
    })
  }

  pageChanges(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.getBookings();
    }
  }

}
