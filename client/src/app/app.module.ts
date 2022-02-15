import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HasRoleDirective } from './directives/has-role.directive';
import { ToastrModule } from 'ngx-toastr';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { RegisterComponent } from './register/register.component';
import { TextInputComponent } from './forms/text-input/text-input.component';
import { ErrorInterceptor } from 'src/_interceptors/error.interceptor';
import { JwtInterceptor } from 'src/_interceptors/jwt.interceptor';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { WorkplaceManagementComponent } from './admin/workplace-management/workplace-management.component';
import { RolesModalComponent } from './roles-modal/roles-modal.component';
import { AccountEditComponent } from './account/account-edit/account-edit.component';
import { BookingComponent } from './booking/booking.component';
import { DateInputComponent } from './forms/date-input/date-input.component';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ManagerComponent } from './manager/manager.component';
import { BookingMonitoringComponent } from './admin/booking-monitoring/booking-monitoring.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ReportsComponent } from './reports/reports.component';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HasRoleDirective,
    HomeComponent,
    NotFoundComponent,
    RegisterComponent,
    TextInputComponent,
    AdminPanelComponent,
    UserManagementComponent,
    WorkplaceManagementComponent,
    RolesModalComponent,
    AccountEditComponent,
    BookingComponent,
    DateInputComponent,
    ManagerComponent,
    BookingMonitoringComponent,
    ReportsComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BsDropdownModule.forRoot(),
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    },
    ),
    FormsModule,
    PaginationModule.forRoot(),
    BsDatepickerModule.forRoot(),
    ReactiveFormsModule,
    TabsModule.forRoot(),
    ModalModule.forRoot()
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
