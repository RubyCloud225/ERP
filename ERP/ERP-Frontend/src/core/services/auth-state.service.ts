import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthStateService {
  private loggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private showDashboardSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor() {
    const token = localStorage.getItem('token');
    this.loggedInSubject.next(!!token);
  }

  get isLoggedIn$(): Observable<boolean> {
    return this.loggedInSubject.asObservable();
  }

  get showDashboard$(): Observable<boolean> {
    return this.showDashboardSubject.asObservable();
  }

  setLoggedIn(value: boolean): void {
    this.loggedInSubject.next(value);
    if (value) {
      this.showDashboardSubject.next(true);
    } else {
      this.showDashboardSubject.next(false);
    }
  }

  hideDashboard(): void {
    this.showDashboardSubject.next(false);
  }
}
