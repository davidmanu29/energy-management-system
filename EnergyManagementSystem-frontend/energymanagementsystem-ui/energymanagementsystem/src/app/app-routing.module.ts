import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DevicesListComponent } from './features/devices/devices-list/devices-list.component';
import { AddDeviceComponent } from './features/devices/add-device/add-device.component';
import { LoginComponent } from './features/auth/login/login.component';
import { EditDeviceComponent } from './features/devices/edit-device/edit-device.component';
import { UsersListComponent } from './features/users/users-list/users-list.component';
import { AddUserComponent } from './features/users/add-user/add-user.component';
import { EditUserComponent } from './features/users/edit-user/edit-user.component';
import { HomeComponent } from './features/public/home/home.component';
import { authGuard } from './features/auth/guards/auth.guard';
import { ChatcomponentComponent } from './chat/chatcomponent/chatcomponent.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path:'admin/devices',
    component: DevicesListComponent,
    canActivate:[authGuard]
  },
  {
    path:'admin/devices/add',
    component: AddDeviceComponent,
    canActivate:[authGuard]
  },
  {
    path:'login',
    component: LoginComponent
  },
  {
    path: 'admin/devices/:deviceid',
    component: EditDeviceComponent,
    canActivate:[authGuard]
  },
  {
    path: 'admin/users',
    component: UsersListComponent,
    canActivate:[authGuard]
  },
  {
    path: 'admin/users/add',
    component: AddUserComponent,
    canActivate:[authGuard]
  },
  {
    path: 'admin/users/:userid',
    component: EditUserComponent,
    canActivate:[authGuard]
  },

  {
    path: 'chat',
    component: ChatcomponentComponent,
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
