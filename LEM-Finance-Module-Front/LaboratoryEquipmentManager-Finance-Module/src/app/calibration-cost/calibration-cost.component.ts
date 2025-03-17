import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface CalibrationCost {
  id: string;
  deviceName: string;
  amount: number;
  currency: string;
  calibrationDate: string;
}

@Component({
  selector: 'app-calibration-cost',
  templateUrl: './calibration-cost.component.html',
  styleUrls: ['./calibration-cost.component.css']
})
export class CalibrationCostComponent implements OnInit {
  calibrationCost: CalibrationCost[] = [];
  selectedYear: number = new Date().getFullYear();
  years: number[] = [];

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.generateYearOptions();
    this.fetchCalibrationCosts();
  }

  generateYearOptions() {
    const currentYear = new Date().getFullYear();
    for (let i = currentYear - 5; i <= currentYear + 1; i++) {
      this.years.push(i);
    }
  }

  fetchCalibrationCosts() {
    this.http.get<CalibrationCost[]>(`/api/calibration-cost/${this.selectedYear}`)
      .subscribe(data => this.calibrationCost = data);
  }
}

