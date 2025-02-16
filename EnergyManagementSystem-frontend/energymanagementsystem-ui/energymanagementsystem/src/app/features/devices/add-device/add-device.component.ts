import { Component, OnDestroy } from '@angular/core';
import { DeviceDto } from '../models/devicedto';
import { DeviceService } from '../services/device.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-device',
  templateUrl: './add-device.component.html',
  styleUrls: ['./add-device.component.css']
})
export class AddDeviceComponent implements OnDestroy{

  model: DeviceDto;
  private addDeviceSub? : Subscription;

  constructor(private deviceService: DeviceService, private router: Router){
    this.model = {
      description: '',
      address: '',
      maxConsumptionPerHour: 0,
      userId: ''
    }
  }
  
  onFormSubmit(){
    this.addDeviceSub = this.deviceService.addDevice(this.model)
    .subscribe({
      next: (response) => {
        console.log('Success!');
        this.router.navigateByUrl('/admin/devices');
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  ngOnDestroy(): void {
    this.addDeviceSub?.unsubscribe();
  }
}
