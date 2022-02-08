import { Component, OnInit } from '@angular/core';
import { User } from 'src/_models/user';
import { AccountService } from 'src/_services/account-service.service';
import { BookingService } from 'src/_services/booking.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client';

  constructor(private accountService: AccountService, private bookingService: BookingService) { }

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser() {
    //@ts-ignore
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (user) {
      this.accountService.setCurrentUser(user);
      this.bookingService.createHubConnection(user);
    }

  }
}
