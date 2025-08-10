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
import { MatTooltipModule } from '@angular/material/tooltip';
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
    MatProgressBarModule,
    MatTooltipModule
  ],
  templateUrl: './report-list.component.html',
  styleUrls: ['./report-list.component.scss']
})
export class ReportListComponent implements OnInit, OnDestroy {
  reports: Report[] = [];
  loading = false;
  requesting = false;
  private refreshSubscription?: Subscription;

  // Enums for template
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
      next: (reports: Report[]) => {
        this.reports = reports;
        this.loading = false;
      },
      error: (error: any) => {
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
      next: (report: Report) => {
        this.reports.unshift(report);
        this.snackBar.open('Yeni rapor talebi oluşturuldu', 'Kapat', {
          duration: 3000,
          panelClass: ['success-snackbar']
        });
        this.requesting = false;
        this.startAutoRefresh(); // Auto refresh'i başlat
      },
      error: (error: any) => {
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
    this.requestNewReport();
  }

  trackByReportId(index: number, report: Report): string {
    return report.id;
  }

  getStatusLabel(status: ReportStatus): string {
    return ReportStatusLabels[status] || 'Bilinmiyor';
  }

  getStatusIcon(status: ReportStatus): string {
    return ReportStatusIcons[status] || 'help';
  }

  getStatusClass(status: ReportStatus): string {
    switch (status) {
      case ReportStatus.Preparing:
        return 'status-preparing';
      case ReportStatus.Completed:
        return 'status-completed';
      case ReportStatus.Failed:
        return 'status-failed';
      default:
        return '';
    }
  }

  getStatusChipClass(status: ReportStatus): string {
    switch (status) {
      case ReportStatus.Preparing:
        return 'status-badge-preparing';
      case ReportStatus.Completed:
        return 'status-badge-completed';
      case ReportStatus.Failed:
        return 'status-badge-failed';
      default:
        return '';
    }
  }

  getReportCountByStatus(status: ReportStatus): number {
    return this.reports.filter(report => report.status === status).length;
  }

  getTotalPersonCount(report: Report): number {
    return report.locationStatistics?.reduce((total, stat) => total + stat.personCount, 0) || 0;
  }

  getTotalPhoneCount(report: Report): number {
    return report.locationStatistics?.reduce((total, stat) => total + stat.phoneNumberCount, 0) || 0;
  }

  hasPreparingReports(): boolean {
    return this.reports.some(report => report.status === ReportStatus.Preparing);
  }

  private startAutoRefresh(): void {
    if (this.hasPreparingReports()) {
      this.refreshSubscription = interval(5000) // 5 saniyede bir
        .pipe(
          switchMap(() => this.reportService.getReports())
        )
        .subscribe({
          next: (reports) => {
            this.reports = reports;
            
            // Eğer hazırlanan rapor kalmadıysa auto refresh'i durdur
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
