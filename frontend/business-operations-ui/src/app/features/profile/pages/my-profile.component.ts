import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthApiService } from '../../../core/services/auth-api.service';
import { AuthStateService } from '../../../core/state/auth-state.service';
import { TranslationService } from '../../../core/services/translation.service';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './my-profile.component.html',
  styleUrl: './my-profile.component.css'
})
export class MyProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(AuthApiService);
  private state = inject(AuthStateService);
  ts = inject(TranslationService);

  user = this.state.currentUser;
  saving = false;
  message = '';
  error = '';

  form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['']
  });

  get initials(): string {
    const u = this.user();
    if (!u) return 'U';
    const f = u.firstName ? u.firstName[0] : '';
    const l = u.lastName ? u.lastName[0] : '';
    return (f + l).toUpperCase() || 'U';
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getCurrentUser().subscribe(r => {
      if (r.data) {
        this.form.patchValue({
          firstName: r.data.firstName,
          lastName: r.data.lastName,
          email: r.data.email,
          phoneNumber: r.data.phoneNumber || ''
        });
      }
    });
  }

  save(): void {
    if (this.form.invalid) return;
    this.saving = true;
    this.message = '';
    this.error = '';
    const v = this.form.getRawValue();

    this.api.updateCurrentUser({
      ...v,
      phoneNumber: v.phoneNumber || null
    }).pipe(finalize(() => this.saving = false)).subscribe({
      next: r => this.message = r.message,
      error: e => this.error = e.error?.message || 'Profile update failed.'
    });
  }
}
