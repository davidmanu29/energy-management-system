import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { DeviceService } from '../services/device.service';
import { Device } from '../models/device';
import { UpdateDeviceRequest } from '../models/update-device-request.model';
import { HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-edit-device',
  templateUrl: './edit-device.component.html',
  styleUrls: ['./edit-device.component.css']
})
export class EditDeviceComponent implements OnInit, OnDestroy{

  deviceid : string | null = null;
  paramsSubscription?: Subscription; 
  editDeviceSubscription?: Subscription; 
  device?: Device

  constructor(private route: ActivatedRoute,
    private deviceService : DeviceService,
    private router: Router
  ){

  }

  ngOnDestroy(): void {
    this.paramsSubscription?.unsubscribe();
    this.editDeviceSubscription?.unsubscribe();
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe({
      next:(params) => {
        this.deviceid = params.get('deviceid');

        if(this.deviceid) {
          this.deviceService.getDeviceById(this.deviceid)
          .subscribe({
            next: (response) => {
              this.device = response
            }
          })
        }
      }
    });
  }

  onFormSubmit(): void {
    const updateDeviceRequest: UpdateDeviceRequest = {
      description: this.device?.description ?? "",
      address: this.device?.address ?? "",
      maxConsumptionPerHour: this.device?.maxConsumptionPerHour ?? 0
    }

    if(this.deviceid){
      this.editDeviceSubscription = this.deviceService.updateDevice(this.deviceid, updateDeviceRequest)
      .subscribe({
        next: (response) => {
          this.router.navigateByUrl('/admin/devices');
        }
      });
    }
  }

  onDelete():void{
    if(this.deviceid){
      this.deviceService.deleteDevice(this.deviceid)
      .subscribe({
        next:(response) => {
          this.router.navigateByUrl('/admin/devices');
        }
      });
    }
  }

}
