import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { interval, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { ReportService } from '../../../core/services/report.service';
import { Report, ReportStatus, ReportStatusLabels, ReportStatusIcons, LocationStatistic } from '../../../core/models/report.model';

@Component({
  selector: 'app-report-detail',
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
    MatDividerModule,
    MatTableModule,
    MatSortModule,
    MatProgressBarModule
  ],
  template: `
    <div class="report-detail-container fade-in">
      <div class="header-section">
        <button mat-icon-button routerLink="/reports" class="back-btn">
          <mat-icon>arrow_back</mat-icon>
        </button>
        <div class="title-section" *ngIf="report">
          <h1 class="page-title">
            <mat-icon class="title-icon">assessment</mat-icon>
            Konum Raporu
          </h1>
          <div class="title-meta">
            <mat-chip class="status-chip" [ngClass]="getStatusChipClass(report.status)">
              <mat-icon class="chip-icon">{{getStatusIcon(report.status)}}</mat-icon>
              {{getStatusLabel(report.status)}}
            </mat-chip>
            <span class="date-info">{{report.requestedAt | date:'dd.MM.yyyy HH:mm'}}</span>
          </div>
        </div>
        <div class="actions">
          <button mat-icon-button (click)="refreshReport()" [disabled]="loading" matTooltip="Yenile">
            <mat-icon [class.spinning]="loading">refresh</mat-icon>
          </button>
        </div>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading && !report" class="loading-container">
        <mat-spinner diameter="50"></mat-spinner>
        <p>Rapor yükleniyor...</p>
      </div>

      <!-- Report Content -->
      <div *ngIf="report" class="content-grid">
        <!-- Report Info Card -->
        <mat-card class="report-info-card custom-card">
          <mat-card-header>
            <div mat-card-avatar class="report-avatar" [ngClass]="getStatusClass(report.status)">
              <mat-icon>{{getStatusIcon(report.status)}}</mat-icon>
            </div>
            <mat-card-title>Rapor Bilgileri</mat-card-title>
            <mat-card-subtitle>Genel bilgiler</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <div class="info-item">
              <mat-icon class="info-icon">schedule</mat-icon>
              <div class="info-content">
                <span class="info-label">Talep Tarihi</span>
                <span class="info-value">{{report.requestedAt | date:'dd.MM.yyyy HH:mm:ss'}}</span>
              </div>
            </div>
            <div class="info-item" *ngIf="report.completedAt">
              <mat-icon class="info-icon">check_circle</mat-icon>
              <div class="info-content">
                <span class="info-label">Tamamlanma Tarihi</span>
                <span class="info-value">{{report.completedAt | date:'dd.MM.yyyy HH:mm:ss'}}</span>
              </div>
            </div>
            <div class="info-item" *ngIf="report.completedAt">
              <mat-icon class="info-icon">timer</mat-icon>
              <div class="info-content">
                <span class="info-label">İşlem Süresi</span>
                <span class="info-value">{{getProcessingTime(report)}}</span>
              </div>
            </div>
            <div class="info-item">
              <mat-icon class="info-icon">fingerprint</mat-icon>
              <div class="info-content">
                <span class="info-label">Rapor ID</span>
                <span class="info-value">{{report.id}}</span>
              </div>
            </div>
          </mat-card-content>
        </mat-card>

        <!-- Status Card -->
        <mat-card class="status-card custom-card" [ngClass]="getStatusClass(report.status)">
          <mat-card-header>
            <div mat-card-avatar class="status-avatar">
              <mat-icon>{{getStatusIcon(report.status)}}</mat-icon>
            </div>
            <mat-card-title>{{getStatusLabel(report.status)}}</mat-card-title>
            <mat-card-subtitle>Rapor durumu</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <!-- Preparing State -->
            <div *ngIf="report.status === ReportStatus.Preparing" class="preparing-content">
              <mat-progress-bar mode="indeterminate" class="progress-bar"></mat-progress-bar>
              <p class="status-message">Rapor şu anda hazırlanıyor. Lütfen bekleyin...</p>
              <div class="auto-refresh-info">
                <mat-icon class="refresh-icon">sync</mat-icon>
                <span>Sayfa otomatik olarak yenileniyor</span>
              </div>
            </div>

            <!-- Completed State -->
            <div *ngIf="report.status === ReportStatus.Completed" class="completed-content">
              <div class="summary-stats">
                <div class="summary-stat">
                  <mat-icon class="stat-icon">location_on</mat-icon>
                  <div class="stat-content">
                    <span class="stat-number">{{report.locationStatistics?.length || 0}}</span>
                    <span class="stat-label">Konum</span>
                  </div>
                </div>
                <div class="summary-stat">
                  <mat-icon class="stat-icon">people</mat-icon>
                  <div class="stat-content">
                    <span class="stat-number">{{getTotalPersonCount()}}</span>
                    <span class="stat-label">Toplam Kişi</span>
                  </div>
                </div>
                <div class="summary-stat">
                  <mat-icon class="stat-icon">phone</mat-icon>
                  <div class="stat-content">
                    <span class="stat-number">{{getTotalPhoneCount()}}</span>
                    <span class="stat-label">Toplam Telefon</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Failed State -->
            <div *ngIf="report.status === ReportStatus.Failed" class="failed-content">
              <div class="error-message">
                <mat-icon class="error-icon">error_outline</mat-icon>
                <div class="error-text">
                  <p class="error-title">Rapor oluşturulamadı</p>
                  <p class="error-details">{{report.errorMessage || 'Bilinmeyen bir hata oluştu.'}}</p>
                </div>
              </div>
              <button mat-raised-button color="accent" (click)="requestNewReport()" class="retry-btn">
                <mat-icon>refresh</mat-icon>
                Tekrar Dene
              </button>
            </div>
          </mat-card-content>
        </mat-card>
      </div>

      <!-- Statistics Table -->
      <div *ngIf="report && report.status === ReportStatus.Completed && report.locationStatistics?.length" class="statistics-section">
        <mat-card class="statistics-card custom-card">
          <mat-card-header>
            <div mat-card-avatar class="statistics-avatar">
              <mat-icon>bar_chart</mat-icon>
            </div>
            <mat-card-title>Konum İstatistikleri</mat-card-title>
            <mat-card-subtitle>Konumlara göre detaylı bilgiler</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <div class="table-container">
              <table mat-table [dataSource]="report.locationStatistics" class="statistics-table" matSort>
                <!-- Location Column -->
                <ng-container matColumnDef="location">
                  <th mat-header-cell *matHeaderCellDef mat-sort-header class="location-header">
                    <mat-icon class="header-icon">location_on</mat-icon>
                    Konum
                  </th>
                  <td mat-cell *matCellDef="let stat" class="location-cell">
                    <div class="location-info">
                      <mat-icon class="location-icon">place</mat-icon>
                      <span class="location-name">{{stat.location || 'Belirtilmemiş'}}</span>
                    </div>
                  </td>
                </ng-container>

                <!-- Person Count Column -->
                <ng-container matColumnDef="personCount">
                  <th mat-header-cell *matHeaderCellDef mat-sort-header class="count-header">
                    <mat-icon class="header-icon">people</mat-icon>
                    Kişi Sayısı
                  </th>
                  <td mat-cell *matCellDef="let stat" class="count-cell">
                    <div class="count-badge person-count">
                      <mat-icon class="badge-icon">person</mat-icon>
                      <span>{{stat.personCount}}</span>
                    </div>
                  </td>
                </ng-container>

                <!-- Phone Count Column -->
                <ng-container matColumnDef="phoneNumberCount">
                  <th mat-header-cell *matHeaderCellDef mat-sort-header class="count-header">
                    <mat-icon class="header-icon">phone</mat-icon>
                    Telefon Sayısı
                  </th>
                  <td mat-cell *matCellDef="let stat" class="count-cell">
                    <div class="count-badge phone-count">
                      <mat-icon class="badge-icon">phone</mat-icon>
                      <span>{{stat.phoneNumberCount}}</span>
                    </div>
                  </td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: displayedColumns;" class="statistics-row"></tr>
              </table>
            </div>

            <!-- Empty Statistics -->
            <div *ngIf="!report.locationStatistics?.length" class="empty-statistics">
              <mat-icon class="empty-icon">location_off</mat-icon>
              <p>Bu raporda konum istatistiği bulunmuyor.</p>
            </div>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .report-detail-container {
      max-width: 1000px;
      margin: 0 auto;
      padding: 20px;
    }

    .header-section {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 30px;
    }

    .back-btn {
      background: rgba(0, 0, 0, 0.04);
      border-radius: 8px;
    }

    .title-section {
      flex: 1;
    }

    .page-title {
      display: flex;
      align-items: center;
      gap: 12px;
      margin: 0 0 8px 0;
      color: var(--text-primary);
      font-size: 1.8rem;
      font-weight: 500;
    }

    .title-icon {
      font-size: 1.8rem;
      width: 1.8rem;
      height: 1.8rem;
      color: var(--primary-color);
    }

    .title-meta {
      display: flex;
      align-items: center;
      gap: 16px;
      flex-wrap: wrap;
    }

    .status-chip {
      display: flex;
      align-items: center;
      gap: 6px;
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

    .chip-icon {
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
    }

    .date-info {
      color: var(--text-secondary);
      font-size: 0.9rem;
    }

    .spinning {
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
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

    .content-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 24px;
      margin-bottom: 30px;
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

    .status-card.preparing {
      border-left: 4px solid #ff9800;
    }

    .status-card.completed {
      border-left: 4px solid #4caf50;
    }

    .status-card.failed {
      border-left: 4px solid #f44336;
    }

    .status-avatar {
      background: var(--primary-color);
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .info-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 0;
      border-bottom: 1px solid #f0f0f0;
    }

    .info-item:last-child {
      border-bottom: none;
    }

    .info-icon {
      color: var(--primary-color);
      font-size: 1.2rem;
      width: 1.2rem;
      height: 1.2rem;
    }

    .info-content {
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .info-label {
      font-size: 0.85rem;
      color: var(--text-secondary);
      font-weight: 500;
    }

    .info-value {
      font-size: 0.95rem;
      color: var(--text-primary);
    }

    .preparing-content {
      text-align: center;
    }

    .progress-bar {
      margin-bottom: 16px;
    }

    .status-message {
      margin: 16px 0;
      color: var(--text-secondary);
    }

    .auto-refresh-info {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      font-size: 0.85rem;
      color: var(--primary-color);
      margin-top: 12px;
    }

    .refresh-icon {
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
      animation: spin 2s linear infinite;
    }

    .completed-content {
      padding: 16px 0;
    }

    .summary-stats {
      display: flex;
      justify-content: space-around;
      gap: 16px;
    }

    .summary-stat {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 8px;
      text-align: center;
    }

    .stat-icon {
      font-size: 2rem;
      width: 2rem;
      height: 2rem;
      color: var(--primary-color);
      background: rgba(33, 150, 243, 0.1);
      border-radius: 50%;
      padding: 8px;
    }

    .stat-content {
      display: flex;
      flex-direction: column;
      align-items: center;
    }

    .stat-number {
      font-size: 1.5rem;
      font-weight: 600;
      color: var(--text-primary);
      line-height: 1;
    }

    .stat-label {
      font-size: 0.8rem;
      color: var(--text-secondary);
      margin-top: 4px;
    }

    .failed-content {
      text-align: center;
    }

    .error-message {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 16px;
      background: #ffebee;
      border-radius: 8px;
      margin-bottom: 16px;
    }

    .error-icon {
      color: #f44336;
      font-size: 1.5rem;
      width: 1.5rem;
      height: 1.5rem;
      margin-top: 2px;
    }

    .error-text {
      flex: 1;
      text-align: left;
    }

    .error-title {
      margin: 0 0 4px 0;
      font-weight: 500;
      color: #c62828;
    }

    .error-details {
      margin: 0;
      font-size: 0.9rem;
      color: #d32f2f;
      line-height: 1.4;
    }

    .retry-btn {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .statistics-section {
      margin-top: 30px;
    }

    .statistics-avatar {
      background: linear-gradient(45deg, #673ab7, #512da8);
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .table-container {
      overflow-x: auto;
      margin-top: 16px;
    }

    .statistics-table {
      width: 100%;
      background: var(--surface-color);
    }

    .location-header,
    .count-header {
      display: flex;
      align-items: center;
      gap: 8px;
      font-weight: 600;
      color: var(--text-primary);
    }

    .header-icon {
      font-size: 1.1rem;
      width: 1.1rem;
      height: 1.1rem;
      color: var(--primary-color);
    }

    .location-cell {
      padding: 16px 12px;
    }

    .location-info {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .location-icon {
      color: var(--primary-color);
      font-size: 1.1rem;
      width: 1.1rem;
      height: 1.1rem;
    }

    .location-name {
      font-weight: 500;
    }

    .count-cell {
      padding: 16px 12px;
    }

    .count-badge {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 6px 12px;
      border-radius: 16px;
      font-size: 0.85rem;
      font-weight: 500;
    }

    .count-badge.person-count {
      background: #e8f5e8;
      color: #2e7d32;
    }

    .count-badge.phone-count {
      background: #e3f2fd;
      color: #1565c0;
    }

    .badge-icon {
      font-size: 0.9rem;
      width: 0.9rem;
      height: 0.9rem;
    }

    .statistics-row {
      transition: background-color 0.2s ease;
    }

    .statistics-row:hover {
      background-color: rgba(0, 0, 0, 0.02);
    }

    .empty-statistics {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
      padding: 40px 20px;
      text-align: center;
      color: var(--text-secondary);
    }

    .empty-icon {
      font-size: 3rem;
      width: 3rem;
      height: 3rem;
      opacity: 0.5;
    }

    .ReportStatus {
      display: none;
    }

    @media (max-width: 768px) {
      .report-detail-container {
        padding: 15px;
      }

      .header-section {
        flex-direction: column;
        align-items: flex-start;
        gap: 12px;
      }

      .page-title {
        font-size: 1.5rem;
      }

      .title-meta {
        flex-direction: column;
        align-items: flex-start;
        gap: 8px;
      }

      .content-grid {
        grid-template-columns: 1fr;
        gap: 20px;
      }

      .summary-stats {
        flex-direction: column;
        gap: 20px;
      }

      .table-container {
        overflow-x: scroll;
      }
    }
  `]
})
export class ReportDetailComponent implements OnInit, OnDestroy {
  report: Report | null = null;
  loading = false;
  displayedColumns: string[] = ['location', 'personCount', 'phoneNumberCount'];
  private refreshSubscription?: Subscription;
  
  // Expose enums to template
  ReportStatus = ReportStatus;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private reportService: ReportService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadReport(id);
    } else {
      this.router.navigate(['/reports']);
    }
  }

  ngOnDestroy(): void {
    this.stopAutoRefresh();
  }

  loadReport(id: string): void {
    this.loading = true;
    this.reportService.getReport(id).subscribe({
      next: (report) => {
        this.report = report;
        this.loading = false;
        
        // Start auto refresh for preparing reports
        if (report.status === ReportStatus.Preparing) {
          this.startAutoRefresh(id);
        }
      },
      error: (error) => {
        console.error('Error loading report:', error);
        this.snackBar.open('Rapor yüklenirken hata oluştu', 'Kapat', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        this.router.navigate(['/reports']);
      }
    });
  }

  refreshReport(): void {
    if (this.report) {
      this.loadReport(this.report.id);
    }
  }

  requestNewReport(): void {
    this.reportService.requestReport().subscribe({
      next: (report) => {
        this.snackBar.open('Yeni rapor talebi oluşturuldu', 'Kapat', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.router.navigate(['/reports', report.id]);
      },
      error: (error) => {
        console.error('Error requesting new report:', error);
        this.snackBar.open('Rapor talebi oluşturulurken hata oluştu', 'Kapat', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
      }
    });
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

  getProcessingTime(report: Report): string {
    if (!report.completedAt) return 'Devam ediyor';
    
    const start = new Date(report.requestedAt).getTime();
    const end = new Date(report.completedAt).getTime();
    const diff = end - start;
    
    const seconds = Math.floor(diff / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    
    if (hours > 0) {
      return `${hours} saat ${minutes % 60} dakika`;
    } else if (minutes > 0) {
      return `${minutes} dakika ${seconds % 60} saniye`;
    } else {
      return `${seconds} saniye`;
    }
  }

  getTotalPersonCount(): number {
    return this.report?.locationStatistics?.reduce((sum, stat) => sum + stat.personCount, 0) || 0;
  }

  getTotalPhoneCount(): number {
    return this.report?.locationStatistics?.reduce((sum, stat) => sum + stat.phoneNumberCount, 0) || 0;
  }

  private startAutoRefresh(reportId: string): void {
    this.stopAutoRefresh();
    
    this.refreshSubscription = interval(5000) // 5 seconds
      .pipe(switchMap(() => this.reportService.getReport(reportId)))
      .subscribe({
        next: (report) => {
          this.report = report;
          
          // Stop auto refresh if report is no longer preparing
          if (report.status !== ReportStatus.Preparing) {
            this.stopAutoRefresh();
            
            if (report.status === ReportStatus.Completed) {
              this.snackBar.open('Rapor başarıyla tamamlandı!', 'Kapat', {
                duration: 5000,
                panelClass: ['success-snackbar']
              });
            } else if (report.status === ReportStatus.Failed) {
              this.snackBar.open('Rapor oluşturulamadı', 'Kapat', {
                duration: 5000,
                panelClass: ['error-snackbar']
              });
            }
          }
        },
        error: (error) => {
          console.error('Auto refresh error:', error);
          this.stopAutoRefresh();
        }
      });
  }

  private stopAutoRefresh(): void {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
      this.refreshSubscription = undefined;
    }
  }
}
