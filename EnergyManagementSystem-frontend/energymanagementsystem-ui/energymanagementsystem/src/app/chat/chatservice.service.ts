import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;

  // Observables to push new messages / updated user lists
  private messageReceivedSource = new BehaviorSubject<{ user: string; message: string } | null>(null);
  messageReceived$ = this.messageReceivedSource.asObservable();

  private usersInRoomSource = new BehaviorSubject<string[]>([]);
  usersInRoom$ = this.usersInRoomSource.asObservable();

  private typingIndicatorSource = new BehaviorSubject<string | null>(null);
  typingIndicator$ = this.typingIndicatorSource.asObservable();

  private readReceiptSource = new BehaviorSubject<string | null>(null);
  readReceipt$ = this.readReceiptSource.asObservable();

  constructor() {}

  public startConnection(userName: string, roomName: string): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost/chatms/chat')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection started');
        this.joinRoom(userName, roomName);
      })
      .catch(err => console.error('Error while starting SignalR connection:', err));

    this.hubConnection.on('ReceiveMessage', (user: string, message: string) => {
      this.messageReceivedSource.next({ user, message });
    });

    this.hubConnection.on('UsersInRoom', (users: string[]) => {
      this.usersInRoomSource.next(users);
    });

    this.hubConnection.on('TypingIndicator', (sender: string) => {
      this.typingIndicatorSource.next(sender);
    });

    this.hubConnection.on('MessageRead', (recipient: string) => {
      this.readReceiptSource.next(recipient);
    });

  }

  public joinRoom(user: string, room: string): void {
    this.hubConnection.invoke('JoinRoom', { user, room })
      .catch(err => console.error(err));
  }

  public sendMessage(message: string): void {
    this.hubConnection.invoke('SendMessage', message)
      .catch(err => console.error(err));
  }

  public sendTypingIndicator(sender: string, recipient: string): void {
    this.hubConnection.invoke('SendTypingIndicator', sender, recipient)
      .catch(err => console.error(err));
  }

  public sendReadReceipt(recipient: string): void {
    this.hubConnection.invoke('SendReadReceipt', recipient)
      .catch(err => console.error(err));
  }
}
