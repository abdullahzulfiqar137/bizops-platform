import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './empty-state.component.html',
  styleUrl: './empty-state.component.css'
})
export class EmptyStateComponent {
  @Input() icon = '📂';
  @Input() title = 'No data found';
  @Input() description = 'There are currently no records to display.';
  @Input() actionLabel = '';
  @Output() action = new EventEmitter<void>();
}
