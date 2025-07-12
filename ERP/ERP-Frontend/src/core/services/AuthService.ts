import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { AuthStateService } from './auth-state.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'token';
  // OAuth configuration moved to environment variables for security
  private readonly OAUTH_AUTHORIZE_URL = process.env['OAUTH_AUTHORIZE_URL'] || '';
  private readonly CLIENT_ID = process.env['OAUTH_CLIENT_ID'] || '';
  private readonly REDIRECT_URI = window.location.origin + (process.env['OAUTH_REDIRECT_URI'] || '/auth-callback');
  private readonly RESPONSE_TYPE = process.env['OAUTH_RESPONSE_TYPE'] || 'token'; // or 'code' for authorization code flow
  private readonly SCOPE = process.env['OAUTH_SCOPE'] || 'openid profile email';

  constructor(private router: Router, private authStateService: AuthStateService) {}

  login(): void {
    const authUrl = `${this.OAUTH_AUTHORIZE_URL}?response_type=${this.RESPONSE_TYPE}&client_id=${this.CLIENT_ID}&redirect_uri=${encodeURIComponent(this.REDIRECT_URI)}&scope=${encodeURIComponent(this.SCOPE)}`;
    window.location.href = authUrl;
  }

  handleAuthCallback(): void {
    // Parse the URL hash to extract the access token
    const hash = window.location.hash.substr(1);
    const params = new URLSearchParams(hash);
    const token = params.get('access_token');
    if (token) {
      localStorage.setItem(this.TOKEN_KEY, token);
      this.authStateService.setLoggedIn(true);
      this.router.navigate(['/']);
    } else {
      this.authStateService.setLoggedIn(false);
      this.router.navigate(['/login']);
    }
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.authStateService.setLoggedIn(false);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUserInfo(): any {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    // Decode JWT token payload (assuming JWT)
    const payload = token.split('.')[1];
    if (!payload) {
      return null;
    }
    try {
      const decoded = atob(payload);
      return JSON.parse(decoded);
    } catch {
      return null;
    }
  }
}
