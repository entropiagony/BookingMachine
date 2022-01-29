import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminGuard } from 'src/_guards/admin.guard';
import { AuthGuard } from 'src/_guards/auth.guard';
import { UserInfoResolver } from 'src/_resolvers/user-info.resolver';
import { AccountEditComponent } from './account/account-edit/account-edit.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './not-found/not-found.component';

const routes: Routes = [{ path: '', component: HomeComponent },
{
  path: '',
  runGuardsAndResolvers: "always",
  canActivate: [AuthGuard],
  children: [
    { path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard] },
    { path: 'account', component: AccountEditComponent, resolve: { userInfo: UserInfoResolver } }
  ]
},
{ path: '**', component: NotFoundComponent, pathMatch: 'full' }

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
