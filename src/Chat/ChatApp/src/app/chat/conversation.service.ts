import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ConversationService {
  private baseUrl = 'http://localhost:5000/api/conversations';
  constructor(private http: HttpClient) {}
  list() { return this.http.get<any[]>(this.baseUrl); }
  create(otherUsername: string) { return this.http.post<{ id: number }>(this.baseUrl, { otherUsername }); }
  messages(conversationId: number, page = 1, pageSize = 30) {
    return this.http.get<any[]>(`${this.baseUrl}/${conversationId}/messages`, { params: { page, pageSize } });
  }
}
