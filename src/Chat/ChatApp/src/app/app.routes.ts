import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login-component/login-component';
import { ChatComponent } from './chat/chat-component/chat-component';
import { AuthGuard } from './auth/auth-guard';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'chat', component: ChatComponent, canActivate: [AuthGuard] },
    { path: '', redirectTo: 'chat', pathMatch: 'full' }
];
