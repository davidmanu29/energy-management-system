import { Injectable } from '@angular/core';
import { DeviceDto } from '../models/devicedto';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Device } from '../models/device';
import { CookieService } from 'ngx-cookie-service';
import { UpdateDeviceRequest } from '../models/update-device-request.model';
import { HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../auth/services/auth.service';
import { User } from '../../users/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {

  constructor(private http: HttpClient,
    private cookieService: CookieService,
    private authService: AuthService 
  ) { }

  addDevice(model: DeviceDto):Observable<void> {
    return this.http.post<void>('http://localhost/devicems/api/device', model, {
      headers:{
        'Authorization': this.cookieService.get('Authorization')
      }
    });
  }

  getAllDevices(): Observable<Device[]>{
    return this.http.get<Device[]>('http://localhost/devicems/api/device');
  }

  getDeviceById(id : string): Observable<Device>{
    return this.http.get<Device>(`http://localhost/devicems/api/device/${id}`);
  }

  updateDevice(id: string, updateDeviceRequest: UpdateDeviceRequest): Observable<Device>{
    const token = this.authService.getToken(); 

    if (!token) {
      console.error('Authorization token is missing');
      throw new Error('Authorization token not found');
    }

    const headers = new HttpHeaders({
      Authorization: token
    });

    return this.http.put<Device>(`http://localhost/devicems/api/device/${id}`, updateDeviceRequest, { headers });
  }

  deleteDevice(id: string): Observable<Device>{
    const token = this.authService.getToken(); 

    if (!token) {
      console.error('Authorization token is missing');
      throw new Error('Authorization token not found');
    }

    const headers = new HttpHeaders({
      Authorization: token
    });

    return this.http.delete<Device>(`http://localhost/devicems/api/device/${id}`, { headers });
  }

  getAllUserDevices(userId: string): Observable<Device[]>{
    return this.http.get<Device[]>(`http://localhost/devicems/api/device/user/${userId}`,{
      headers:{
        'Authorization': this.cookieService.get('Authorization')
      }
    });
  }

  getUserByUsername(username: string): Observable<User> {
    const url = `http://localhost/userms/api/user/username/${username}`;
    return this.http.get<User>(url, {
      headers:{
        'Authorization': this.cookieService.get('Authorization')
      }
    });
  } 
}
