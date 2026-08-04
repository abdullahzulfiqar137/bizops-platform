import { Component, HostListener, inject, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthApiService } from '../../../../core/services/auth-api.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  @ViewChild('robotContainer', { static: false }) robotContainer!: ElementRef;

  private fb = inject(FormBuilder);
  private authApi = inject(AuthApiService);
  private router = inject(Router);

  loading = false;
  showPw = false;
  showConfirmPw = false;

  errorMessage = '';
  successMessage = '';

  robotMood: 'welcome' | 'excited' | 'confused' = 'welcome';
  speechMessage = "Welcome to AI BizOps Enterprise! 🚀 We help businesses streamline POS billing, stock inventory, and AI analytics. Join us & register now!";

  headTransform = 'rotateX(0deg) rotateY(0deg)';
  pupilTransform = 'translate(0px, 0px)';

  registerForm = this.fb.group({
    companyName: ['', [Validators.required, Validators.maxLength(200)]],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    username: ['', Validators.required],
    phoneNumber: [''],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required]
  });

  @HostListener('document:mousemove', ['$event'])
  onMouseMove(event: MouseEvent): void {
    if (!this.robotContainer) return;
    const rect = this.robotContainer.nativeElement.getBoundingClientRect();
    const eyeCenterX = rect.left + rect.width / 2;
    const eyeCenterY = rect.top + 35;

    const angle = Math.atan2(event.clientY - eyeCenterY, event.clientX - eyeCenterX);
    const distance = Math.min(Math.hypot(event.clientX - eyeCenterX, event.clientY - eyeCenterY) / 20, 5);

    const pupilX = Math.cos(angle) * distance;
    const pupilY = Math.sin(angle) * distance;
    this.pupilTransform = `translate(${pupilX.toFixed(1)}px, ${pupilY.toFixed(1)}px)`;

    const deltaX = (event.clientX - eyeCenterX) / (window.innerWidth / 2);
    const deltaY = (event.clientY - eyeCenterY) / (window.innerHeight / 2);

    const rotY = Math.max(Math.min(deltaX * 22, 22), -22);
    const rotX = Math.max(Math.min(-deltaY * 18, 18), -18);

    this.headTransform = `perspective(600px) rotateX(${rotX.toFixed(1)}deg) rotateY(${rotY.toFixed(1)}deg)`;
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      this.errorMessage = 'Please complete all required fields.';
      this.speechMessage = "Oops! Please complete all required fields above to register your account!";
      return;
    }

    const val = this.registerForm.value;
    if (val.password !== val.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      this.speechMessage = "Oops! Your Password and Confirm Password do not match.";
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authApi.register({
      companyName: val.companyName!,
      firstName: val.firstName!,
      lastName: val.lastName!,
      email: val.email!,
      username: val.username!,
      phoneNumber: val.phoneNumber || null,
      password: val.password!,
      confirmPassword: val.confirmPassword!
    }).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success) {
          this.robotMood = 'excited';
          this.successMessage = 'Account created successfully! Redirecting to login...';
          this.speechMessage = "Congratulations! 🎉 Your account has been registered successfully! Now go back to Sign In and log in with your credentials to enjoy our business platform!";
          setTimeout(() => this.router.navigate(['/auth/login']), 3500);
        } else {
          this.errorMessage = res.message || 'Registration failed.';
        }
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err.error?.message || 'Registration failed.';
      }
    });
  }
}
