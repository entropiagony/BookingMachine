import { Injectable } from '@angular/core';
import {
  Router, Resolve,
  RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { UserInfo } from 'src/_models/user-info';
import { AccountService } from 'src/_services/account-service.service';

@Injectable({
  providedIn: 'root'
})
export class UserInfoResolver implements Resolve<UserInfo> {
  constructor(private accountService: AccountService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<UserInfo> {
    return this.accountService.getCurrentUser();
  }
}
