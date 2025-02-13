import { AfterViewChecked, AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators, FormArray, FormBuilder, Form, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { AddDeviceDto, ApiServiceService, DeviceDto, MeasuredRangesDto, MeasuredValueDto, ModelDto, PagedAndSortedQueryOfDevicesList } from '../api-service.service';
import { Observable, OperatorFunction, catchError, debounceTime, distinctUntilChanged, forkJoin, map, of, pipe, switchMap } from 'rxjs';
import { NgbTypeaheadModule, NgbTypeaheadSelectItemEvent } from '@ng-bootstrap/ng-bootstrap';

interface ModelNameId {
  id: any,
  name: any
}

@Component({
  selector: 'app-add-device',
  templateUrl: './add-device.component.html',
  styleUrls: ['./add-device.component.css']
})
export class AddDeviceComponent implements OnInit, AfterViewInit {
  deviceForm: FormGroup;
  selectedFile: File;
  submitted = false;
  fieldRequired = 'Pole jest wymagane';
  selectedDeviceFiles: File[] = [];
  selectedModelFiles: File[] = [];
  deviceQuery = new PagedAndSortedQueryOfDevicesList();
  devices: any[] = [];
  modelsNames: string[] = [];
  modelsSerialNumbers: string[] = [];
  modelInputsDisabled: boolean = false;
  modelSerialNumberInputDisabled: boolean = false;
  modelNameInputDisabled: boolean = false;
  selectedRelatedModelsNames: any[] = [];
  modelNameIds: any[] = [];
  cooperatedModelsIds: number[] = [];
  modelSelected: boolean = false;

  constructor(private router: Router, private fb: FormBuilder, private apiService: ApiServiceService) {
    this.deviceForm = this.fb.group({
      identificationNumber: ['', Validators.required],
      productionDate: [''],
      lastCalibrationDate: [''],
      calibrationPeriodInYears: [''],
      nextCalibrationDate: [''],
      isCalibrationCloseToExpire: [''],
      storageLocation: [''],
      documents: [null],

      model: this.fb.group({
        name: ['', Validators.required],
        serialNumber: ['', Validators.required],
        companyName: [''],
        documents: [''],
        cooperatedModelsIds: [''],

        measuredValues: this.fb.array([])
      })
    });
  }
  ngAfterViewInit(): void {
    this.getDevices();
   }

  ngOnInit(): void {
  }


	searchModelName: OperatorFunction<string, readonly string[]> = (text$: Observable<string>) =>
		text$.pipe(
			debounceTime(200),
			distinctUntilChanged(),
			map((term) =>
				term.length < 2 ? [] : this.modelsNames.filter((v) => v.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10),
			),
	);

  onSelectionChange() {
  }

  onModelNameSelect($event: NgbTypeaheadSelectItemEvent){
    const selectedDevice = $event.item;
    let deviceSelected = this.devices.find(x => x.modelName === selectedDevice);
    this.deviceForm.patchValue({
      model: {
        serialNumber: deviceSelected.modelSerialNumber,
        companyName: deviceSelected.producer
      }
    });
    this.modelSelected = true;
    this.setModelInputAsDisabledByName('model.name');
    this.setModelInputAsDisabledByName('model.serialNumber');
    this.setModelInputAsDisabledByName('model.companyName');
    this.setModelInputAsDisabledByName('model.documents');
    this.modelInputsDisabled = true;
  }

  resetModelSelection(){
    this.resetModelInputByName('model.name');
    this.resetModelInputByName('model.serialNumber');
    this.resetModelInputByName('model.companyName');
    this.resetModelInputByName('model.documents');
    this.modelSelected = false;
    this.modelInputsDisabled = false;
  }

  serchModelSerialNumber: OperatorFunction<string, readonly string[]> = (text$: Observable<string>) =>
		text$.pipe(
			debounceTime(200),
			distinctUntilChanged(),
			map((term) =>
				term.length < 2 ? [] : this.modelsSerialNumbers.filter((v) => v.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10),
			),
	);

  onModelSerialNumberSelect($event: NgbTypeaheadSelectItemEvent){
    const selectedSerialNumber = $event.item;
    let deviceSelected = this.devices.find(x => x.modelSerialNumber === selectedSerialNumber);
    this.deviceForm.patchValue({
      model: {
        name: deviceSelected.modelName,
        companyName: deviceSelected.producer
      }
    });
    this.modelInputsDisabled = true;
    this.modelNameInputDisabled = true;
  }

  getDevices() {
    this.apiService.getDevices(this.deviceQuery).subscribe((x: any) => {
      this.devices = x.items;
      let modelIdNamesWithDuplicates: any = [];
      this.devices.forEach( device => {
        let modelIdName = {
          id: device.modelId,
          name: device.modelName
        };
        modelIdNamesWithDuplicates.push(modelIdName);
        this.modelsNames.push(device.modelName);
        this.modelsSerialNumbers.push(device.modelSerialNumber);
      })
      if (modelIdNamesWithDuplicates.length > 0){
        this.modelNameIds = this.prepareReatedModelsListToDisplay(modelIdNamesWithDuplicates);
      }
    });
  }

  private prepareReatedModelsListToDisplay(modelIdNamesWithDuplicates: any) : any[] {
    const uniqueIds = new Set();

    const filteredModels = modelIdNamesWithDuplicates.filter((model : any) => {
        if (!uniqueIds.has(model.id)) {
            uniqueIds.add(model.id);
            return true;
        }
        return false;
    });

    return filteredModels;
  }

  get measuredValues() {
    return this.deviceForm.get('model.measuredValues') as FormArray;
  }

  removeMeasuredValue(index: number): void {
    this.measuredValues.removeAt(index);
  }

  measuredValueRanges(index: number): FormArray {
    return this.measuredValues.at(index).get('ranges') as FormArray;
  }

  removeRangeForMeasuredValue(measuredValueIndex: number, rangeIndex: number): void {
    this.measuredValueRanges(measuredValueIndex).removeAt(rangeIndex);
  }

  addRangeForMeasuredValue(rangeIndex: number) : void {
    this.measuredValueRanges(rangeIndex).push(this.addNewRangeFG());
  }

  addMeasuredValue() : void {
    this.measuredValues.push(this.addNewMeasuredValueFG());
  }

  onSubmit() {
    this.submitted = true;
    if (this.deviceForm.invalid) {
      return;
    }

    let addDeviceDto = this.mapDeviceFormValuesToAddDeviceDto();

    const deviceFilesFormData = new FormData();
    const modelFilesFormData = new FormData();

    this.selectedDeviceFiles.forEach(file => {
      deviceFilesFormData.append('files', file);
    });

    this.selectedModelFiles.forEach(file => {
      modelFilesFormData.append('files', file);
    });

    if (this.selectedRelatedModelsNames.length > 0) {
      this.selectedRelatedModelsNames.forEach(x => {
        this.cooperatedModelsIds.push(x.id);
      });
      addDeviceDto.Model.CooperatedModelsIds = this.cooperatedModelsIds;
    }

    this.markFormGroupTouched(this.deviceForm);

    if (this.deviceForm.valid) {
      this.apiService.createDevice(addDeviceDto).pipe(
        switchMap((x: any) => {
          const observables = [];
          if (this.selectedDeviceFiles.length > 0) {
            deviceFilesFormData.append('deviceId', x.deviceId.toString());
            deviceFilesFormData.append('modelId', '');
            observables.push(this.apiService.addDocuments(deviceFilesFormData).pipe(catchError(error => of(error))));
          }
          if (this.selectedModelFiles.length > 0) {
            modelFilesFormData.append('deviceId', '');
            modelFilesFormData.append('modelId', x.modelId.toString());
            observables.push(this.apiService.addDocuments(modelFilesFormData).pipe(catchError(error => of(error))));
          }
          if(observables.length > 0){
            return forkJoin(observables).pipe(
              map(() => x)
            );
          } else {
            return of(x);
          }
        })
      ).subscribe((x) => {
        alert(`Urządzenie o id: ${x.identificationNumber} zostało dodane`);
        this.deviceForm.reset();
        window.scroll({
          top: 0,
          behavior: 'smooth'
        });
      });
    }
  }

  markFormGroupTouched(control: AbstractControl) {
    if (control instanceof FormGroup) {
      Object.keys(control.controls).forEach(key => {
        const subControl = control.controls[key];
        this.markFormGroupTouched(subControl);
      });
    } else if (control instanceof FormArray) {
      control.controls.forEach(subControl => this.markFormGroupTouched(subControl));
    } else if (control instanceof FormControl) {
      control.markAsTouched();
    }
  }

  navigateToDevicesList(): void {
    this.router.navigate(['/devices-list']);
  }

  shouldShowError(controlName: string): boolean {
    const control = this.deviceForm.get(controlName);
    return control!.invalid && (control!.touched || this.submitted);
  }

  shouldShowErrorById(id: string): boolean {
    const control = document.getElementById(id) as any;
    return !control.validity.valid;
  }

  onDeviceFileChange(event: any) {
    if (event.target.files.length > 0) {
      this.selectedDeviceFiles = Array.from(event.target.files);
    }
  }

  onModelFileChange(event: any) {
    if (event.target.files.length > 0) {
      this.selectedModelFiles = Array.from(event.target.files);
    }
  }

  private addNewRangeFG(): FormGroup {
    return this.fb.group({
      range:'',
      accuracy:''
    })
  }

  private addNewMeasuredValueFG(): FormGroup {
    return this.fb.group({
      physicalMagnitudeName: ['', Validators.required],
      physicalMagnitudeUnit: '',
      ranges: this.fb.array([])
    })
  }

  private mapDeviceFormValuesToAddDeviceDto(): AddDeviceDto {
    let addDeviceDto = new AddDeviceDto();
    addDeviceDto.IdentificationNumber = this.getValueFromDeviceForm('identificationNumber');
    let productionDate = this.getValueFromDeviceForm('productionDate');
    addDeviceDto.ProductionDate = productionDate != null ? new Date(this.getValueFromDeviceForm('productionDate')) : undefined;
    addDeviceDto.CalibrationPeriodInYears = this.getValueFromDeviceForm('calibrationPeriodInYears');
    let lastCalibrationDate = this.getValueFromDeviceForm('lastCalibrationDate');
    addDeviceDto.LastCalibrationDate = lastCalibrationDate != null ? new Date(this.getValueFromDeviceForm('lastCalibrationDate')) : undefined;
    let nextCalibrationDate = this.getValueFromDeviceForm('nextCalibrationDate');
    addDeviceDto.NextCalibrationDate = nextCalibrationDate != null ? new Date(this.getValueFromDeviceForm('nextCalibrationDate')) : undefined;
    addDeviceDto.IsCalibrated = this.getValueFromDeviceForm('isCalibrated');
    addDeviceDto.IsCalibrationCloseToExpire = this.getValueFromDeviceForm('isCalibrationCloseToExpire');
    addDeviceDto.StorageLocation = this.getValueFromDeviceForm('storageLocation');
    addDeviceDto.Model = this.getModelFromDeviceForm();

    return addDeviceDto;
  }

  private getValueFromDeviceForm(name: string): any {
    return this.deviceForm.get(name)?.value ? this.deviceForm.get(name)?.value : null;
  }

  private setModelInputAsDisabledByName(name: string) {
    this.deviceForm.get(name)?.disable();
  }

  private resetModelInputByName(name: string) {
    this.deviceForm.get(name)?.enable();
    this.deviceForm.get(name)?.setValue(null);
  }

  private getModelForm() : FormGroup {
    return this.deviceForm.get('model') as FormGroup;
  }

  private getModelFromDeviceForm(): ModelDto {
    let modelDto = new ModelDto();
    modelDto.Name = this.getValueFromDeviceForm('model.name');
    modelDto.SerialNumber = this.getValueFromDeviceForm('model.serialNumber')
    modelDto.CompanyName = this.getValueFromDeviceForm('model.companyName');
    modelDto.CooperatedModelsIds = [];
    modelDto.MeasuredValues = this.getMeasuredValuesFromDeviceForm();

    return modelDto;
  }

  private getMeasuredValuesFromDeviceForm(): any {
    let measuredValues = this.deviceForm.get('model.measuredValues') as FormArray;
    let measuredValuesDto: MeasuredValueDto[] = [];

    if(measuredValues.length === 0) {
      return null;
    }

    measuredValues.controls.forEach(x => {
      let measuredValueDto = new MeasuredValueDto();
      measuredValueDto.PhysicalMagnitudeName = x.get('physicalMagnitudeName')?.value;
      measuredValueDto.PhysicalMagnitudeUnit = x.get('physicalMagnitudeUnit')?.value;
      measuredValueDto.MeasuredRanges = this.getMeasuredRangesFromDeviceForm(x);
      measuredValuesDto.push(measuredValueDto);
    });

    return measuredValuesDto;
  }

  private getMeasuredRangesFromDeviceForm(measuredValue: AbstractControl): any {
    let measuredRangesDto: MeasuredRangesDto[] = [];

    let measuredRanges = measuredValue.get('ranges') as FormArray;

    if(!measuredRanges.value) {
      return null;
    }

    measuredRanges.controls.forEach(x => {
      let measuredRangeDto = new MeasuredRangesDto();
      measuredRangeDto.AccuracyInPercent = +x.get('accuracy')?.value;
      measuredRangeDto.Range = x.get('range')?.value;
      measuredRangesDto.push(measuredRangeDto);
    });

    return measuredRangesDto;
  }
}
