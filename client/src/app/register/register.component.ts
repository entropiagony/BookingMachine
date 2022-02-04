import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Manager } from 'src/_models/manager';
import { AccountService } from 'src/_services/account-service.service';
import { ManagerService } from 'src/_services/manager.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm!: FormGroup;
  validationErrors: string[] = [];
  managers: Manager[] = [];
  phonePattern = "^((\\+91-?)|0)?[0-9]{10}$";

  constructor(private accountService: AccountService, private toastr: ToastrService,
    private fb: FormBuilder, private router: Router, private managerService: ManagerService) { }

  ngOnInit(): void {
    this.initializeForm();
    this.getManagers();
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      username: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      managerId: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern(this.phonePattern)]],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues("password")]]
    })

    this.registerForm.controls['password'].valueChanges.subscribe(() => {
      this.registerForm.controls['confirmPassword'].updateValueAndValidity();
    })
  }

  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      // @ts-ignore
      return control?.value === control?.parent?.controls[matchTo].value ? null : { isMatching: true } //@ts-ignore

    }
  }

  register() {

    this.accountService.register(this.registerForm.value).subscribe(response => {
      this.router.navigateByUrl('/book');
      this.cancel();
    }, error => {
      this.validationErrors = error;
    }
    );
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

  getManagers() {
    this.managerService.getManagers().subscribe(managers => {
      this.managers = managers;
    })
  }

  changeManager(e: any) {
    this.managerName?.setValue(e.target.value, {
      onlySelf: true
    })
  }

  get managerName() {
    return this.registerForm.get('manager');
  }
}
