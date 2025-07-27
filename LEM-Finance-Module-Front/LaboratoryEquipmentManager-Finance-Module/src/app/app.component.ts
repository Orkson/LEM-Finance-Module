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
  let initialRedirectDone = false;

  this.router.events.subscribe(event => {
    if (event instanceof NavigationStart) {
      const targetUrl = event.url;

      this.authService.isAuthenticated().subscribe(isAuthenticated => {
        this.isLoggedIn = isAuthenticated;

        if (initialRedirectDone) return;

        if (isAuthenticated) {
          if (targetUrl === '/login' || targetUrl === '/register') {
            this.router.navigate(['/devices-list']);
          }
        } else {
          if (targetUrl !== '/login' && targetUrl !== '/register') {
            this.router.navigate(['/login']);
          }
        }

        initialRedirectDone = true;
      });
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

  navigateToExpensesPlannerList() {
    this.router.navigate(['/expenses-planner']);
  }

  navigateToNewExpensesPlanner() {
    this.router.navigate(['/expenses-planner-form']);
  }

  logOut() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
