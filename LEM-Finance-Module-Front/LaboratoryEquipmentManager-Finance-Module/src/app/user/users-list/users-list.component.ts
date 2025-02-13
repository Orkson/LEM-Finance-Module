import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { AuthService } from 'src/app/auth-service.service';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.css'
})
export class UsersListComponent implements OnInit {
  @Input() currentUserUsername: string;
  users: any[] = [];
  usersToDisplay: any[] = [];

  constructor(private authService: AuthService){}

  ngOnInit(): void {
    this.authService.users$.subscribe(users => {
      this.users = users;
    });

    this.authService.loadUsersList();
  }

  deleteUser(username: string): void {
    if (confirm(`Czy na pewno chcesz usunąć użytkownika ${username}?`)) {
      this.authService.deleteUserByUsername(username).subscribe({
        next: () => {
          alert('Użytkownik został usunięty');
          this.authService.loadUsersList();
        },
        error: (err) => {
          console.error('Error deleting user:', err);
          alert('Wystąpił błąd podczas usuwania użytkownika');
        }
      });
    }
  }

  isCurrentUser(username: string) : boolean {
    return username === this.currentUserUsername
  }
}
