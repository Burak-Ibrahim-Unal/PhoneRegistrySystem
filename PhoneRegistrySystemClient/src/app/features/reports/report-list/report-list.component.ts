import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatBadgeModule } from '@angular/material/badge';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ReportService } from '../../../core/services/report.service';
import { Report, ReportStatus, ReportStatusLabels, ReportStatusColors, ReportStatusIcons } from '../../../core/models/report.model';

@Component({
  selector: 'app-report-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatChipsModule,
    MatBadgeModule,
    MatProgressBarModule
  ],
  template: `
    <div class="report-list-container fade-in">
      <div class="header-section">
        <h1 class="page-title">
          <mat-icon class="title-icon">assessment</mat-icon>
          Raporlar
        </h1>
        <div class="header-actions">
          <button mat-raised-button color="primary" (click)="requestNewReport()" [disabled]="requesting" class="request-btn">
            <mat-spinner diameter="20" *ngIf="requesting"></mat-spinner>
            <mat-icon *ngIf="!requesting">add_chart</mat-icon>
            {{ requesting ? 'Talep Ediliyor...' : 'Yeni Rapor Talep Et' }}
          </button>
          <button mat-icon-button (click)="loadReports()" [disabled]="loading" class="refresh-btn" matTooltip="Yenile">
            <mat-icon [class.spinning]="loading">refresh</mat-icon>
          </button>
        </div>
      </div>

      <!-- Stats Section -->
      <div class="stats-section">
        <div class="stat-card">
          <div class="stat-icon preparing">
            <mat-icon>hourglass_empty</mat-icon>
          </div>
          <div class="stat-content">
            <span class="stat-number">{{getReportCountByStatus(ReportStatus.Preparing)}}</span>
            <span class="stat-label">Hazırlanıyor</span>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon completed">
            <mat-icon>check_circle</mat-icon>
          </div>
          <div class="stat-content">
            <span class="stat-number">{{getReportCountByStatus(ReportStatus.Completed)}}</span>
            <span class="stat-label">Tamamlandı</span>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon failed">
            <mat-icon>error</mat-icon>
          </div>
          <div class="stat-content">
            <span class="stat-number">{{getReportCountByStatus(ReportStatus.Failed)}}</span>
            <span class="stat-label">Başarısız</span>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon total">
            <mat-icon>assessment</mat-icon>
          </div>
          <div class="stat-content">
            <span class="stat-number">{{reports.length}}</span>
            <span class="stat-label">Toplam</span>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading && reports.length === 0" class="loading-container">
        <mat-spinner diameter="50"></mat-spinner>
        <p>Raporlar yükleniyor...</p>
      </div>

      <!-- Empty State -->
      <div *ngIf="!loading && reports.length === 0" class="empty-state">
        <mat-icon class="empty-icon">assessment</mat-icon>
        <h2>Henüz rapor bulunmuyor</h2>
        <p>İlk raporunuzu oluşturmak için "Yeni Rapor Talep Et" butonuna tıklayın.</p>
        <button mat-raised-button color="primary" (click)="requestNewReport()" [disabled]="requesting">
          <mat-icon>add_chart</mat-icon>
          Rapor Talep Et
        </button>
      </div>

      <!-- Reports Grid -->
      <div *ngIf="!loading && reports.length > 0" class="reports-grid">
        <mat-card *ngFor="let report of reports; trackBy: trackByReportId" class="report-card custom-card">
          <mat-card-header>
            <div mat-card-avatar class="report-avatar" [ngClass]="getStatusClass(report.status)">
              <mat-icon>{{getStatusIcon(report.status)}}</mat-icon>
            </div>
            <mat-card-title class="report-title">
              Konum Raporu
              <mat-chip class="status-chip" [ngClass]="getStatusChipClass(report.status)">
                {{getStatusLabel(report.status)}}
              </mat-chip>
            </mat-card-title>
            <mat-card-subtitle>
              {{report.requestedAt | date:'dd.MM.yyyy HH:mm'}}
            </mat-card-subtitle>
          </mat-card-header>

          <mat-card-content>
            <!-- Progress Bar for Preparing Reports -->
            <div *ngIf="report.status === ReportStatus.Preparing" class="progress-section">
              <mat-progress-bar mode="indeterminate" class="progress-bar"></mat-progress-bar>
              <p class="progress-text">Rapor hazırlanıyor...</p>
            </div>

            <!-- Report Summary for Completed Reports -->
            <div *ngIf="report.status === ReportStatus.Completed" class="report-summary">
              <div class="summary-item">
                <mat-icon class="summary-icon">location_on</mat-icon>
                <span>{{report.locationStatistics?.length || 0}} konum</span>
              </div>
              <div class="summary-item">
                <mat-icon class="summary-icon">people</mat-icon>
                <span>{{getTotalPersonCount(report)}} kişi</span>
              </div>
              <div class="summary-item">
                <mat-icon class="summary-icon">phone</mat-icon>
                <span>{{getTotalPhoneCount(report)}} telefon</span>
              </div>
            </div>

            <!-- Error Message for Failed Reports -->
            <div *ngIf="report.status === ReportStatus.Failed" class="error-section">
              <mat-icon class="error-icon">error_outline</mat-icon>
              <p class="error-text">{{report.errorMessage || 'Rapor oluşturulurken bir hata oluştu.'}}</p>
            </div>

            <!-- Completion Time -->
            <div *ngIf="report.completedAt" class="completion-info">
              <mat-icon class="completion-icon">schedule</mat-icon>
              <span>{{report.completedAt | date:'dd.MM.yyyy HH:mm'}} tarihinde tamamlandı</span>
            </div>
          </mat-card-content>

          <mat-card-actions align="end">
            <button 
              mat-button 
              color="primary" 
              [routerLink]="['/reports', report.id]"
              [disabled]="report.status === ReportStatus.Preparing">
              <mat-icon>visibility</mat-icon>
              {{ report.status === ReportStatus.Completed ? 'Görüntüle' : 'Detaylar' }}
            </button>
            <button 
              *ngIf="report.status === ReportStatus.Failed" 
              mat-button 
              color="accent" 
              (click)="retryReport(report)">
              <mat-icon>refresh</mat-icon>
              Tekrar Dene
            </button>
          </mat-card-actions>
        </mat-card>
      </div>

      <!-- Auto Refresh Indicator -->
      <div *ngIf="hasPreparingReports()" class="auto-refresh-indicator">
        <mat-icon class="refresh-icon">sync</mat-icon>
        <span>Hazırlanan raporlar için otomatik yenileme aktif</span>
      </div>
    </div>
  `,
  styles: [`
    .report-list-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 20px;
    }

    .header-section {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 30px;
      flex-wrap: wrap;
      gap: 20px;
    }

    .page-title {
      display: flex;
      align-items: center;
      gap: 12px;
      margin: 0;
      color: var(--text-primary);
      font-size: 2rem;
      font-weight: 500;
    }

    .title-icon {
      font-size: 2rem;
      width: 2rem;
      height: 2rem;
      color: var(--primary-color);
    }

    .header-actions {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .request-btn {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 12px 24px;
      border-radius: 8px;
      font-weight: 500;
      min-width: 180px;
    }

    .refresh-btn {
      background: rgba(0, 0, 0, 0.04);
      border-radius: 8px;
    }

    .spinning {
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    .stats-section {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 20px;
      margin-bottom: 30px;
    }

    .stat-card {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 20px;
      background: var(--surface-color);
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      transition: all 0.3s ease;
    }

    .stat-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    }

    .stat-icon {
      width: 48px;
      height: 48px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
    }

    .stat-icon.preparing {
      background: linear-gradient(45deg, #ff9800, #f57c00);
    }

    .stat-icon.completed {
      background: linear-gradient(45deg, #4caf50, #388e3c);
    }

    .stat-icon.failed {
      background: linear-gradient(45deg, #f44336, #d32f2f);
    }

    .stat-icon.total {
      background: linear-gradient(45deg, var(--primary-color), #1976d2);
    }

    .stat-content {
      display: flex;
      flex-direction: column;
    }

    .stat-number {
      font-size: 1.8rem;
      font-weight: 600;
      color: var(--text-primary);
      line-height: 1;
    }

    .stat-label {
      font-size: 0.9rem;
      color: var(--text-secondary);
      margin-top: 4px;
    }

    .loading-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 60px 20px;
      text-align: center;
      gap: 20px;
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 60px 20px;
      text-align: center;
      gap: 20px;
      color: var(--text-secondary);
    }

    .empty-icon {
      font-size: 4rem;
      width: 4rem;
      height: 4rem;
      opacity: 0.5;
    }

    .reports-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
      gap: 24px;
      margin-bottom: 30px;
    }

    .report-card {
      transition: all 0.3s ease;
      position: relative;
      overflow: hidden;
    }

    .report-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
    }

    .report-avatar {
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
    }

    .report-avatar.preparing {
      background: linear-gradient(45deg, #ff9800, #f57c00);
    }

    .report-avatar.completed {
      background: linear-gradient(45deg, #4caf50, #388e3c);
    }

    .report-avatar.failed {
      background: linear-gradient(45deg, #f44336, #d32f2f);
    }

    .report-title {
      display: flex;
      align-items: center;
      gap: 12px;
      flex-wrap: wrap;
    }

    .status-chip {
      font-size: 0.75rem;
      height: 24px;
      font-weight: 500;
    }

    .status-chip.preparing {
      background: #fff3e0;
      color: #e65100;
    }

    .status-chip.completed {
      background: #e8f5e8;
      color: #2e7d32;
    }

    .status-chip.failed {
      background: #ffebee;
      color: #c62828;
    }

    .progress-section {
      margin: 16px 0;
    }

    .progress-bar {
      margin-bottom: 8px;
    }

    .progress-text {
      margin: 0;
      font-size: 0.9rem;
      color: var(--text-secondary);
      text-align: center;
    }

    .report-summary {
      display: flex;
      flex-direction: column;
      gap: 8px;
      margin: 16px 0;
    }

    .summary-item {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 0.9rem;
      color: var(--text-secondary);
    }

    .summary-icon {
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
      color: var(--primary-color);
    }

    .error-section {
      display: flex;
      align-items: flex-start;
      gap: 8px;
      margin: 16px 0;
      padding: 12px;
      background: #ffebee;
      border-radius: 8px;
      border-left: 4px solid #f44336;
    }

    .error-icon {
      color: #f44336;
      font-size: 1.2rem;
      width: 1.2rem;
      height: 1.2rem;
      margin-top: 2px;
    }

    .error-text {
      margin: 0;
      font-size: 0.9rem;
      color: #c62828;
      line-height: 1.4;
    }

    .completion-info {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-top: 12px;
      font-size: 0.85rem;
      color: var(--text-secondary);
    }

    .completion-icon {
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
    }

    .auto-refresh-indicator {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      padding: 12px;
      background: #e3f2fd;
      border-radius: 8px;
      color: var(--primary-color);
      font-size: 0.9rem;
      margin-top: 20px;
    }

    .refresh-icon {
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
      animation: spin 2s linear infinite;
    }

    .ReportStatus {
      display: none;
    }

    @media (max-width: 768px) {
      .report-list-container {
        padding: 15px;
      }

      .header-section {
        flex-direction: column;
        align-items: stretch;
        text-align: center;
      }

      .page-title {
        font-size: 1.5rem;
        justify-content: center;
      }

      .header-actions {
        justify-content: center;
      }

      .request-btn {
        width: 100%;
        justify-content: center;
      }

      .stats-section {
        grid-template-columns: repeat(2, 1fr);
        gap: 16px;
      }

      .reports-grid {
        grid-template-columns: 1fr;
        gap: 16px;
      }
    }

    @media (max-width: 480px) {
      .stats-section {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class ReportListComponent implements OnInit, OnDestroy {
  reports: Report[] = [];
  loading = false;
  requesting = false;
  private refreshSubscription?: Subscription;
  
  // Expose enums to template
  ReportStatus = ReportStatus;

  constructor(
    private reportService: ReportService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadReports();
    this.startAutoRefresh();
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
  }

  loadReports(): void {
    this.loading = true;
    this.reportService.getReports().subscribe({
      next: (reports) => {
        this.reports = reports.sort((a, b) => 
          new Date(b.requestedAt).getTime() - new Date(a.requestedAt).getTime()
        );
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading reports:', error);
        this.snackBar.open('Raporlar yüklenirken hata oluştu', 'Kapat', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        this.loading = false;
      }
    });
  }

  requestNewReport(): void {
    this.requesting = true;
    this.reportService.requestReport().subscribe({
      next: (report) => {
        this.snackBar.open('Rapor talebi başarıyla oluşturuldu', 'Kapat', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.reports.unshift(report);
        this.requesting = false;
        this.startAutoRefresh();
      },
      error: (error) => {
        console.error('Error requesting report:', error);
        this.snackBar.open('Rapor talebi oluşturulurken hata oluştu', 'Kapat', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        this.requesting = false;
      }
    });
  }

  retryReport(report: Report): void {
    // In a real application, you might have a retry endpoint
    this.requestNewReport();
  }

  trackByReportId(index: number, report: Report): string {
    return report.id;
  }

  getStatusLabel(status: ReportStatus): string {
    return ReportStatusLabels[status] || 'Bilinmeyen';
  }

  getStatusIcon(status: ReportStatus): string {
    return ReportStatusIcons[status] || 'help';
  }

  getStatusClass(status: ReportStatus): string {
    switch (status) {
      case ReportStatus.Preparing:
        return 'preparing';
      case ReportStatus.Completed:
        return 'completed';
      case ReportStatus.Failed:
        return 'failed';
      default:
        return '';
    }
  }

  getStatusChipClass(status: ReportStatus): string {
    return this.getStatusClass(status);
  }

  getReportCountByStatus(status: ReportStatus): number {
    return this.reports.filter(r => r.status === status).length;
  }

  getTotalPersonCount(report: Report): number {
    return report.locationStatistics?.reduce((sum, stat) => sum + stat.personCount, 0) || 0;
  }

  getTotalPhoneCount(report: Report): number {
    return report.locationStatistics?.reduce((sum, stat) => sum + stat.phoneNumberCount, 0) || 0;
  }

  hasPreparingReports(): boolean {
    return this.reports.some(r => r.status === ReportStatus.Preparing);
  }

  private startAutoRefresh(): void {
    this.stopAutoRefresh();
    
    if (this.hasPreparingReports()) {
      this.refreshSubscription = interval(5000) // 5 seconds
        .pipe(switchMap(() => this.reportService.getReports()))
        .subscribe({
          next: (reports) => {
            this.reports = reports.sort((a, b) => 
              new Date(b.requestedAt).getTime() - new Date(a.requestedAt).getTime()
            );
            
            // Stop auto refresh if no preparing reports
            if (!this.hasPreparingReports()) {
              this.stopAutoRefresh();
            }
          },
          error: (error) => {
            console.error('Auto refresh error:', error);
            this.stopAutoRefresh();
          }
        });
    }
  }

  private stopAutoRefresh(): void {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
      this.refreshSubscription = undefined;
    }
  }
}
