import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth-service.service';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {
  userInfo: { username: string, email: string, isAdmin: boolean } | undefined;
  showModal: boolean = false;
  showRegisterModal: boolean = false;

  oldPassword: string = '';
  newPassword: string = '';
  confirmPassword: string = '';
  passwordError: string | null = null;
  passwordMismatch: boolean = false;

  registerEmail: string = '';
  registerUsername: string = '';
  registerPassword: string = '';
  isAdmin: boolean = false;
  isCurrentUserAdmin: boolean = false;
  currentUserUsername: string = '';
  confirmRegisterPassword: string = '';
  emailError: string | null = null;
  usernameError: string | null = null;
  registerPasswordError: string | null = null;
  registerPasswordMismatch: boolean = false;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.getUserInfo();
  }

  openChangePasswordModal(): void {
    this.showModal = true;
  }

  closeChangePasswordModal(): void {
    this.showModal = false;
    this.clearModalFields();
  }

  openRegisterModal(): void {
    this.showRegisterModal = true;
  }

  closeRegisterModal(): void {
    this.showRegisterModal = false;
    this.clearRegisterFields();
  }

  changePassword(): void {
    this.passwordMismatch = this.newPassword !== this.confirmPassword;

    if (this.passwordMismatch) {
      return;
    }

    if (!this.validatePassword(this.newPassword)) {
      return;
    }

    this.authService.changePassword(this.oldPassword, this.newPassword)
      .subscribe({
        next: () => {
          alert('Hasło zostało zmienione');
          this.closeChangePasswordModal();
        },
        error: (err) => {
          console.error('Error changing password', err);
          this.passwordError = 'Błąd przy zmianie hasła';
        }
      });
  }

  register(): void {
    this.registerPasswordMismatch = this.registerPassword !== this.confirmRegisterPassword;

    if (this.registerPasswordMismatch) {
      return;
    }

    if (!this.validateEmail(this.registerEmail)) {
      this.emailError = 'Niepoprawny format e-mail.';
      return;
    }
    if (!this.validateUsername(this.registerUsername)) {
      this.usernameError = 'Nazwa użytkownika musi mieć co najmniej 5 znaków.';
      return;
    }

    if (!this.validatePassword(this.registerPassword)) {
      return;
    }

    this.authService.register(this.registerEmail, this.registerUsername, this.registerPassword, this.isAdmin)
      .subscribe({
        next: () => {
          alert('Nowy użytkownik został dodany');
          this.authService.loadUsersList();
          this.closeRegisterModal();
        },
        error: (err: any) => {
          console.error('Error registering user', err);
        }
      });
  }

  validatePassword(password: string): boolean {
    if (password.length < 6) {
      this.passwordError = 'Hasło musi mieć co najmniej 6 znaków.';
      return false;
    }
    if (!/[A-Z]/.test(password)) {
      this.passwordError = 'Hasło musi zawierać co najmniej jedną dużą literę.';
      return false;
    }
    if (!/[0-9]/.test(password)) {
      this.passwordError = 'Hasło musi zawierać co najmniej jedną cyfrę.';
      return false;
    }
    if (!/[^a-zA-Z0-9]/.test(password)) {
      this.passwordError = 'Hasło musi zawierać co najmniej jeden znak specjalny.';
      return false;
    }
    this.passwordError = null;
    return true;
  }

  validateEmail(email: string): boolean {
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailPattern.test(email);
  }

  validateUsername(username: string): boolean {
    return username.length >= 5;
  }

  private clearModalFields(): void {
    this.oldPassword = '';
    this.newPassword = '';
    this.confirmPassword = '';
    this.passwordError = null;
    this.passwordMismatch = false;
  }

  private clearRegisterFields(): void {
    this.registerEmail = '';
    this.registerUsername = '';
    this.registerPassword = '';
    this.confirmRegisterPassword = '';
    this.emailError = null;
    this.usernameError = null;
    this.registerPasswordError = null;
    this.registerPasswordMismatch = false;
  }

  private getUserInfo(): void {
    this.authService.getUserInfo().subscribe({
      next: (data) => {
        this.userInfo = data;
        this.isCurrentUserAdmin = data.isAdmin;
        this.currentUserUsername = data.username;
      },
      error: (err) => {
        console.error('Error fetching user info', err);
      }
    });
  }
}
