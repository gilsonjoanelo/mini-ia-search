import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { AuthService } from '../auth/auth.service';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ChatService {  private hub!: signalR.HubConnection;

  public message$ = new Subject<any>();
  public status$ = new Subject<{ conversationId: number; messageId: number; status: string }>();
  public typing$ = new Subject<{ conversationId: number; username: string; typing: boolean }>();
  public presence$ = new Subject<{ username: string; online: boolean; at: string }>();

  constructor(private auth: AuthService) {}

  async connect() {
    const token = this.auth.getToken()!;
    this.hub = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/hubs/chat', { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    this.hub.on('MessageReceived', (conversationId, message) => this.message$.next({ conversationId, ...message }));
    this.hub.on('MessageStatusChanged', (conversationId, messageId, status) => this.status$.next({ conversationId, messageId, status }));
    this.hub.on('Typing', (conversationId, username, typing) => this.typing$.next({ conversationId, username, typing }));
    this.hub.on('PresenceChanged', (username, online, at) => this.presence$.next({ username, online, at }));

    await this.hub.start();
  }

  sendMessage(conversationId: number, content: string, mediaUrl?: string) {
    return this.hub.invoke('SendMessage', conversationId, content, mediaUrl);
  }

  startTyping(conversationId: number) { return this.hub.invoke('StartTyping', conversationId); }
  stopTyping(conversationId: number) { return this.hub.invoke('StopTyping', conversationId); }
  markDelivered(conversationId: number, messageId: number) { return this.hub.invoke('MarkDelivered', conversationId, messageId); }
  markRead(conversationId: number) { return this.hub.invoke('MarkRead', conversationId); }
}
