import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Floor } from 'src/_models/floor';
import { Pagination } from 'src/_models/pagination';
import { Report } from 'src/_models/report';
import { FloorService } from 'src/_services/floor.service';
import { ReportService } from 'src/_services/report.service';

@Component({
  selector: 'app-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent implements OnInit {
  floors: Floor[] = [];
  pagination!: Pagination;
  dateRange!: Date[];
  reportForm!: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;
  reports!: Report[];
  pageNumber = 1;
  pageSize = 3;

  constructor(private floorService: FloorService, private fb: FormBuilder, private reportService: ReportService) {
    this.bsConfig = {
      containerClass: 'theme-red',
      dateInputFormat: 'DD MMMM YYYY'
    }
  }

  ngOnInit(): void {
    this.initializeForm();
    this.getFloors();
  }


  getReports() {
    this.reportService.getReports(this.pageNumber, this.pageSize, this.reportForm.value).subscribe(response => {
      this.reports = response.result;
      this.pagination = response.pagination;
    });
  }

  getFloors() {
    this.floorService.getFloors().subscribe(floors => {
      this.floors = floors;
    })
  }

  initializeForm() {
    this.reportForm = this.fb.group({
      floorNumber: '',
      dateRange: ''
    })
  }

  pageChanges(event: any) {
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.getReports();
    }
  }

  isDefaultDate(date: Date){
    return new Date(date).getTime() < new Date('0002-01-01T00:00:00Z').getTime();
  }
}
