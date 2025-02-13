import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogContent } from '@angular/material/dialog';
import { ApiServiceService } from 'src/app/api-service.service';
import { DialogRef } from '@angular/cdk/dialog';

@Component({
  selector: 'app-model-details',
  templateUrl: './model-details.component.html',
  styleUrl: './model-details.component.css'
})
export class ModelDetailsComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { modelDetails: any },
    private dialog: MatDialog,
    private selfDialog: DialogRef<ModelDetailsComponent>,
    private apiService: ApiServiceService
  ){}

  shouldDisplayMeasuredValuesTable = false;
  modelDetials = this.data.modelDetails;
  modelDocuments = this.modelDetials.modelDocuments;
  relatedModels = this.modelDetials.relatedModelsDetails;


  ngOnInit(): void {
    this.setDisplayMeasuredValuesTable();
  }

  onClose(): void {
    this.selfDialog.close();
  }

  getModelDocuemnts() {
    this.modelDocuments = this.modelDetials.modelDocuments;
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

    event.preventDefault();

    let documentName: string = file.name;
    let modelId: string = this.modelDetials.id;

    this.apiService.downloadFile(documentName, modelId, '').subscribe(
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
        // Handle error as needed
      }
    );
  }

  private setDisplayMeasuredValuesTable() {
    this.shouldDisplayMeasuredValuesTable = this.modelDetials.measuredValues.length > 0;
  }
}
