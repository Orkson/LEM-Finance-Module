import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef} from '@angular/material/dialog';
import { ApiServiceService } from 'src/app/api-service.service';
import { RemoveDeviceWarningModalComponent } from './remove-device-warning-modal/remove-device-warning-modal.component';
import { DialogRef } from '@angular/cdk/dialog';
import { ModelDetailsComponent } from './model-details/model-details/model-details.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-device-details',
  templateUrl: './device-details.component.html',
  styleUrls: ['./device-details.component.css']
})
export class DeviceDetailsComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { deviceDto: any },
    private dialog: MatDialog,
    private selfDialog: MatDialogRef<DeviceDetailsComponent>,
    private apiService: ApiServiceService,
    private router: Router
  ){}

  shouldDisplayMeasuredValuesTable = false;
  deviceDto = this.data.deviceDto;
  rowspan = 1;
  deviceDocuments = this.deviceDto.deviceDocuments;
  modelDocuments = this.deviceDto.modelDocuments;
  relatedModels = this.deviceDto.relatedModels;

  ngOnInit(): void {
    this.setDisplayMeasuredValuesTable();
  }

  onDeleteDevice(): void {
    const dialogRef = this.dialog.open(RemoveDeviceWarningModalComponent, {
      data: { deviceId: this.data.deviceDto.deviceId },
      autoFocus: false
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.selfDialog.close(true);
      }
    });
  }

  onClose(): void {
    this.selfDialog.close();
  }

  setRowspan(measuredValue: any): number {
    return measuredValue.measuringRanges.length;
  }

  setDisplayMeasuredValuesTable() {
    this.shouldDisplayMeasuredValuesTable = this.deviceDto.measuredValues.length > 0;
  }

  getModelDocuemnts() {
    this.modelDocuments = this.deviceDto.modelDocuments;
    let modelDocumentsNames: string[] = [];
    this.modelDocuments.forEach((x: any) => {
      modelDocumentsNames.push(x.name)
    });
    return modelDocumentsNames.join(',');
  }

  getRelatedDeviceName(relatedModel: any): string {
    let relatedModelName = relatedModel.name;
    return relatedModelName;
  }

  downloadFile(file: any, event: Event, downloadFor: string) {
    const deviceWord = "device";
    const modelWord = "model";

    event.preventDefault();

    let documentName: string = file.name;
    let modelId: string = this.deviceDto.modelId;
    let deviceId: string = this.deviceDto.deviceId;

    if (downloadFor === deviceWord) {
      modelId = "";
    } else if( downloadFor === modelWord) {
      deviceId = "";
    }

    this.apiService.downloadFile(documentName, modelId, deviceId).subscribe(
      (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = documentName;
        a.style.display = 'none';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      },
      error => {
        console.error('Error downloading file:', error);
      }
    );
  }

  openModelDetails(modelId: number, modelName: string, event: Event) {
    event.preventDefault();

    this.apiService.getModelDetails(modelId, modelName).subscribe((modelDetails: any) => {
      this.dialog.open(ModelDetailsComponent, {data: {modelDetails: modelDetails}, autoFocus: false});
    })
  }

  navigateToEdit(): any {
    this.router.navigate(['edit-device'], { state: { data: this.deviceDto } })
    this.dialog.closeAll();
  }

}


