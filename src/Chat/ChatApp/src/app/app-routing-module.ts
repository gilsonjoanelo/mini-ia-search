import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { ChatComponent } from './chat/chat/chat.component';
import { AuthGuard } from './auth/auth.guard';
import { ConversationListComponent } from './chat/conversation-list/conversation-list.component';
import { environment } from '../environments/environment';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'conversations', component: ConversationListComponent, canActivate: [AuthGuard] },
  { path: 'chat/:id', component: ChatComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: 'conversations', pathMatch: 'full' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes/*, { enableTracing: !environment.production }*/)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
