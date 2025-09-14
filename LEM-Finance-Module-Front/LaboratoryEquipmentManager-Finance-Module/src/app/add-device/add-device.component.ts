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
  deviceOptions: { id: number; name: string }[] = [];
  selectedRelatedDeviceIds: number[] = []; 


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
      this.devices = x?.items ?? [];
      
      if (this.devices.length) {
          console.log('Przykładowy rekord urządzenia (keys):', Object.keys(this.devices[0]));
          console.log('Przykładowy rekord urządzenia (value):', this.devices[0]);
      }

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

      this.deviceOptions = this.devices
      .map((d: any) => {
        const id =
          d.id ?? d.deviceId ?? d.deviceID ?? d.device_id ?? d.DeviceId ?? d.modelId;
        
        const name = d.deviceIdentificationNumber ?? '(bez nazwy)';

        return id != null ? { id: Number(id), name } : undefined;
      })
      .filter((o): o is { id: number; name: string } => !!o);

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
      this.markFormGroupTouched(this.deviceForm);
      return;
    }

    let addDeviceDto = this.mapDeviceFormValuesToAddDeviceDto();

    if (this.selectedRelatedDeviceIds.length) {
    addDeviceDto.relatedDeviceIds = this.selectedRelatedDeviceIds.slice();
  }

    const formData = new FormData();

    formData.append('addDeviceDto', JSON.stringify(addDeviceDto));

    this.selectedDeviceFiles.forEach(file => formData.append('deviceDocuments', file));

    //if (this.selectedRelatedModelsNames.length > 0) {
    //  this.cooperatedModelsIds = [];
    //  this.selectedRelatedModelsNames.forEach(x => {
    //    this.cooperatedModelsIds.push(x.id);
    //  });
    //  addDeviceDto.cooperatedModelsIds = this.cooperatedModelsIds;
//
    //  formData.set('addDeviceDto', JSON.stringify(addDeviceDto));
    //}

    //this.markFormGroupTouched(this.deviceForm);

    if (this.deviceForm.valid) {
    this.apiService.createDevice(formData).subscribe({
      next: (x: any) => {
        alert(`Urządzenie o id: ${x.identificationNumber} zostało dodane`);
        this.deviceForm.reset();
        window.scroll({
          top: 0,
          behavior: 'smooth'
        });
      },
      error: (err) => {
        console.error('Error adding device:', err);
        alert('Wystąpił błąd podczas dodawania urządzenia.');
      }
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
    addDeviceDto.identificationNumber = this.getValueFromDeviceForm('identificationNumber');
    let productionDate = this.getValueFromDeviceForm('productionDate');
    addDeviceDto.productionDate = productionDate ? new Date(productionDate).toISOString() : undefined;
    addDeviceDto.calibrationPeriodInYears = this.getValueFromDeviceForm('calibrationPeriodInYears');
    let lastCalibrationDate = this.getValueFromDeviceForm('lastCalibrationDate');
    addDeviceDto.lastCalibrationDate = lastCalibrationDate ? new Date(lastCalibrationDate).toISOString() : undefined;
    let nextCalibrationDate = this.getValueFromDeviceForm('nextCalibrationDate');
    addDeviceDto.nextCalibrationDate = nextCalibrationDate ? new Date(nextCalibrationDate).toISOString() : undefined;
    addDeviceDto.isCalibrated = this.getValueFromDeviceForm('isCalibrated')?? undefined;
    addDeviceDto.isCalibrationCloseToExpire = this.getValueFromDeviceForm('isCalibrationCloseToExpire');
    addDeviceDto.storageLocation = this.getValueFromDeviceForm('storageLocation');
    addDeviceDto.model = this.deviceForm.get('model.name')?.value;
    addDeviceDto.serialNumber = this.deviceForm.get('model.serialNumber')?.value;
    addDeviceDto.company = { name: this.deviceForm.get('model.companyName')?.value || '' };
    addDeviceDto.relatedDeviceIds = [];

    console.log('Mapped AddDeviceDto:', addDeviceDto);
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
