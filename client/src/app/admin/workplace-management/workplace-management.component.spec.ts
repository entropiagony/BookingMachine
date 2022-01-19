import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkplaceManagementComponent } from './workplace-management.component';

describe('WorkplaceManagementComponent', () => {
  let component: WorkplaceManagementComponent;
  let fixture: ComponentFixture<WorkplaceManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WorkplaceManagementComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkplaceManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
