import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterModule } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';

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
    MatListModule
  ],
  template: `
    <div class="app-container">
      <mat-toolbar color="primary" class="app-toolbar">
        <button mat-icon-button (click)="sidenav.toggle()" class="menu-button">
          <mat-icon>menu</mat-icon>
        </button>
        <span class="app-title">
          <mat-icon class="title-icon">contacts</mat-icon>
          Phone Registry System
        </span>
        <span class="spacer"></span>
        <button mat-icon-button>
          <mat-icon>notifications</mat-icon>
        </button>
        <button mat-icon-button>
          <mat-icon>account_circle</mat-icon>
        </button>
      </mat-toolbar>

      <mat-sidenav-container class="sidenav-container">
        <mat-sidenav #sidenav mode="side" opened class="sidenav">
          <mat-nav-list>
            <a mat-list-item routerLink="/persons" routerLinkActive="active-link">
              <mat-icon matListItemIcon>people</mat-icon>
              <span matListItemTitle>Kişiler</span>
            </a>
            <a mat-list-item routerLink="/persons/create" routerLinkActive="active-link">
              <mat-icon matListItemIcon>person_add</mat-icon>
              <span matListItemTitle>Kişi Ekle</span>
            </a>
            <mat-divider></mat-divider>
            <a mat-list-item routerLink="/reports" routerLinkActive="active-link">
              <mat-icon matListItemIcon>assessment</mat-icon>
              <span matListItemTitle>Raporlar</span>
            </a>
          </mat-nav-list>
        </mat-sidenav>

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
    }

    .app-toolbar {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      z-index: 1000;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }

    .app-title {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 1.2rem;
      font-weight: 500;
    }

    .title-icon {
      font-size: 1.5rem;
      width: 1.5rem;
      height: 1.5rem;
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
      background: #fafafa;
      border-right: 1px solid #e0e0e0;
    }

    .main-content {
      background: #f5f7fa;
    }

    .content-wrapper {
      padding: 20px;
      min-height: calc(100vh - 84px);
    }

    .active-link {
      background: rgba(63, 81, 181, 0.1) !important;
      color: #3f51b5 !important;
    }

    .active-link mat-icon {
      color: #3f51b5 !important;
    }

    @media (max-width: 768px) {
      .sidenav {
        width: 200px;
      }
      
      .content-wrapper {
        padding: 15px;
      }
    }
  `]
})
export class AppComponent {
  title = 'Phone Registry System';
}
