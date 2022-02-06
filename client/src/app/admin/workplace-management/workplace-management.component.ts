import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Floor } from 'src/_models/floor';
import { FloorService } from 'src/_services/floor.service';
import { WorkplaceService } from 'src/_services/workplace.service';

@Component({
  selector: 'app-workplace-management',
  templateUrl: './workplace-management.component.html',
  styleUrls: ['./workplace-management.component.css']
})
export class WorkplaceManagementComponent implements OnInit {

  floors!: Floor[];
  workPlaceCount: number = 0;
  floorForm!: FormGroup;
  selectedFloor!: Floor;
  @Input()
  floorNumber!: number;
  @Input()
  quantity!: number;

  constructor(private floorService: FloorService, private toastr: ToastrService,
    private fb: FormBuilder, private workPlaceService: WorkplaceService) { }

  ngOnInit(): void {
    this.getFloors();
    this.initializeForm();
  }

  initializeForm() {
    this.floorForm = this.fb.group({
      floorId: ['', Validators.required],
      workPlaceId: ['', Validators.required],
    })
  }

  getFloors() {
    this.floorService.getFloors().subscribe(floors => {
      this.floors = floors;
      floors.forEach(floor => {
        this.workPlaceCount += floor.workPlaces.length;
      });
    })
  }

  changeFloor(e: any) {
    let id = this.floorForm.controls['floorId'].value;
    this.selectedFloor = this.floors.find(x => x.id == id)!;
  }

  deleteWorkPlace() {
    let workPlaceId = this.floorForm.value.workPlaceId;
    this.workPlaceService.deleteWorkPlace(workPlaceId).subscribe(() => {
      this.toastr.success("Workplace successfully deleted!");
      this.workPlaceCount--;
      this.selectedFloor.workPlaces = this.selectedFloor.workPlaces.filter(workplace => workplace.id != workPlaceId);
      this.floorForm.get("workPlaceId")?.setValue(this.selectedFloor.workPlaces[0].id);
    }
    );
  }

  addWorkPlaces() {
    let floorId = this.selectedFloor.id;
    this.workPlaceService.createWorkPlaces(this.quantity, floorId).subscribe(response => {
      this.toastr.success("Workplaces successfully added!");
      this.workPlaceCount += response.length;
      if (this.selectedFloor.workPlaces != null) {
        this.selectedFloor.workPlaces = this.selectedFloor.workPlaces.concat(response);
      } else {
        this.selectedFloor.workPlaces = response;
      }
    })
  }

  addFloor() {
    this.floorService.createFloor(this.floorNumber).subscribe(response => {
      this.toastr.success("Floor successfully created!");
      this.floors.push(response);
    })
  }

  deleteFloor() {
    this.floorService.deleteFloor(this.selectedFloor.id).subscribe(response => {
      this.toastr.success("Floor successfully deleted!");
      this.floors = this.floors.filter(x => x.id != this.selectedFloor.id);
      this.selectedFloor = this.floors[0];
    })
  }
}
