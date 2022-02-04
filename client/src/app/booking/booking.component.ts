import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
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

  constructor(private floorService: FloorService, private toastr: ToastrService,
    private fb: FormBuilder, private bookingService: BookingService) { }

  ngOnInit(): void {
    this.getFloors();
    this.getBookings();
    this.initializeForm();
  }

  initializeForm() {
    this.bookingForm = this.fb.group({
      floorId: ['', Validators.required],
      workPlaceId: ['', Validators.required],
      bookingDate: ['', Validators.required]
    })

    this.bookingForm.controls['password'].valueChanges.subscribe(() => {
      this.bookingForm.controls['confirmPassword'].updateValueAndValidity();
    })
  }


  book() {
    this.bookingService.createBooking(this.bookingForm.value).subscribe(response => {
      this.toastr.success("Booking successfully created!");
      console.log(response);
      this.bookings.push(response);
    }, error => {
      this.validationErrors = error;
    }
    );
  }

  getFloors() {
    this.floorService.getFloors().subscribe(floors => {
      this.floors = floors;
    })
  }

  getBookings() {
    this.bookingService.getEmployeeBookings().subscribe(bookings => {
      this.bookings = bookings;
    })
  }

  changeFloor(e: any) {
    let id = this.bookingForm.controls['floorId'].value;
    this.selectedFloor = this.floors.find(x => x.id == id)!;
  }

 
}
