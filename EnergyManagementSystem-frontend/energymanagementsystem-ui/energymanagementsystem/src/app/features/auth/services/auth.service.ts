import { Injectable } from '@angular/core';
import { LoginRequest } from '../models/login-request.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoginResponse } from '../models/login-response.model';
import { HttpClient } from '@angular/common/http';
import { User } from '../models/user.model';
import { CookieService } from 'ngx-cookie-service';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  $user = new BehaviorSubject<User | undefined>(undefined)

  constructor(private http: HttpClient,
    private cookie: CookieService 
  ) { }

  login(request: LoginRequest): Observable<LoginResponse>{
    return this.http.post<LoginResponse>("http://localhost/userms/account/login", {
      username: request.username,
      password: request.password
    });
  }

  logout():void{
    localStorage.clear();
    this.cookie.delete('Authorization', '/');
    this.$user.next(undefined);
  }
  
  setUser(user : User): void {
    this.$user.next(user);

    localStorage.setItem('user-username', user.username);
    localStorage.setItem('user-roles', user.roles.join(','));
 }

  user() : Observable<User | undefined>{
    return this.$user.asObservable();
  }

  getUser(): User | undefined {
    const username = localStorage.getItem('user-username');
    const roles = localStorage.getItem('user-roles');

    if(username && roles){
      const user: User = {
        username: username,
        roles: roles.split(',')
      };

      return user;
    }
    
    return undefined;
  }

  getToken(): string | null {
    return this.cookie.get('Authorization') || null;
  }

}
 