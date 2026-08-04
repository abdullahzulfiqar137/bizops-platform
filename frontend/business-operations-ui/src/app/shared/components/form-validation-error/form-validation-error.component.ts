import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-form-validation-error',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './form-validation-error.component.html',
  styleUrl: './form-validation-error.component.css'
})
export class FormValidationErrorComponent {
  @Input() show = false;
  @Input() message = 'This field is required';
}
