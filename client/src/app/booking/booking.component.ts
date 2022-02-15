import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { EmployeeBooking } from 'src/_models/employee-booking';
import { Floor } from 'src/_models/floor';
import { Manager } from 'src/_models/manager';
import { Pagination } from 'src/_models/pagination';
import { BookingService } from 'src/_services/booking.service';
import { FloorService } from 'src/_services/floor.service';

@Component({
  selector: 'app-booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css']
})
export class BookingComponent implements OnInit {
  bookingForm!: FormGroup;
  validationErrors: string[] = [];
  floors: Floor[] = [];
  selectedFloor!: Floor;
  bookings: EmployeeBooking[] = [];
  noBookings = false;
  pageNumber = 1;
  pageSize = 10;
  pagination!: Pagination;

  constructor(private floorService: FloorService, private toastr: ToastrService,
    private fb: FormBuilder, private bookingService: BookingService) { }

  ngOnInit(): void {
    this.getFloors();
    this.getBookings();
    this.listenToNewBookings();
    this.listenToApprovedBookings();
    this.listenToDeclinedBookings();
    this.initializeForm();
  }

  initializeForm() {
    this.bookingForm = this.fb.group({
      floorId: ['', Validators.required],
      workPlaceId: ['', Validators.required],
      bookingDate: ['', Validators.required]
    })
  }


  book() {
    this.bookingService.createBooking(this.bookingForm.value).then(() => {
      if (this.noBookings)
        this.noBookings = false;
    })
  }

  getFloors() {
    this.floorService.getFloors().subscribe(floors => {
      this.floors = floors;
    })
  }

  getBookings() {
    this.bookingService.getEmployeeBookings(this.pageNumber, this.pageSize).subscribe(bookings => {
      this.bookings = bookings.result;
      this.noBookings = bookings.result.length == 0;
      this.pagination = bookings.pagination;
    })
  }

  changeFloor(e: any) {
    let id = this.bookingForm.controls['floorId'].value;
    let date = this.bookingForm.controls['bookingDate'].value
    this.selectedFloor = this.floors.find(x => x.id == id)!;
    this.bookingForm.reset({ floorId: id, workPlaceId: this.selectedFloor.workPlaces[0].id, bookingDate: date });
  }

  listenToNewBookings() {
    this.bookingService.employeeBookings$.subscribe(bookings => {
      if (bookings[bookings.length - 1]) {
        this.bookings.unshift(bookings[bookings.length - 1]);
        this.pagination.totalItems++;
      }
      if (this.bookings.length > this.pageSize)
        this.bookings.pop();
    })

  }

  listenToApprovedBookings() {
    this.bookingService.approvedBookingIds$.subscribe(id => {
      this.bookings.forEach(element => {
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
    })
  }

  pageChanges(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.getBookings();
    }
  }

}
