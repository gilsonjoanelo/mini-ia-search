import "@angular/compiler";
import { Component, OnInit } from '@angular/core';
import { ConversationService } from '../conversation.service';
import { ChatService } from '../chat.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-conversation-list',
  standalone: false,
  templateUrl: './conversation-list.component.html',
  styleUrl: './conversation-list.component.scss',
})
@Component({ /* ... */ })
export class ConversationListComponent implements OnInit {
  conversations: any[] = [];
  presence: Record<string, { online: boolean; at: string }> = {};

  constructor(private conv: ConversationService, private chat: ChatService, private router: Router) {}

  async ngOnInit() {
    await this.chat.connect();
    this.conv.list().subscribe(list => this.conversations = list);

    this.chat.message$.subscribe(m => {
      const c = this.conversations.find(x => x.id === m.conversationId);
      if (c) { c.lastMessage = m.content; c.unreadCount = (c.unreadCount || 0) + 1; }
    });

    this.chat.presence$.subscribe(p => this.presence[p.username] = { online: p.online, at: p.at });
  }

  open(conv: any) { this.router.navigate(['/chat', conv.id]); }
}
