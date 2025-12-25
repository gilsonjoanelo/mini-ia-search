// src/app/chat/chat.service.ts
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from '../auth/auth-service';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private hub!: signalR.HubConnection;
  public messages$ = new Subject<{ sender: string; content: string; at: string }>();
  public privateMessages$ = new Subject<{ sender: string; content: string; at: string }>();

  constructor(private auth: AuthService) {}

  connect() {
    const token = this.auth.getToken()!;
    this.hub = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/chat', { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    this.hub.on('ReceiveMessage', (sender, content, at) => {
      this.messages$.next({ sender, content, at });
    });

    this.hub.on('ReceivePrivateMessage', (sender, content, at) => {
      this.privateMessages$.next({ sender, content, at });
    });

    return this.hub.start();
  }

  sendPublic(content: string) {
    return this.hub.invoke('SendToAll', content);
  }

  sendPrivate(recipient: string, content: string) {
    return this.hub.invoke('SendToUser', recipient, content);
  }
}