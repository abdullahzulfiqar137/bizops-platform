import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthStateService } from '../../core/state/auth-state.service';
import { TranslationService } from '../../core/services/translation.service';
import { Roles } from '../../core/constants/roles.constant';
import {
  LucideBell, LucideBot, LucideBriefcaseBusiness, LucideBuilding2,
  LucideChartNoAxesCombined, LucideCreditCard, LucideDatabaseBackup,
  LucideFileUp, LucideLandmark, LucideLayoutDashboard, LucideMessageSquare,
  LucidePackage, LucideReceiptText, LucideScrollText, LucideShoppingCart,
  LucideTags, LucideTruck, LucideUndo2, LucideUserCog, LucideUsersRound,
  LucideWalletCards, LucideWarehouse
} from '@lucide/angular';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule, RouterLink, RouterLinkActive, LucideBell, LucideBot,
    LucideBriefcaseBusiness, LucideBuilding2, LucideChartNoAxesCombined,
    LucideCreditCard, LucideDatabaseBackup, LucideFileUp, LucideLandmark,
    LucideLayoutDashboard, LucideMessageSquare, LucidePackage,
    LucideReceiptText, LucideScrollText, LucideShoppingCart, LucideTags,
    LucideTruck, LucideUndo2, LucideUserCog, LucideUsersRound,
    LucideWalletCards, LucideWarehouse
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent {
  @Input() isOpen = false;
  @Input() collapsed = false;
  @Output() closeSidebar = new EventEmitter<void>();

  private authState = inject(AuthStateService);
  ts = inject(TranslationService);
  readonly roles = Roles;

  onClose(): void {
    this.closeSidebar.emit();
  }

  onNavClick(): void {
    if (window.innerWidth <= 768) {
      this.onClose();
    }
  }
}
