import { ChatService } from './../chat-service';
// src/app/chat/chat.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-chat',
  templateUrl: './chat-component.html',
  styleUrls: ['./chat-component.scss']
})
export class ChatComponent implements OnInit, OnDestroy {
  publicMessages: any[] = [];
  privateMessages: any[] = [];
  text = '';
  recipient = ''; // vazio = público
  subs: Subscription[] = [];

  constructor(private chat: ChatService, private http: HttpClient) {}

  async ngOnInit() {
    await this.chat.connect();

    this.subs.push(this.chat.messages$.subscribe(m => this.publicMessages.push(m)));
    this.subs.push(this.chat.privateMessages$.subscribe(m => this.privateMessages.push(m)));

    this.http.get<any[]>('http://localhost:5000/api/messages/public')
      .subscribe(ms => this.publicMessages = ms.map(x => ({ sender: x.sender, content: x.content, at: x.sentAt })));
  }

  ngOnDestroy() { this.subs.forEach(s => s.unsubscribe()); }

  send() {
    if (!this.text.trim()) return;
    if (this.recipient.trim()) this.chat.sendPrivate(this.recipient.trim(), this.text.trim());
    else this.chat.sendPublic(this.text.trim());
    this.text = '';
  }
}