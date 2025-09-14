import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormControl, FormArray, AbstractControl } from '@angular/forms';
import { MatCheckboxChange } from '@angular/material/checkbox';
import { Router } from '@angular/router';
import { NgbTypeaheadSelectItemEvent } from '@ng-bootstrap/ng-bootstrap';
import { Observable, OperatorFunction, catchError, debounceTime, distinctUntilChanged, forkJoin, map, of, switchMap } from 'rxjs';
import { AddDeviceDto, ApiServiceService, MeasuredRangesDto, MeasuredValueDto, ModelDto, PagedAndSortedQueryOfDevicesList } from 'src/app/api-service.service';

export enum TableName {
  editedDeviceDocuments = 'editedDeviceDocuments',
  editedModelDocuments = 'editedModelDocuments',
  relatedDevices = 'relatedDevices'
}
@Component({
  selector: 'app-edit-device',
  templateUrl: './edit-device.component.html',
  styleUrl: './edit-device.component.css'
})
export class EditDeviceComponent implements AfterViewInit, OnInit {
  deviceToEdit: any;
  deviceToBeEditedDto: AddDeviceDto | null = null;
  deviceForm: FormGroup;
  deviceQuery = new PagedAndSortedQueryOfDevicesList();
  devices: any[] = [];
  modelNameIds: any[] = [];
  modelsNames: string[] = [];
  modelsSerialNumbers: string[] = [];
  fieldRequired = 'Pole jest wymagane';
  submitted = false;
  measuredValuesControlss: FormControl[] = [];
  mesValFormArray:  FormArray = new FormArray([new FormControl]);
  selectedRelatedModelsNames: any[] = [];
  relateModelsNamesToEdit: any[] = [];
  relatedModelsIds: any[] = [];
  oldRelatedModelsIds: any[] = [];
  relateModelsIdsToBeRemoved: number[] = [];
  editedDeviceDocuments: any[] = [];
  deviceDocumentsIdsToBeRemoved: any[] = [];
  editedModelDocuments: any[] = [];
  modelDocumentsIdsToBeRemoved: any[] = [];
  checkedMap: {[key: string]: boolean} = {};
  selectedDeviceFiles: File[] = [];
  anyRelatedDevices: boolean = false;
  anyDeviceDocuments: boolean = false;
  anyModelDocuments: boolean = false;
  selectedModelFiles: File[] = [];
  cooperatedModelsIds: number[] = [];
  selectedModelFilesIdsToBeRemoved: number[] = [];
  selectedDeviceFilesIdsToBeRemoved: number[] = [];
  relatedDevicesToEdit: { id: number; name: string }[] = [];
  relatedDeviceIds: number[] = [];
  oldRelatedDeviceIds: number[] = [];
  relatedDeviceIdsToBeRemoved: number[] = [];
  deviceOptions: { id: number; name: string }[] = [];
  selectedRelatedDeviceIds: number[] = [];

  successMessage: string = '';
  errorMessage: string = '';

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

  ngOnInit(): void {
    this.deviceToEdit = history.state.data;
    if (!this.deviceToEdit) {
      alert('Brak danych urządzenia do edycji.');
      this.navigateToDevicesList();
      return;
    }

    this.apiService.getDeviceById(this.deviceToEdit.deviceId, true).subscribe((full: any) => {
    this.deviceToEdit = full;
    
    const relations = full?.deviceRelations ?? full?.deviceRelations ?? [];

    this.relatedDeviceIds = Array.isArray(relations)
    ? relations.map((r: any) => Number(r.relatedDeviceId)).filter((id: any) => !isNaN(id))
    : [];

    this.relatedDevicesToEdit = Array.isArray(relations)
      ? relations.map((r: any) => ({
        id: Number(r.relatedDeviceId),
        name: this.buildRelatedDeviceDisplayName(r.relatedDevice)
        }))
      : [];

    this.oldRelatedDeviceIds = [...this.relatedDeviceIds];

    this.getDevices();
    this.setOptionalAreasVisibility();
    this.setRelateModelsNamesToEdit(this.deviceToEdit.relatedModels);
    
    this.initializeMeasuredValues(this.deviceToEdit.measuredValues);
    this.initializeEditedDeviceDocuments(this.deviceToEdit.deviceDocuments);
    this.initializeEditedModelDocuments(this.deviceToEdit.modelDocuments);
    this.bindOldValues();
    this.deviceToBeEditedDto = this.mapDeviceToEditToAddDeviceDto();
  });
  }

  private buildRelatedDeviceDisplayName(d: any): string {
    if (!d) return 'ID ?';
    const num = d.identificationNumber ?? d.deviceIdentificationNumber ?? null;
    const model = d.model ?? d.modelName ?? null;
    const serial = d.serialNumber ?? null;
    if (num && model && serial) return `${num} • ${model} (${serial})`;
    if (num && model) return `${num} • ${model}`;
    if (num) return `${num}`;
    if (model && serial) return `${model} (${serial})`;
    if (model) return `${model}`;
    return `ID ${d.id ?? '?'}`;
  }

private setRelatedDevicesToEdit(relatedDevices: any[]) {
  this.relatedDevicesToEdit = [];
  if (!Array.isArray(relatedDevices)) relatedDevices = [];

  const displayName = (d: any): string => {
    const num = d.deviceIdentificationNumber ?? d.identificationNumber ?? d.DeviceIdentificationNumber ?? d.IdentificationNumber ?? null;
    const model = d.model ?? d.Model ?? d.modelName ?? d.ModelName ?? null;
    const serial = d.serialNumber ?? d.SerialNumber ?? d.modelSerialNumber ?? d.ModelSerialNumber ?? null;
    if (num && model && serial) return `${num} • ${model} (${serial})`;
    if (num && model) return `${num} • ${model}`;
    if (num) return `${num}`;
    if (model && serial) return `${model} (${serial})`;
    if (model) return `${model}`;
    return `ID ${d.id ?? d.DeviceId ?? d.deviceId ?? '?'}`;
  };

  this.relatedDevicesToEdit = relatedDevices.map((r: any) => ({
    id: Number(r.relatedDeviceId),
    name: displayName(r.relatedDevice)
  }));

  this.relatedDeviceIds = this.relatedDevicesToEdit.map(x => x.id);
  this.oldRelatedDeviceIds = [...this.relatedDeviceIds];
  this.selectedRelatedDeviceIds = [...this.relatedDeviceIds];
}

  private setOptionalAreasVisibility() {
  this.anyModelDocuments  = Array.isArray(this.deviceToEdit?.modelDocuments)  && this.deviceToEdit.modelDocuments.length  > 0;
  this.anyDeviceDocuments = Array.isArray(this.deviceToEdit?.deviceDocuments) && this.deviceToEdit.deviceDocuments.length > 0;
  this.anyRelatedDevices  = Array.isArray(this.deviceToEdit?.deviceRelations)  && this.deviceToEdit.deviceRelations.length  > 0;
}

  setRelateModelsNamesToEdit(relatedModelsToEdit: any[]) {
    this.relateModelsNamesToEdit = [];

    if (!relatedModelsToEdit) {
    return;
    }

    relatedModelsToEdit.forEach(x => {
      this.relateModelsNamesToEdit.push({id: x.id, name: x.name});
    })

    this.relatedModelsIds = relatedModelsToEdit?.map((x: any) => x.id) || [];
    this.oldRelatedModelsIds = [...this.relatedModelsIds];
  }

  initializeEditedDeviceDocuments(documents: any) {
    this.editedDeviceDocuments = [];
    if (!Array.isArray(documents) || documents.length === 0) {
      return;
    }
    documents.forEach((x: any) => {
      this.editedDeviceDocuments.push({name: x.name, Id: x.id})
    })
  }

  initializeEditedModelDocuments(documents: any) {
    this.editedModelDocuments = []
    if (!Array.isArray(documents) || documents.length === 0) {
      return;
    }
    documents.forEach((x: any) => {
      this.editedModelDocuments.push({name: x.name, Id: x.id})
    })
  }

  initializeMeasuredValues(measuredValues: any[]) {
    const measuredValuesArray = this.deviceForm.get('model.measuredValues') as FormArray;
    measuredValuesArray.clear();
    if (!measuredValues || measuredValues.length === 0) {
    return;
    }

    measuredValues.forEach(x => {
      const rangesArray = this.fb.array([] as FormGroup[]);

      if(x.measuredRanges != null){
        x.measuredRanges.forEach((y: any) => {
          rangesArray.push(this.fb.group({
            accuracyInPercent: [y.accuracyInPercent],
            range: [y.range]
          }));
        });
      }

      const physicalMagnitudeFG = this.fb.group({
        physicalMagnitudeName: [x.physicalMagnitudeName],
        physicalMagnitudeUnit: [x.physicalMagnitudeUnit],
        ranges: rangesArray
      });

      measuredValuesArray.push(physicalMagnitudeFG);
    });
  }

  private hasFormChanged(newEditedAddDeviceDto: AddDeviceDto): boolean {
  if (!this.deviceToBeEditedDto) return true;

  const devicesTheSame = this.compareDevices(this.deviceToBeEditedDto, newEditedAddDeviceDto);
  const linksSame = this.arraysHaveSameValues(this.oldRelatedDeviceIds ?? [], this.selectedRelatedDeviceIds ?? []);
  const filesOrDocsChanged =
    this.selectedDeviceFiles.length > 0 ||
    this.deviceDocumentsIdsToBeRemoved.length > 0 ||
    this.modelDocumentsIdsToBeRemoved.length > 0;

  return !devicesTheSame || !linksSame || filesOrDocsChanged;
}

  private arraysHaveSameValues(arr1: any[], arr2: any[]): boolean {
    if (arr1.length !== arr2.length) {
      return false;
    }

    const sortedArr1 = arr1.slice().sort();
    const sortedArr2 = arr2.slice().sort();

    for (let i = 0; i < sortedArr1.length; i++) {
      if (sortedArr1[i] !== sortedArr2[i]) {
        return false;
      }
    }

    return true;
  }

onSubmit() {
  type ResultItem = { type: 'addDocuments' | 'removeDocuments'; success: boolean; data?: any; error?: any; };
  this.submitted = true;

  const newEditedAddDeviceDto = this.mapDeviceFormValuesToAddDeviceDto();

  if (this.deviceForm.invalid) return;

  const toRemove = this.oldRelatedDeviceIds.filter(id => !this.selectedRelatedDeviceIds.includes(id));
  this.relatedDeviceIdsToBeRemoved = toRemove;

  newEditedAddDeviceDto.relatedDeviceIds = this.selectedRelatedDeviceIds.length
    ? [...this.selectedRelatedDeviceIds]
    : undefined;

  if (!this.hasFormChanged(newEditedAddDeviceDto)) {
    alert('Aby dokonać edycji urządzenia, wprowadzone dane muszą się różnić.');
    return;
  }

  const deviceFilesFormData = new FormData();
  this.selectedDeviceFiles.forEach((f) => deviceFilesFormData.append('files', f));

  this.apiService
    .editDevice(
      this.deviceToEdit.deviceId,
      this.deviceToBeEditedDto!,
      newEditedAddDeviceDto,
      this.relatedDeviceIdsToBeRemoved
    )
    .pipe(
      switchMap((x: any) => {
        const observables: Observable<any>[] = [];

        if (this.selectedDeviceFiles.length > 0) {
          deviceFilesFormData.append('deviceId', x.deviceId.toString());
          deviceFilesFormData.append('modelId', '');
          observables.push(
            this.apiService.addDocuments(deviceFilesFormData).pipe(
              map(res => ({ type: 'addDocuments', success: true, data: res })),
              catchError(err => of({ type: 'addDocuments', success: false, error: err }))
            )
          );
        }
        if (this.deviceDocumentsIdsToBeRemoved.length > 0) {
          observables.push(
            this.apiService.removeDocuments(this.deviceDocumentsIdsToBeRemoved).pipe(
              map(res => ({ type: 'removeDocuments', success: true, data: res })),
              catchError(err => of({ type: 'removeDocuments', success: false, error: err }))
            )
          );
        }
        return observables.length > 0
          ? forkJoin(observables).pipe(map(results => ({ x, results })))
          : of({ x, results: [] as ResultItem[] });
      })
    )
    .subscribe(({ x, results }) => {
      this.relatedDevicesToEdit = this.relatedDevicesToEdit.filter(rd => !toRemove.includes(rd.id));
      this.anyRelatedDevices = this.relatedDevicesToEdit.length > 0;
      this.oldRelatedDeviceIds = [...this.selectedRelatedDeviceIds];

      const addResult = results.find(r => r.type === 'addDocuments');
      const removeResult = results.find(r => r.type === 'removeDocuments');
      let anyError = false;
      if (addResult && !addResult.success) { this.errorMessage = addResult.error?.error || 'Błąd podczas dodawania plików.'; anyError = true; }
      else if (addResult && addResult.success) { this.successMessage = 'Pliki zostały pomyślnie dodane.'; }
      if (removeResult && !removeResult.success) { this.errorMessage = removeResult.error?.error || 'Błąd podczas usuwania plików.'; anyError = true; }
      if (anyError) { window.scroll({ top: 0, behavior: 'smooth' }); return; }

      const successText = addResult?.success
        ? 'Urządzenie zostało zaktualizowane. Pliki dodane pomyślnie.'
        : 'Urządzenie zostało zaktualizowane.';

      this.router.navigate(['/devices-list'], { state: { message: successText } });
    });
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

  compareDevices(device1: AddDeviceDto, device2: AddDeviceDto): boolean {
    return this.compareObjects(device1, device2);
}

  compareObjects(obj1: any, obj2: any): boolean {
    if (obj1 === obj2) return true;

    if (!obj1 || !obj2) return false;

    if (typeof obj1 !== typeof obj2) return false;

    if (obj1 instanceof Date && obj2 instanceof Date) {
      return obj1.getTime() === obj2.getTime();
    }

    if (Array.isArray(obj1)) {
        if (!Array.isArray(obj2)) return false;
        if (obj1.length !== obj2.length) return false;
        for (let i = 0; i < obj1.length; i++) {
            if (!this.compareObjects(obj1[i], obj2[i])) return false;
        }
        return true;
    }

    if (typeof obj1 === 'object') {
        const keys1 = Object.keys(obj1);
        const keys2 = Object.keys(obj2);

        if (keys1.length !== keys2.length) return false;

        for (const key of keys1) {
            if (!this.compareObjects(obj1[key], obj2[key])) return false;
        }
        return true;
    }

    return obj1 === obj2;
}

private mapDeviceToEditToAddDeviceDto(): AddDeviceDto {
  const addDeviceDto = new AddDeviceDto();
  addDeviceDto.identificationNumber = this.deviceToEdit.deviceIdentificationNumber || '';
  addDeviceDto.productionDate = this.deviceToEdit.productionDate ? new Date(this.deviceToEdit.productionDate).toISOString() : undefined;
  addDeviceDto.calibrationPeriodInYears = this.deviceToEdit.calibrationPeriodInYears || undefined;
  addDeviceDto.lastCalibrationDate = this.deviceToEdit.lastCalibrationDate ? new Date(this.deviceToEdit.lastCalibrationDate).toISOString() : undefined;
  addDeviceDto.nextCalibrationDate = this.deviceToEdit.nextCalibrationDate ? new Date(this.deviceToEdit.nextCalibrationDate).toISOString() : undefined;
  addDeviceDto.isCalibrated = this.deviceToEdit.isCalibrated ?? undefined;
  addDeviceDto.isCalibrationCloseToExpire = this.deviceToEdit.isCalibrationCloseToExpire ?? undefined;
  addDeviceDto.storageLocation = this.deviceToEdit.storageLocation || undefined;
  addDeviceDto.model = this.deviceToEdit.modelName || '';
  addDeviceDto.serialNumber = this.deviceToEdit.serialNumber || '';
  addDeviceDto.company = { name: this.deviceToEdit.producer || '' };
  addDeviceDto.cooperatedModelsIds = this.deviceToEdit.relatedModels?.map((x: any) => x.id) || [];
  addDeviceDto.relatedDeviceIds = this.deviceToEdit.relatedDevices?.map((x: any) => x.id) || [];

  return addDeviceDto;
}

private mapDeviceFormValuesToAddDeviceDto(): AddDeviceDto {
    const addDeviceDto = new AddDeviceDto();
    addDeviceDto.identificationNumber = this.getValueFromDeviceForm('identificationNumber') || '';
    const productionDate = this.getValueFromDeviceForm('productionDate');
    addDeviceDto.productionDate = productionDate ? new Date(productionDate).toISOString() : undefined;
    addDeviceDto.calibrationPeriodInYears = this.getValueFromDeviceForm('calibrationPeriodInYears') ?
      +this.getValueFromDeviceForm('calibrationPeriodInYears') : undefined;
    const lastCalibrationDate = this.getValueFromDeviceForm('lastCalibrationDate');
    const calibrationPeriod = this.getValueFromDeviceForm('calibrationPeriodInYears');
    
    addDeviceDto.lastCalibrationDate = lastCalibrationDate ? new Date(lastCalibrationDate).toISOString() : undefined;
    
    if (lastCalibrationDate && calibrationPeriod) {
      const currentISO = new Date(lastCalibrationDate).toISOString();
      const oldISO = this.deviceToEdit.lastCalibrationDate
        ? new Date(this.deviceToEdit.lastCalibrationDate).toISOString()
        : null;

      if (currentISO !== oldISO) {
        addDeviceDto.lastCalibrationDate = currentISO;
      }
    }
    if (lastCalibrationDate && calibrationPeriod) {
      const nextDate = new Date(lastCalibrationDate);
      nextDate.setFullYear(nextDate.getFullYear() + Number(calibrationPeriod));
      addDeviceDto.nextCalibrationDate = nextDate.toISOString();
    } else {
      addDeviceDto.nextCalibrationDate = undefined;
    }  
    addDeviceDto.isCalibrated = this.getValueFromDeviceForm('isCalibrated') ?? undefined;
    addDeviceDto.isCalibrationCloseToExpire = this.getValueFromDeviceForm('isCalibrationCloseToExpire') ?? undefined;
    addDeviceDto.storageLocation = this.getValueFromDeviceForm('storageLocation') || undefined;
    addDeviceDto.model = this.deviceForm.get('model.name')?.value || '';
    addDeviceDto.serialNumber = this.deviceForm.get('model.serialNumber')?.value || '';
    addDeviceDto.company = { name: this.deviceForm.get('model.companyName')?.value || '' };
    addDeviceDto.cooperatedModelsIds = this.cooperatedModelsIds;
    
    if (this.selectedRelatedDeviceIds?.length) {
    addDeviceDto.relatedDeviceIds = this.selectedRelatedDeviceIds.slice();
  }

    return addDeviceDto;
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
      measuredRangeDto.AccuracyInPercent = +x.get('accuracyInPercent')?.value;
      measuredRangeDto.Range = x.get('range')?.value;
      measuredRangesDto.push(measuredRangeDto);
    });

    return measuredRangesDto;
  }

  private getValueFromDeviceForm(name: string): any {
    return this.deviceForm.get(name)?.value ? this.deviceForm.get(name)?.value : null;
  }

  ngAfterViewInit(): void {
    this.getDevices();
  }
  get measuredValuesControls() {
    return (this.deviceForm.get('model.measuredValues') as FormArray).controls;
  }

  onCheckboxChange(name: string, id: number, tableName: string, event: MatCheckboxChange) {
  this.checkedMap[name] = event.checked;
  if (tableName === TableName.relatedDevices) {
    if (event.checked) {
      this.relatedDeviceIdsToBeRemoved.push(id);
    } else {
      this.relatedDeviceIdsToBeRemoved = this.relatedDeviceIdsToBeRemoved.filter(n => n !== id);
    }
    return;
  }
}

  getDevices() {
  this.apiService.getDevices(this.deviceQuery).subscribe((x: any) => {
    this.devices = x?.items ?? [];

    this.deviceOptions = this.devices
      .map((d: any) => {
        const id = d.id ?? d.deviceId ?? d.deviceID ?? d.device_id ?? d.DeviceId;
        const name = d.deviceIdentificationNumber ?? '(bez nazwy)';
        return id != null ? { id: Number(id), name } : undefined;
      })
      .filter((o): o is { id: number; name: string } => !!o)
      .filter((o) => o.id !== this.deviceToEdit.deviceId);
      
    this.selectedRelatedDeviceIds = [...this.relatedDeviceIds];

    if (!this.selectedRelatedDeviceIds?.length && this.relatedDeviceIds?.length) {
    this.selectedRelatedDeviceIds = [...this.relatedDeviceIds];
    }
  });
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

  searchModelName: OperatorFunction<string, readonly string[]> = (text$: Observable<string>) =>
		text$.pipe(
			debounceTime(200),
			distinctUntilChanged(),
			map((term) =>
				term.length < 2 ? [] : this.modelsNames.filter((v) => v.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10),
			),
	);

  serchModelSerialNumber: OperatorFunction<string, readonly string[]> = (text$: Observable<string>) =>
		text$.pipe(
			debounceTime(200),
			distinctUntilChanged(),
			map((term) =>
				term.length < 2 ? [] : this.modelsSerialNumbers.filter((v) => v.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10),
			),
	);

  onModelNameSelect($event: NgbTypeaheadSelectItemEvent){
    const selectedDevice = $event.item;
    let deviceSelected = this.devices.find(x => x.modelName === selectedDevice);
    var modelMeasuredValues = deviceSelected.measuredValues;
    this.initializeMeasuredValues(modelMeasuredValues);

    this.deviceForm.patchValue({
      identificationNumber: this.deviceToEdit.deviceIdentificationNumber || '',
      lastCalibrationDate: this.deviceToEdit.lastCalibrationDate ? this.deviceToEdit.lastCalibrationDate.split('T')[0] : '',    calibrationPeriodInYears: this.deviceToEdit.calibrationPeriodInYears || '',
      storageLocation: this.deviceToEdit.storageLocation || '',
      model: {
        name: deviceSelected.name,
        serialNumber: deviceSelected.serialNumber,
        companyName: deviceSelected.producer
      }
    });
  }

  onModelSerialNumberSelect($event: NgbTypeaheadSelectItemEvent){
    const selectedSerialNumber = $event.item;
    let deviceSelected = this.devices.find(x => x.modelSerialNumber === selectedSerialNumber);
    this.deviceForm.patchValue({
      model: {
        name: deviceSelected.name,
        companyName: deviceSelected.producer
      }
    });
  }

  navigateToDevicesList(): void {
    this.router.navigate(['/devices-list']);
  }

  shouldShowError(controlName: string): boolean {
    const control = this.deviceForm.get(controlName);
    return control!.invalid && (control!.touched || this.submitted);
  }

  get measuredValues() {
    return this.deviceForm.get('model.measuredValues') as FormArray;
  }

  shouldShowErrorById(id: string): boolean {
    const control = document.getElementById(id) as any;
    return !control.validity.valid;
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

  private addNewRangeFG(): FormGroup {
    return this.fb.group({
      range:'',
      accuracyInPercent:''
    })
  }

  private addNewMeasuredValueFG(): FormGroup {
    return this.fb.group({
      physicalMagnitudeName: ['', Validators.required],
      physicalMagnitudeUnit: '',
      ranges: this.fb.array([])
    })
  }

  private bindOldValues() {
  this.deviceForm.patchValue({
    identificationNumber: this.deviceToEdit.deviceIdentificationNumber || '',
    productionDate: this.deviceToEdit.productionDate?.split('T')[0] || '',
    lastCalibrationDate: this.deviceToEdit.lastCalibrationDate ? this.deviceToEdit.lastCalibrationDate.split('T')[0] : '',    calibrationPeriodInYears: this.deviceToEdit.calibrationPeriodInYears || '',
    nextCalibrationDate: this.deviceToEdit.nextCalibrationDate ? this.deviceToEdit.nextCalibrationDate.split('T')[0] : '',
    isCalibrationCloseToExpire: this.deviceToEdit.isCalibrationCloseToExpire ?? null,
    storageLocation: this.deviceToEdit.storageLocation || '',
    model: {
      name: this.deviceToEdit.model || '',
      serialNumber: this.deviceToEdit.serialNumber || '',
      companyName: this.deviceToEdit.producer || ''
    }
  });
  this.deviceForm.markAsPristine();
  this.deviceForm.updateValueAndValidity();
}
}
