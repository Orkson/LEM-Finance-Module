import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth-service.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(private authService: AuthService, private router: Router) {}

  login(): void {
    this.authService.login(this.username, this.password).subscribe({
      next: (token) => {
        this.authService.setToken(token);
        this.router.navigate(['/devices-list']);
      },
      error: (err) => {
        this.errorMessage = 'Nieprawidłowa nazwa użytkownika lub hasło';
      }
    });
  }
}
