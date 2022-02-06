import { Component, Input, OnInit, Self } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css']
})
export class DateInputComponent implements ControlValueAccessor {
  @Input()
  label!: string;
  bsConfig: Partial<BsDatepickerConfig>;
  minDate = new Date();
  value: any;
  onChange!: (value: any) => void;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: 'theme-red',
      dateInputFormat: 'DD MMMM YYYY'
    }
  }


  bsValueChange(val: any) {
    setTimeout(() => {
      this.value = val;
      if (val instanceof Date) {
        this.onChange(new Date(val.getTime() - val.getTimezoneOffset() * 60 * 1000));
      } else {
        this.onChange(val);
      }
    });
  }

  writeValue(val: any): void {
    if (val) {
      if (val instanceof Date) {
        this.value = new Date(val.getTime() + val.getTimezoneOffset() * 60 * 1000);
      } else {
        this.value = val;
      }
    }
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }
  registerOnTouched(fn: any): void {

  }

}
