import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule,
    MatDividerModule
  ],
  template: `
    <div class="app-container">
      <!-- Toolbar -->
      <mat-toolbar color="primary" class="app-toolbar">
        <button 
          mat-icon-button 
          (click)="sidenav.toggle()" 
          class="menu-button"
          [attr.aria-label]="'Toggle navigation menu'">
          <mat-icon>menu</mat-icon>
        </button>
        
        <span class="app-title">
          <mat-icon class="title-icon">contacts</mat-icon>
          Phone Registry System
        </span>
        
        <span class="spacer"></span>
        
        <button mat-icon-button [attr.aria-label]="'Notifications'">
          <mat-icon>notifications</mat-icon>
        </button>
        
        <button mat-icon-button [attr.aria-label]="'User profile'">
          <mat-icon>account_circle</mat-icon>
        </button>
      </mat-toolbar>

      <!-- Sidenav Container -->
      <mat-sidenav-container class="sidenav-container">
        <mat-sidenav 
          #sidenav 
          mode="side" 
          opened 
          class="sidenav"
          [attr.aria-label]="'Navigation menu'">
          
          <mat-nav-list>
            <!-- Persons Section -->
            <a mat-list-item 
               routerLink="/persons" 
               routerLinkActive="active-link" 
               (click)="closeIfMobile(sidenav)"
               [attr.aria-label]="'Navigate to persons list'">
              <mat-icon>people</mat-icon>
              <span>Kişiler</span>
            </a>

            <a mat-list-item 
               routerLink="/persons/create" 
               routerLinkActive="active-link" 
               (click)="closeIfMobile(sidenav)"
               [attr.aria-label]="'Navigate to create person'">
              <mat-icon>person_add</mat-icon>
              <span>Kişi Ekle</span>
            </a>

            <mat-divider></mat-divider>

            <!-- Reports Section -->
            <a mat-list-item 
               routerLink="/reports" 
               routerLinkActive="active-link" 
               (click)="closeIfMobile(sidenav)"
               [attr.aria-label]="'Navigate to reports'">
              <mat-icon>assessment</mat-icon>
              <span>Raporlar</span>
            </a>
          </mat-nav-list>
        </mat-sidenav>

        <!-- Main Content -->
        <mat-sidenav-content class="main-content">
          <div class="content-wrapper">
            <router-outlet></router-outlet>
          </div>
        </mat-sidenav-content>
      </mat-sidenav-container>
    </div>
  `,
  styles: [`
    .app-container {
      height: 100vh;
      display: flex;
      flex-direction: column;
      background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 50%, #e2e8f0 100%);
    }

    .app-toolbar {
      position: sticky; 
      top: 0; 
      z-index: 1000;
      background: var(--surface-0) !important;
      color: var(--primary-700) !important;
      border-bottom: 1px solid var(--neutral-200);
      box-shadow: var(--shadow-sm);
    }

    .menu-button {
      margin-right: 8px;
    }

    .menu-button mat-icon {
      color: var(--primary-700) !important;
    }

    .app-title {
      display: flex;
      align-items: center;
      gap: 8px;
      font-weight: 600;
      color: var(--primary-700);
      font-size: 1.1rem;
    }

    .title-icon {
      font-size: 20px;
      color: var(--primary-700);
    }

    .spacer {
      flex: 1;
    }

    .sidenav-container {
      flex: 1;
      margin-top: 64px;
    }

    .sidenav {
      width: 250px;
      background: rgba(255, 255, 255, 0.95);
      backdrop-filter: blur(20px);
      border-right: 1px solid var(--border-color);
      box-shadow: var(--shadow-lg);
      transition: width 0.3s ease;
    }

    .main-content {
      background: transparent;
    }

    .content-wrapper {
      padding: 20px;
      min-height: calc(100vh - 84px);
    }

    .active-link {
      background: var(--gradient-primary) !important;
      color: white !important;
      border-radius: 12px !important;
      margin: 4px 8px !important;
      box-shadow: 0 4px 15px rgba(59, 130, 246, 0.3) !important;
    }

    .active-link mat-icon {
      color: white !important;
    }

    .mat-mdc-list-item {
      border-radius: 12px !important;
      margin: 4px 8px !important;
      transition: all 0.3s ease !important;
      color: var(--text-primary) !important;
    }
    
    .mat-mdc-list-item mat-icon {
      color: var(--text-secondary) !important;
      margin-right: 12px;
    }

    .mat-mdc-list-item:hover {
      background: rgba(59, 130, 246, 0.1) !important;
      transform: translateX(4px) !important;
    }

    .mat-divider {
      margin: 8px 16px;
    }

    /* Responsive Design */
    @media (max-width: 768px) {
      .sidenav {
        width: 200px;
      }
      
      .content-wrapper {
        padding: 15px;
      }

      .app-title {
        font-size: 1rem;
      }

      .title-icon {
        font-size: 18px;
      }
    }

    @media (max-width: 480px) {
      .sidenav {
        width: 180px;
      }
      
      .content-wrapper {
        padding: 10px;
      }

      .app-title {
        font-size: 0.9rem;
      }
    }
  `]
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'Phone Registry System';
  isMobile = false;
  
  private destroy$ = new Subject<void>();

  constructor(private breakpointObserver: BreakpointObserver) {}

  ngOnInit(): void {
    this.breakpointObserver
      .observe([Breakpoints.HandsetPortrait, Breakpoints.TabletPortrait])
      .pipe(takeUntil(this.destroy$))
      .subscribe(result => {
        this.isMobile = result.matches;
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  closeIfMobile(sidenav: any): void {
    if (this.isMobile) {
      sidenav.close();
    }
  }
}

