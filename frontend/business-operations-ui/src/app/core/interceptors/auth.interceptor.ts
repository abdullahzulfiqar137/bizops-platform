import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, take, switchMap } from 'rxjs/operators';
import { TokenStorageService } from '../services/token-storage.service';
import { AuthApiService } from '../services/auth-api.service';
import { Router } from '@angular/router';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(
    private tokenStorage: TokenStorageService,
    private authApi: AuthApiService,
    private router: Router
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let authReq = request;
    const token = this.tokenStorage.getAccessToken();

    if (token && request.url.includes('/api/')) {
      authReq = this.addTokenHeader(request, token);
    }

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (
          error.status === 401 &&
          !authReq.url.includes('/auth/login') &&
          !authReq.url.includes('/auth/refresh-token')
        ) {
          return this.handle401Error(authReq, next);
        }
        return throwError(() => error);
      })
    );
  }

  private addTokenHeader(request: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  private handle401Error(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      const accessToken = this.tokenStorage.getAccessToken();
      if (accessToken) {
        return this.authApi.refreshToken({ accessToken, refreshToken: '' }).pipe(
          switchMap(res => {
            this.isRefreshing = false;
            if (res.success && res.data) {
              this.refreshTokenSubject.next(res.data.accessToken);
              return next.handle(this.addTokenHeader(request, res.data.accessToken));
            }
            this.authApi.logout();
            this.router.navigate(['/auth/login']);
            return throwError(() => new Error('Refresh token invalid'));
          }),
          catchError(err => {
            this.isRefreshing = false;
            this.authApi.logout();
            this.router.navigate(['/auth/login']);
            return throwError(() => err);
          })
        );
      } else {
        this.isRefreshing = false;
        this.authApi.logout();
        this.router.navigate(['/auth/login']);
        return throwError(() => new Error('No tokens stored'));
      }
    } else {
      return this.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(jwt => next.handle(this.addTokenHeader(request, jwt!)))
      );
    }
  }
}
