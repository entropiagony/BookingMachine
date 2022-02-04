import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from 'src/_services/account-service.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public accountService: AccountService, private router: Router) { }

  ngOnInit(): void {

  }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      this.router.navigateByUrl('/book');

    })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
