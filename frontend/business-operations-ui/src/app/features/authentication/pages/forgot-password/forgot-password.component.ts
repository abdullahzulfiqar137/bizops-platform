import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthApiService } from '../../../../core/services/auth-api.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password.component.html',
  styleUrl: '../password-recovery.scss'
})
export class ForgotPasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authApi = inject(AuthApiService);

  loading = false;
  message = '';
  error = '';
  form = this.fb.nonNullable.group({ email: ['', [Validators.required, Validators.email]] });

  submit(): void {
    if (this.form.invalid || this.loading) return;
    this.loading = true;
    this.message = '';
    this.error = '';

    this.authApi.forgotPassword(this.form.getRawValue()).pipe(finalize(() => (this.loading = false))).subscribe({
      next: response => {
        if (response.success) {
          this.message = response.message || 'If the account exists, password reset instructions will be sent.';
          this.form.disable();
        } else {
          this.error = response.message || 'Unable to process the recovery request.';
        }
      },
      error: err => this.error = err.error?.message || 'Unable to process the recovery request. Please try again.',
    });
  }
}
