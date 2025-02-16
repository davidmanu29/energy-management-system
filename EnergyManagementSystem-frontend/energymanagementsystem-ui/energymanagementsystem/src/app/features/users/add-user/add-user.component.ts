import { Component } from '@angular/core';
import { AddUser } from '../models/add-user.model';
import { Router } from '@angular/router';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.css']
})
export class AddUserComponent {

  model:AddUser;

  constructor(private userService:UserService, private router:Router){
    this.model = {
      name: '',
      username: '',
      password: '',
      isAdmin: false
    };
  }

  onFormSubmit():void{
    this.userService.createUser(this.model)
    .subscribe({
      next:(response) => {
        this.router.navigateByUrl('/admin/users');
      }
    });
  }
}
