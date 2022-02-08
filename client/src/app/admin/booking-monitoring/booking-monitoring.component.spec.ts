import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BookingMonitoringComponent } from './booking-monitoring.component';

describe('BookingMonitoringComponent', () => {
  let component: BookingMonitoringComponent;
  let fixture: ComponentFixture<BookingMonitoringComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BookingMonitoringComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BookingMonitoringComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
