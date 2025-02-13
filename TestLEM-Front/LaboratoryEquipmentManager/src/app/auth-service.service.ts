import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private usersSubject = new BehaviorSubject<any[]>([]);
  public users$ = this.usersSubject.asObservable();
  private apiUrl = environment.apiUrl;
  private loggedInSubject = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) {}

  login(username: string, password: string): Observable<string> {
    return this.http.post<{ token: string }>(`${this.apiUrl}/login`, { username, password })
      .pipe(map(response => {
        this.setToken(response.token);
        this.loggedInSubject.next(true);
        return response.token;
      }));
  }

  setToken(token: string): void {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  private hasToken(): boolean {
    return this.getToken() !== null;
  }

  logout(): void {
    localStorage.removeItem('token');
    this.loggedInSubject.next(false);
  }

  isAuthenticated(): Observable<boolean> {
    return this.loggedInSubject.asObservable();
  }

  getUserInfo(): Observable<any> {
    const headers = this.createAuthorizationHeader();
    return this.http.get(`${this.apiUrl}/me`, { headers });
  }

  changePassword(currentPassword: string, newPassword: string): Observable<any> {
    const headers = this.createAuthorizationHeader();
    return this.http.post(`${this.apiUrl}/change-password`, { currentPassword, newPassword }, { headers });
  }

  changeUsername(newUsername: string): Observable<any> {
    const headers = this.createAuthorizationHeader();
    return this.http.post(`${this.apiUrl}/change-username`, { newUsername }, { headers });
  }

  register(email: string, username: string, password: string, isAdmin: boolean): Observable<any> {
    const headers = this.createAuthorizationHeader();
    const payload = { email, username, password, isAdmin };
    return this.http.post(`${this.apiUrl}/register`, payload, {headers});
  }

  getUsersList() {
    const headers = this.createAuthorizationHeader();
    return this.http.get(`${this.apiUrl}/users`, { headers });
  }

  loadUsersList(): void {
    this.getUsersList().subscribe((users : any) => {
      this.usersSubject.next(users);
    });
  }

  deleteUserByUsername(username: string): Observable<any> {
    const headers = this.createAuthorizationHeader();
    return this.http.delete(`${this.apiUrl}/user/${username}`, { headers });
  }

  private createAuthorizationHeader(): HttpHeaders {
    const token = this.getToken();
    if (!token) {
      throw new Error('Token not found in localStorage');
    }
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }
}
