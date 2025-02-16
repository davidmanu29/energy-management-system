import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './core/components/navbar/navbar.component';
import { DevicesListComponent } from './features/devices/devices-list/devices-list.component';
import { AddDeviceComponent } from './features/devices/add-device/add-device.component';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { LoginComponent } from './features/auth/login/login.component';
import { EditDeviceComponent } from './features/devices/edit-device/edit-device.component';
import { UsersListComponent } from './features/users/users-list/users-list.component';
import { AddUserComponent } from './features/users/add-user/add-user.component';
import { EditUserComponent } from './features/users/edit-user/edit-user.component';
import { HomeComponent } from './features/public/home/home.component';
import { ChatcomponentComponent } from './chat/chatcomponent/chatcomponent.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    DevicesListComponent,
    AddDeviceComponent,
    LoginComponent,
    EditDeviceComponent,
    UsersListComponent,
    AddUserComponent,
    EditUserComponent,
    HomeComponent,
    ChatcomponentComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
