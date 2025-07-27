import { Component } from '@angular/core';
import { AuthService } from '../auth-service.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  username: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';
  isAdmin: boolean = false;
  errorMessage: string = '';
  passwordMismatch: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  register(): void {
    this.passwordMismatch = this.password !== this.confirmPassword;

    if (this.passwordMismatch) {
      this.errorMessage = 'Hasła nie pasują!';
      return;
    }

    this.authService.register(this.email, this.username, this.password, this.isAdmin)
      .subscribe({
        next: () => {
          alert('Rejestracja zakończona pomyślnie. Zaloguj się.');
          this.authService.logout();
          this.router.navigate(['/login']);
        },
        error: (err) => {
          this.errorMessage = 'Błąd przy rejestracji użytkownika.';
        }
      });
  }
}
