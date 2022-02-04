import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, NgForm, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { User } from 'src/_models/user';
import { UserInfo } from 'src/_models/user-info';
import { AccountService } from 'src/_services/account-service.service';

@Component({
  selector: 'app-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.css']
})
export class AccountEditComponent implements OnInit {
  @ViewChild('editForm') editForm!: NgForm;
  accountForm!: FormGroup;
  validationErrors: string[] = [];
  phonePattern = "^((\\+91-?)|0)?[0-9]{10}$";
  userInfo!: UserInfo;
  user!: User;
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }
  constructor(private accountService: AccountService, private toastr: ToastrService,
    private fb: FormBuilder, private route: ActivatedRoute) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
    })
  }

  ngOnInit(): void {
    this.userInfo = this.route.snapshot.data['userInfo'];
    this.initializeForm();
  }


  updateUser() {
    this.accountService.updateUser(this.accountForm.value).subscribe(() => {
      this.toastr.success("Profile updated successfully.");
      this.editForm.reset(this.accountForm.value);
      
    });

  }

  initializeForm() {
    this.accountForm = this.fb.group({
      firstName: [this.userInfo.firstName, Validators.required],
      lastName: [this.userInfo.lastName, Validators.required],
      phoneNumber: [this.userInfo.phoneNumber, [Validators.required, Validators.pattern(this.phonePattern)]]
    })

  }
}
