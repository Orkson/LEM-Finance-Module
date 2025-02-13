import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { ApiServiceService, DeviceDto, PagedAndSortedQueryOfDevicesList, SearchPhraseDto } from '../api-service.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { DeviceDetailsComponent } from './device-details/device-details.component';
import { FormControl } from '@angular/forms';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';

@Component({
  selector: 'app-devices-list',
  templateUrl: './devices-list.component.html',
  styleUrls: ['./devices-list.component.css']
})
export class DevicesListComponent implements OnInit, AfterViewInit {

  constructor(
    private service: ApiServiceService,
    private router: Router,
    private dialog: MatDialog
    ){}

  @ViewChild(MatPaginator) paginator: MatPaginator;
  selectedMonthsToExpire: number = 1;
  DevicesList: any = [];
  measuredValues: any = [];
  measuredValuesAsStringTab: string[] = [];
  serchPhraseFC = new FormControl();
  searchPhrase = new SearchPhraseDto;
  order = "asc";
  sortColumn = '';

  deviceQuery = new PagedAndSortedQueryOfDevicesList();
  totalDevicesCount: number = 0;

  devicePhysicalMagnitudeNames: string[] = [];

  ngOnInit(): void {
  }
  ngAfterViewInit(): void {
    this.refreshDevicesList();
  }

  refreshDevicesList(): void {
    let searchPhrase = this.serchPhraseFC.value;

    this.deviceQuery.Page = 1;
    this.deviceQuery.PageSize = this.paginator?.pageSize ? this.paginator.pageSize : 10;
    this.deviceQuery.SearchTerm = searchPhrase;
    this.deviceQuery.SortOrder = this.order;
    this.deviceQuery.SortColumn = this.sortColumn;
    this.service.getDevices(this.deviceQuery).subscribe((x: any) => {
      this.DevicesList = x;
      this.prepareDevicesMeasuredValuesToDisplay(this.DevicesList.items)
      this.totalDevicesCount = x.totalCount;
    });
    if (this.paginator){
      this.paginator.firstPage();
    }
  }

  onPageChanged($event: any): void {
    this.deviceQuery.Page = $event.pageIndex + 1;
    this.deviceQuery.PageSize = $event.pageSize;

    this.service.getDevices(this.deviceQuery).subscribe((x: any) => {
      this.DevicesList = x;
      this.prepareDevicesMeasuredValuesToDisplay(this.DevicesList.items)
    });
  }

  navigateToAddDevice(): void {
    this.router.navigate(['/add-device'])
  }

  openDeviceDetails(deviceId: any) {
    this.service.getDeviceDetailsById(deviceId).subscribe(deviceDetails => {
      const dialogRef = this.dialog.open(DeviceDetailsComponent, {
        data: { deviceDto: deviceDetails },
        autoFocus: false
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.refreshDevicesList();
        }
      });
    });
  }

  clearList(){
    this.DevicesList = null;
  }

  find() {
    this.refreshDevicesList();
  }

  onSearchChanged($event: any){
    this.refreshDevicesList();
  }

  markDeviceCloseToExpire(device: any): string {
    const now = new Date();

    if(device.nextCalibrationDate === null){
      return "";
    }
    let expireDate = device.nextCalibrationDate;
    expireDate = new Date(expireDate);

    if (now > expireDate){
      return "table-danger";
    }

    const differenceInMilliseconds = expireDate.getTime() - now.getTime()
    const diffrerenceInDays = Math.floor(differenceInMilliseconds / (1000 * 60 * 60 * 24));
    const diffrerenceInMonths = diffrerenceInDays/(30);

    if(this.selectedMonthsToExpire > expireDate){
      return "table-warning";
    }

    if (this.selectedMonthsToExpire < diffrerenceInMonths){
      return "table-success";
    } else {
      return "table-warning";
    }
  }

  refreshCloseToExpireValue($event: any) {
    this.refreshDevicesList();
  }

  setOrderDirection(columnName: string) {
    let buttonId = columnName == "modelName" ? "orderButtonName" : "orderButtonDate";
    let orderBtn = document.getElementById(buttonId);
    if (this.order === "asc") {
      this.order = "desc";
      orderBtn?.setAttribute("style", "transform: rotate(180deg)")
    } else if (this.order === "desc") {
      this.order = "asc";
      orderBtn?.setAttribute("style", "transform: rotate(0deg)")
    }
    this.sortColumn = columnName;
    this.refreshDevicesList();
    this.sortColumn = '';
  }

  getCalibrationExpireDateForDevice(device: any): any {
    if (device.lastCalibrationDate === null) {
      return null;
    }

    const calibrationPeriodInYears = device.calibrationPeriodInYears;
    const lastCalibrationDate = new Date(device.lastCalibrationDate);
    const expireDate = new Date(lastCalibrationDate.setFullYear(lastCalibrationDate.getFullYear() + calibrationPeriodInYears));
    return expireDate;
  }

  private getDevicesBySearchedPhrase(searchPhrase: SearchPhraseDto): void {
    this.service.getDevicesByModelName(searchPhrase).subscribe(devices => {
      //this.prepareDevicesToDisplay(devices);
    })
  }

  private prepareDevicesToDisplay(devices: any[]): void {
    this.DevicesList = devices;
    this.prepareDevicesMeasuredValuesToDisplay(devices);
  }

  private prepareDevicesMeasuredValuesToDisplay(devices: any[]): void {
    this.measuredValuesAsStringTab = [];
    devices.forEach(x => {
      x.measuredValues.forEach((element: any) => {
        this.devicePhysicalMagnitudeNames.push(element.physicalMagnitudeName);
      });
      let value = this.devicePhysicalMagnitudeNames.length === 0 ? "--" : this.createStringFromDevicePhysicalMagnitudeNamesList(this.devicePhysicalMagnitudeNames)
      this.measuredValuesAsStringTab.push(value);
      this.devicePhysicalMagnitudeNames = [];
    });
  }

  private createStringFromDevicePhysicalMagnitudeNamesList(devicePhysicalMagnitudeNames: any []): string {
    return devicePhysicalMagnitudeNames.join(', ');
  }
}
