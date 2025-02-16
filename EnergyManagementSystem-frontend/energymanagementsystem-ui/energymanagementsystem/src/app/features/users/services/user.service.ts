import { Injectable } from '@angular/core';
import { AddUser } from '../models/add-user.model';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user.model';
import { CookieService } from 'ngx-cookie-service';
import { UpdateUser } from '../models/update-user.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = environment.backendUserUrl; 

  constructor(private http: HttpClient, private cookieService: CookieService) { }

  createUser(data: AddUser): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/user`, data, {
      headers: {
        'Authorization': this.cookieService.get('Authorization')
      }
    });
  }

  getAllUsers(): Observable<User[]> {
    console.log('API Base URL:', this.baseUrl);

    return this.http.get<User[]>(`${this.baseUrl}/user`, {
      headers: {
        'Authorization': this.cookieService.get('Authorization')
      }
    });
  }

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/user/${id}`, {
      headers: {
        'Authorization': this.cookieService.get('Authorization')
      }
    });
  }  

  updateUser(id: string, data: UpdateUser): Observable<User> {
    return this.http.put<User>(`${this.baseUrl}/user/${id}`, data, {
      headers: {
        'Authorization': this.cookieService.get('Authorization')
      }
    });
  }

  deleteUser(id: string): Observable<User> {
    return this.http.delete<User>(`${this.baseUrl}/user/${id}`, {
      headers: {
        'Authorization': this.cookieService.get('Authorization')
      }
    });
  }
}
