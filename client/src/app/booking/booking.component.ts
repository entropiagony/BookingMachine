import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { EmployeeBooking } from 'src/_models/employee-booking';
import { Floor } from 'src/_models/floor';
import { Manager } from 'src/_models/manager';
import { BookingService } from 'src/_services/booking.service';
import { FloorService } from 'src/_services/floor.service';

@Component({
  selector: 'app-booking',
  templateUrl: './booking.component.html',
  styleUrls: ['./booking.component.css']
})
export class BookingComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  bookingForm!: FormGroup;
  validationErrors: string[] = [];
  floors: Floor[] = [];
  selectedFloor!: Floor;
  bookings: EmployeeBooking[] = [];
  noBookings = false;

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
    this.bookingService.getEmployeeBookings().subscribe(bookings => {
      this.bookings = bookings;
      this.noBookings = bookings.length == 0;
    })
  }

  changeFloor(e: any) {
    let id = this.bookingForm.controls['floorId'].value;
    this.selectedFloor = this.floors.find(x => x.id == id)!;
  }

  listenToNewBookings() {
    this.bookingService.employeeBookings$.subscribe(bookings => {
      if (bookings[bookings.length - 1])
        this.bookings.push(bookings[bookings.length - 1]);
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

}
