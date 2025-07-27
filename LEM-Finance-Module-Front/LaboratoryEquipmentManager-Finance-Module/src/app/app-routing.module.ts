import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DevicesListComponent } from './devices-list/devices-list.component';
import { AddDeviceComponent } from './add-device/add-device.component';
import { UserComponent } from './user/user.component';
import { EditDeviceComponent } from './edit-device/edit-device/edit-device.component';
import { LoginComponent } from './login/login.component';
import { AuthGuard } from './auth.guard';
import { ExpensePlannerFormComponent } from './expense-planner-form/expense-planner-form.component';
import { ExpensePlannerListComponent } from './expense-planner-list/expense-planner-list.component';
import { RegisterComponent } from './register/register.component';

const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'devices-list', component: DevicesListComponent, canActivate: [AuthGuard] },
  { path: 'add-device', component: AddDeviceComponent, canActivate: [AuthGuard] },
  { path: 'edit-device', component: EditDeviceComponent, canActivate: [AuthGuard]},
  { path: 'user', component: UserComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'expenses-planner', component: ExpensePlannerListComponent, canActivate: [AuthGuard] },
  { path: 'expenses-planner-form', component: ExpensePlannerFormComponent, canActivate: [AuthGuard] },
  { path: 'expenses-planner-form/:id', component: ExpensePlannerFormComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'expenses-planner' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
