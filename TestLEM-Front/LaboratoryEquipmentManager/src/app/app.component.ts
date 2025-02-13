import { Component, HostListener, OnInit } from '@angular/core';
import { NavigationStart, Router } from '@angular/router';
import { AuthService } from './auth-service.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  private inactivityTime: any; // Timer do inaktywności
  private timeoutDuration: number = 2 * 60 * 1000; // 5 minut
  isLoggedIn: boolean = false;

  constructor(private authService: AuthService,
    private router: Router
  ){}

  ngOnInit(): void {
    this.authService.isAuthenticated().subscribe(isAuthenticated => {
      this.isLoggedIn = isAuthenticated;
      if (isAuthenticated) {
        this.router.navigate(['/devices-list']);
      } else {
        this.router.navigate(['/login']);
      }
    });

    window.addEventListener('beforeunload', () => {
      this.authService.logout();
    });
  }
  title = 'LaboratoryEquipmentManager';

  navigateToProfile(){
    this.router.navigate(['/user']);
  }

  navigateToDevicesList() {
    this.router.navigate(['/devices-list']);
  }

  logOut() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
