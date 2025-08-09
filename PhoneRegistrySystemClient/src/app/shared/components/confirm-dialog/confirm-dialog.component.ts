import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface ConfirmDialogData {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  confirmColor?: 'primary' | 'accent' | 'warn';
}

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule
  ],
  template: `
    <div class="confirm-dialog">
      <h2 mat-dialog-title class="dialog-title">
        <mat-icon class="title-icon">warning</mat-icon>
        {{ data.title }}
      </h2>
      
      <div mat-dialog-content class="dialog-content">
        <p>{{ data.message }}</p>
      </div>
      
      <div mat-dialog-actions class="dialog-actions">
        <button 
          mat-button 
          (click)="onCancel()"
          class="cancel-btn">
          {{ data.cancelText || 'İptal' }}
        </button>
        <button 
          mat-raised-button 
          [color]="data.confirmColor || 'warn'"
          (click)="onConfirm()"
          class="confirm-btn">
          <mat-icon>check</mat-icon>
          {{ data.confirmText || 'Onayla' }}
        </button>
      </div>
    </div>
  `,
  styles: [`
    .confirm-dialog {
      min-width: 300px;
      max-width: 500px;
    }

    .dialog-title {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 0;
      color: var(--text-primary);
      font-weight: 500;
    }

    .title-icon {
      color: #ff9800;
      font-size: 1.5rem;
      width: 1.5rem;
      height: 1.5rem;
    }

    .dialog-content {
      padding: 20px 0;
      color: var(--text-secondary);
      line-height: 1.5;
    }

    .dialog-content p {
      margin: 0;
    }

    .dialog-actions {
      display: flex;
      gap: 12px;
      justify-content: flex-end;
      padding: 20px 0 0 0;
      margin: 0;
    }

    .cancel-btn {
      min-width: 80px;
    }

    .confirm-btn {
      display: flex;
      align-items: center;
      gap: 8px;
      min-width: 100px;
    }

    @media (max-width: 480px) {
      .confirm-dialog {
        min-width: 280px;
      }

      .dialog-actions {
        flex-direction: column-reverse;
        gap: 8px;
      }

      .cancel-btn,
      .confirm-btn {
        width: 100%;
        justify-content: center;
      }
    }
  `]
})
export class ConfirmDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData
  ) {}

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }
}