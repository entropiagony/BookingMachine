import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from 'src/_guards/auth.guard';
import { RoleGuard } from 'src/_guards/role.guard';
import { UserInfoResolver } from 'src/_resolvers/user-info.resolver';
import { AccountEditComponent } from './account/account-edit/account-edit.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { BookingComponent } from './booking/booking.component';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './not-found/not-found.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
{
  path: '',
  runGuardsAndResolvers: "always",
  canActivate: [AuthGuard],
  children: [
    { path: 'admin', component: AdminPanelComponent, canActivate: [RoleGuard],
    data: { roles: ['Admin'] } },
    { path: 'account', component: AccountEditComponent, resolve: { userInfo: UserInfoResolver } },
    {
      path: 'book', component: BookingComponent, canActivate: [RoleGuard],
      data: { roles: ['Employee'] }
    }
  ]
},
{ path: '**', component: NotFoundComponent, pathMatch: 'full' }

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
