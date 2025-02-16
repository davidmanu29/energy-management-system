import { Component, OnDestroy, OnInit } from '@angular/core';
import { DeviceService } from '../../devices/services/device.service';
import { Device } from '../../devices/models/device';
import { Observable, Subscription } from 'rxjs';
import { AuthService } from '../../auth/services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy{

  devices$?: Observable<Device[]>;
  private userSubscription$?: Subscription;

  constructor(private deviceService: DeviceService,
    private authService:AuthService
  ){

  }
  ngOnDestroy(): void {
    this.userSubscription$?.unsubscribe();
  }

  ngOnInit(): void {
    this.userSubscription$ = this.authService.$user.subscribe(user => {
      if (user) {
        const username = user.username;
        this.deviceService.getUserByUsername(username).subscribe(
          (user) => {
            const userId = user.id;
            this.devices$ = this.deviceService.getAllUserDevices(userId);
          },
          (error) => console.error('Error fetching user by username:', error)
        );
      } else {
        this.devices$ = undefined;
      }
    });
  }




}
