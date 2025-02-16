import { Component, OnInit } from '@angular/core';
import { UserService } from '../services/user.service';
import { Observable } from 'rxjs';
import { User } from '../models/user.model';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.css']
})
export class UsersListComponent implements OnInit {

  users$?: Observable<User[]>

  constructor(private userService:UserService) {

  }

  ngOnInit(): void {
    this.users$ = this.userService.getAllUsers();
  }

}
