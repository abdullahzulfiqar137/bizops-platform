import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd, RouterLink } from '@angular/router';
import { filter } from 'rxjs/operators';

interface Breadcrumb {
  label: string;
  url: string;
}

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './breadcrumb.component.html',
  styleUrl: './breadcrumb.component.css'
})
export class BreadcrumbComponent {
  private router = inject(Router);
  breadcrumbs: Breadcrumb[] = [];

  constructor() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.breadcrumbs = this.buildBreadcrumbs(this.router.url);
    });
    this.breadcrumbs = this.buildBreadcrumbs(this.router.url);
  }

  private buildBreadcrumbs(url: string): Breadcrumb[] {
    const paths = url.split('?')[0].split('/').filter(p => p.length > 0);
    const crumbs: Breadcrumb[] = [{ label: 'Home', url: '/dashboard' }];

    let accumulatedUrl = '';
    for (const path of paths) {
      accumulatedUrl += `/${path}`;
      if (accumulatedUrl !== '/dashboard') {
        crumbs.push({
          label: path.replace(/-/g, ' '),
          url: accumulatedUrl
        });
      }
    }

    return crumbs;
  }
}
