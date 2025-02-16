import { Component, OnInit } from '@angular/core';
import { ChatService } from '../chatservice.service';
import { AuthService } from 'src/app/features/auth/services/auth.service';

@Component({
  selector: 'app-chat',
  template: `
    <div class="container my-4">
      <div class="row justify-content-center">
        <div class="col-md-8">

          <div class="d-flex justify-content-between align-items-center mb-3">
            <h2>Chat Room: {{ currentRoom }}</h2>
          </div>

          <div *ngIf="typingUser" class="alert alert-info py-2">
            <em>{{ typingUser }} is typing...</em>
          </div>

          <div 
            class="border p-3 mb-3 bg-light" 
            style="height: 300px; overflow-y: auto;">
            <ul class="list-unstyled mb-0">
              <li *ngFor="let msg of messages" class="mb-2">
                <strong>{{ msg.user }}:</strong> {{ msg.message }}
                <span *ngIf="wasMessageRead(msg)" class="text-success">
                  (read)
                </span>
              </li>
            </ul>
          </div>

          <div class="input-group mb-4">
            <input 
              type="text" 
              class="form-control" 
              [(ngModel)]="newMessage"
              name="newMessage"
              (keyup.enter)="sendMessage()"
              (keyup)="onTyping()"
              placeholder="Type a message..." />
            <button class="btn btn-primary" (click)="sendMessage()">Send</button>
          </div>

          <h4 class="mb-3">Users in {{ currentRoom }}:</h4>
          <ul class="list-group">
            <li 
              class="list-group-item list-group-item-action" 
              *ngFor="let user of onlineUsers" 
              (click)="startPrivateChat(user)">
              {{ user }}
            </li>
          </ul>
          <p class="mt-2 text-muted">
            <small>(Click a user name to start a private chat)</small>
          </p>

        </div>
      </div>
    </div>
  `,
  styles: [`
    .list-group-item-action {
      cursor: pointer;
    }
  `]
})

export class ChatcomponentComponent implements OnInit {
  newMessage = '';
  messages: { user: string; message: string }[] = [];
  onlineUsers: string[] = [];

  currentUser = this.authService.$user.getValue()?.username || 'Anonymous';      
  currentRoom = 'lobby';   

  typingUser: string | null = null;

  readMessages: string[] = [];

  constructor(
    private chatService: ChatService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.chatService.startConnection(this.currentUser, this.currentRoom);

    this.chatService.messageReceived$.subscribe(msg => {
      if (msg) {
        this.messages.push(msg);
    
        if (this.currentRoom.startsWith('private_')) {
          this.sendReadReceipt(msg.user); 
        }
      }
    });

    this.chatService.usersInRoom$.subscribe(users => {
      this.onlineUsers = users;
    });

    this.chatService.typingIndicator$.subscribe(sender => {
      if (sender && sender !== this.currentUser) {
        this.typingUser = sender; 
        setTimeout(() => {
          this.typingUser = null;
        }, 2000);
      }
    });

    this.chatService.readReceipt$.subscribe(recipient => {
      if (recipient === this.currentUser) {
        this.readMessages = [...this.messages.map(m => m.message)];
      }
    });
  }

  sendMessage(): void {
    if (this.newMessage.trim().length > 0) {
      this.chatService.sendMessage(this.newMessage);
      this.newMessage = '';
    }
  }

  onTyping() {
    if (this.currentRoom.startsWith('private_')) {
      const parts = this.currentRoom.replace('private_', '').split('_');
      const otherUser = (parts[0] === this.currentUser) ? parts[1] : parts[0];
      this.chatService.sendTypingIndicator(this.currentUser, otherUser);
    } else {
    }
  }

  sendReadReceipt(sender: string) {
    this.chatService.sendReadReceipt(sender);
  }

  startPrivateChat(otherUser: string) {
    const sorted = [this.currentUser, otherUser].sort().join('_');
    const privateRoomName = `private_${sorted}`;

    this.messages = [];
    this.currentRoom = privateRoomName;
    this.chatService.joinRoom(this.currentUser, privateRoomName);
  }

  wasMessageRead(msg: { user: string; message: string }): boolean {
    return this.readMessages.includes(msg.message);
  }
}
