import { AuthService } from './../auth-service';
// src/app/auth/login.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login-component.html',
  styleUrls: ['./login-component.scss']
})
export class LoginComponent {
  username = '';
  password = '';
  isRegister = false;
  error = '';

  constructor(private auth: AuthService, private router: Router) {}

  submit() {
    const req = this.isRegister
      ? this.auth.register(this.username, this.password)
      : this.auth.login(this.username, this.password);

    req.subscribe({
      next: res => { this.auth.setToken(res.token); this.router.navigate(['/chat']); },
      error: err => { this.error = err.error || 'Falha na autenticação'; }
    });
  }
}