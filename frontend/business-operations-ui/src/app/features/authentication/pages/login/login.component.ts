import { Component, HostListener, inject, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../../../core/services/auth-api.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  @ViewChild('robotContainer', { static: false }) robotContainer!: ElementRef;

  private fb = inject(FormBuilder);
  private authApi = inject(AuthApiService);
  private router = inject(Router);

  loading = false;
  showPassword = false;
  errorMessage = '';

  robotMood: 'happy' | 'sad' | 'thinking' = 'happy';
  speechMessage = "Hi there! Welcome back again! 👋 Sign in here quickly to access your live sales, inventory, and AI operations.";

  headTransform = 'rotateX(0deg) rotateY(0deg)';
  pupilTransform = 'translate(0px, 0px)';

  loginForm = this.fb.group({
    usernameOrEmail: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  @HostListener('document:mousemove', ['$event'])
  onMouseMove(event: MouseEvent): void {
    if (!this.robotContainer) return;
    const rect = this.robotContainer.nativeElement.getBoundingClientRect();
    const eyeCenterX = rect.left + rect.width / 2;
    const eyeCenterY = rect.top + 35;

    // Calculate angle & distance for eyes
    const angle = Math.atan2(event.clientY - eyeCenterY, event.clientX - eyeCenterX);
    const distance = Math.min(Math.hypot(event.clientX - eyeCenterX, event.clientY - eyeCenterY) / 20, 5);

    const pupilX = Math.cos(angle) * distance;
    const pupilY = Math.sin(angle) * distance;
    this.pupilTransform = `translate(${pupilX.toFixed(1)}px, ${pupilY.toFixed(1)}px)`;

    // Calculate 3D Head Tilt & Turn
    const deltaX = (event.clientX - eyeCenterX) / (window.innerWidth / 2);
    const deltaY = (event.clientY - eyeCenterY) / (window.innerHeight / 2);

    const rotY = Math.max(Math.min(deltaX * 22, 22), -22);
    const rotX = Math.max(Math.min(-deltaY * 18, 18), -18);

    this.headTransform = `perspective(600px) rotateX(${rotX.toFixed(1)}deg) rotateY(${rotY.toFixed(1)}deg)`;
  }

  isInvalid(controlName: string): boolean {
    const control = this.loginForm.get(controlName);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onInputFocus(): void {
    if (this.robotMood === 'sad') {
      this.robotMood = 'happy';
      this.speechMessage = "Hi there! Welcome back again! 👋 Sign in here quickly to access your live sales, inventory, and AI operations.";
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.robotMood = 'thinking';
    this.speechMessage = "Verifying your credentials with security servers...";

    const val = this.loginForm.value;
    this.authApi.login({
      usernameOrEmail: val.usernameOrEmail!,
      password: val.password!
    }).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success) {
          this.router.navigate(['/dashboard']);
        } else {
          this.robotMood = 'sad';
          this.errorMessage = res.message || 'Login failed. Invalid credentials.';
          this.speechMessage = "Oops! Invalid credentials 🥺 If you don't have an account yet, click 'Register Account Now' to join us!";
        }
      },
      error: (err) => {
        this.loading = false;
        this.robotMood = 'sad';
        this.errorMessage = err.error?.message || 'Authentication failed. Incorrect username or password.';
        this.speechMessage = "Oops! Incorrect username or password 🥺 If you don't have an account yet, click 'Register Account Now' below to sign up!";
      }
    });
  }
}
