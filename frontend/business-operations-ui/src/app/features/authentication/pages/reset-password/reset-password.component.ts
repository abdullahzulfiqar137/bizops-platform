import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthApiService } from '../../../../core/services/auth-api.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './reset-password.component.html',
  styleUrl: '../password-recovery.scss'
})
export class ResetPasswordComponent {
  private fb = inject(FormBuilder);
  private api = inject(AuthApiService);
  private route = inject(ActivatedRoute);

  loading = false;
  message = '';
  error = '';

  form = this.fb.nonNullable.group({
    email: [this.route.snapshot.queryParamMap.get('email') || '', [Validators.required, Validators.email]],
    token: [this.route.snapshot.queryParamMap.get('token') || '', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', Validators.required]
  });

  submit(): void {
    if (this.form.invalid || this.loading) return;
    const v = this.form.getRawValue();
    if (v.newPassword !== v.confirmPassword) {
      this.error = 'Passwords do not match.';
      return;
    }
    this.loading = true;
    this.error = '';
    this.api.resetPassword(v)
      .pipe(finalize(() => this.loading = false))
      .subscribe({
        next: r => this.message = r.message,
        error: e => this.error = e.error?.message || 'Password reset failed.'
      });
  }
}
