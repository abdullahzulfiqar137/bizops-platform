import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-skeleton-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './skeleton-loader.component.html',
  styleUrl: './skeleton-loader.component.css'
})
export class SkeletonLoaderComponent {
  @Input() type: 'card' | 'table' | 'line' = 'card';
  @Input() count = 4;
  @Input() height = 20;

  get countArray(): number[] {
    return Array(this.count).fill(0);
  }
}
