import { Component, OnInit, OnDestroy } from '@angular/core';
import { firstValueFrom, Subscription } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ChatService } from '../chat.service';
import { ActivatedRoute } from '@angular/router';
import { ConversationService } from '../conversation.service';


@Component({
  selector: 'app-chat',
  standalone: false,
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss',
})
export class ChatComponent implements OnInit, OnDestroy {
  convId!: number;
  messages: any[] = [];
  text = '';
  isTyping = false;
  typingUsers = new Set<string>();
  page = 1;
  subs: Subscription[] = [];

  constructor(
    private route: ActivatedRoute,
    private conv: ConversationService,
    private chat: ChatService,
    private http: HttpClient
  ) {}

  async ngOnInit() {
    await this.chat.connect();

    this.convId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadPage();

    this.subs.push(this.chat.message$.subscribe(m => {
      if (m.conversationId === this.convId) {
        this.messages.push(m);
        this.chat.markDelivered(this.convId, m.id);
        // auto-scroll aqui, se desejar
      }
    }));

    this.subs.push(this.chat.status$.subscribe(s => {
      if (s.conversationId === this.convId) {
        const msg = this.messages.find(x => x.id === s.messageId);
        if (msg) msg.status = s.status;
      }
    }));

    this.subs.push(this.chat.typing$.subscribe(t => {
      if (t.conversationId === this.convId) {
        t.typing ? this.typingUsers.add(t.username) : this.typingUsers.delete(t.username);
      }
    }));

    // marca como lida ao abrir
    this.chat.markRead(this.convId);
  }

  ngOnDestroy() { this.subs.forEach(s => s.unsubscribe()); }

  loadPage() {
    this.conv.messages(this.convId, this.page).subscribe(ms => {
      this.messages = [...ms, ...this.messages];
      if (ms.length > 0) this.page += 1;
    });
  }

  send() {
    const content = this.text.trim();
    if (!content) return;
    this.chat.sendMessage(this.convId, content);
    this.text = '';
    this.chat.stopTyping(this.convId);
  }

  onInputChange() {
    if (!this.isTyping) {
      this.isTyping = true;
      this.chat.startTyping(this.convId);
      setTimeout(() => { this.isTyping = false; this.chat.stopTyping(this.convId); }, 1500);
    }
  }

  async uploadFile(event: any) {
    const file = event.target.files?.[0];
    if (!file) return;
    const form = new FormData();
    form.append('file', file);
    const res = await firstValueFrom(this.http.post<{ url: string }>('http://localhost:5000/api/media/upload', form));
    this.chat.sendMessage(this.convId, '', res.url);
  }

  get typingUsersString(): string {
    // Perform the array operation in the TypeScript code
    return [...this.typingUsers].join(', ');
  }

  // Or use a method
  getTypingUsers(): string {
    return [...this.typingUsers].join(', ');
  }
}
