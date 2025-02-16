import { Component, OnInit } from '@angular/core';
import { DeviceService } from '../services/device.service';
import { Device } from '../models/device';
import { Router } from '@angular/router';

@Component({
  selector: 'app-devices-list',
  templateUrl: './devices-list.component.html',
  styleUrls: ['./devices-list.component.css']
})
export class DevicesListComponent implements OnInit {

  devices? : Device[];

  constructor(private deviceService: DeviceService,
    private router: Router
  ) { 

  }
  ngOnInit(): void {
    this.deviceService.getAllDevices()
    .subscribe({
      next:(response) => {
        this.devices = response;
      }
    });
  }

}
