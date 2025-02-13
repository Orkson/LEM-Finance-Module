import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ApiServiceService } from 'src/app/api-service.service';

@Component({
  selector: 'app-remove-device-warning-modal',
  templateUrl: './remove-device-warning-modal.component.html',
  styleUrls: ['./remove-device-warning-modal.component.css']
})
export class RemoveDeviceWarningModalComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: { deviceId: number },
    public dialogRef: MatDialogRef<RemoveDeviceWarningModalComponent>,
    public matDialog: MatDialog,
    private apiService: ApiServiceService,
  ){}

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onDeleteDevice(): void {
    this.apiService.removeDevice(this.data.deviceId).subscribe(response => {
      if (response) {
        alert('Urządzenie zostało usunięte');
        this.dialogRef.close(true);
      } else {
        alert('Wystąpił błąd przy usuwaniu urządzenia');
        this.dialogRef.close(false);
      }
    });
  }
}
