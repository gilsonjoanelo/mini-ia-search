import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms'; // Import FormsModule
import { CommonModule } from '@angular/common'; // Import CommonModule

import { AppRoutingModule } from './app-routing-module';
import { MaterialModule } from './material.module';
import { App } from './app';
import { LoginComponent } from './auth/login/login.component';
import { ChatComponent } from './chat/chat/chat.component';
import { TokenInterceptor } from './core/token-interceptor';
import { ConversationListComponent } from './chat/conversation-list/conversation-list.component';

@NgModule({
  declarations: [
    App,
    LoginComponent,
    ChatComponent,
    ConversationListComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    CommonModule,
    AppRoutingModule,
    MaterialModule // importa todos os módulos do Material
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    { provide: HTTP_INTERCEPTORS, useClass: TokenInterceptor, multi: true }
  ],
  bootstrap: [App]
})
export class AppModule { }
