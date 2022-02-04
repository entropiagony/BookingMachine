import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router} from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { map, Observable } from 'rxjs';
import { AccountService } from 'src/_services/account-service.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private accountService: AccountService, private toastr: ToastrService, private router: Router) { }


  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    let roles = route.data["roles"] as Array<string>;
    return this.accountService.currentUser$.pipe(
      map(user => {
        roles.forEach(role => {
          if (!user.roles.includes(role)) {
            this.toastr.error("You cannot enter this area");
            this.router.navigateByUrl('/');
            return false;

          }
        });
        return true;
      })
    );
  }

}
