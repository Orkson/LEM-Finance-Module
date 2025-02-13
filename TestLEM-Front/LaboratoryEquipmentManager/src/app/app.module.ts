import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DevicesListComponent } from './devices-list/devices-list.component';
import { HttpClientModule } from '@angular/common/http';
import { DeviceDetailsComponent } from './devices-list/device-details/device-details.component';
import { AddDeviceComponent } from './add-device/add-device.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { RemoveDeviceWarningModalComponent } from './devices-list/device-details/remove-device-warning-modal/remove-device-warning-modal.component';
import { UserComponent } from './user/user.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { PolishPaginatorIntl } from './polish-paginator-inlt';
import { PercentageDirective } from './commons/directives/percentage.directive';
import { NgbModule, NgbTypeaheadModule } from '@ng-bootstrap/ng-bootstrap';
import { MultiSelectModule } from 'primeng/multiselect';
import { ModelDetailsComponent } from './devices-list/device-details/model-details/model-details/model-details.component';
import { EditDeviceComponent } from './edit-device/edit-device/edit-device.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { LoginComponent } from './login/login.component';
import { AuthService } from './auth-service.service';
import { UsersListComponent } from './user/users-list/users-list.component';

@NgModule({
  declarations: [
    AppComponent,
    DevicesListComponent,
    DeviceDetailsComponent,
    AddDeviceComponent,
    RemoveDeviceWarningModalComponent,
    UserComponent,
    PercentageDirective,
    ModelDetailsComponent,
    EditDeviceComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatSelectModule,
    BrowserAnimationsModule,
    MatPaginatorModule,
    FormsModule,
    NgbModule,
    NgbTypeaheadModule,
    MultiSelectModule,
    MatCheckboxModule,
    UsersListComponent
    ],
  providers: [
    { provide: MatPaginatorIntl, useClass: PolishPaginatorIntl }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
