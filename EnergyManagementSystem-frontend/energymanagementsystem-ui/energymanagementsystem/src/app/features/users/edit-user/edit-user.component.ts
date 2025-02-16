import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { UserService } from '../services/user.service';
import { User } from '../models/user.model';
import { UpdateUser } from '../models/update-user.model';

@Component({
  selector: 'app-edit-user',
  templateUrl: './edit-user.component.html',
  styleUrls: ['./edit-user.component.css']
})
export class EditUserComponent implements OnInit, OnDestroy {

  userid: string | null = null;
  routeSubscription?: Subscription;
  updateUserSubscription?: Subscription;
  getUserSubscription?: Subscription;
  deleteUserSubscription?: Subscription;
  model?: User;

  constructor(private route: ActivatedRoute,
    private userService: UserService,
    private router: Router
  ){

  }
  ngOnInit(): void {
    this.routeSubscription = this.route.paramMap.subscribe({
      next:(params) => {
        this.userid = params.get('userid');
        
        if(this.userid){
          this.getUserSubscription = this.userService.getUserById(this.userid)
           .subscribe({
              next: (response) => {
                this.model = response;
              }
            });
        }
      }
    });
  }

  onFormSubmit():void{
    if(this.model && this.userid){
      var updateUser: UpdateUser = {
        name: this.model.name,
        username: this.model.username,
        password:  this.model.password,
        isAdmin: this.model.isAdmin
      };

      this.updateUserSubscription = this.userService.updateUser(this.userid, updateUser).subscribe({
        next:(response) => {
          this.router.navigateByUrl('/admin/users');
        }
      });
    }
  }

  onDelete():void{
    if(this.userid){
      this.deleteUserSubscription = this.userService.deleteUser(this.userid).subscribe({
        next:(response) => {
          this.router.navigateByUrl('/admin/users');
        }
      });
    }
  }

  ngOnDestroy(): void {
    this.routeSubscription?.unsubscribe();
    this.updateUserSubscription?.unsubscribe();
    this.getUserSubscription?.unsubscribe();
    this.deleteUserSubscription?.unsubscribe();
  }
}
