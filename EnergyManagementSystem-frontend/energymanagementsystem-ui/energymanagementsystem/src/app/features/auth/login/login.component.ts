import { Component } from '@angular/core';
import { LoginRequest } from '../models/login-request.model';
import { AuthService } from '../services/auth.service';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { CustomJwtPayload } from '../models/jwt-payload.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  model: LoginRequest;

  constructor(private authService: AuthService,
    private cookie: CookieService,
    private router: Router
  ){
    this.model = {
      username: '',
      password: ''
    };
  }

  onFormSubmit(): void{
    this.authService.login(this.model)
    .subscribe({
      next:(response) =>{
        this.cookie.set('Authorization', `Bearer ${response.token}`,
          undefined,'/', undefined, true, 'Strict');

          this.authService.setUser({
            username: response.username,
            roles: response.roles
          });

          console.log('Roles are:', response.roles.join(','));

          this.router.navigateByUrl('/');
      }
    });
  }

}
