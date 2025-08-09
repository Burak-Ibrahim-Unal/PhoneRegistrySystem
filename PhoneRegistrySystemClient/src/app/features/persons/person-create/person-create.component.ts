import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';

import { PersonService } from '../../../core/services/person.service';
import { CreatePersonRequest } from '../../../core/models/person.model';

@Component({
  selector: 'app-person-create',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  template: `
    <div class="person-create-container fade-in">
      <div class="header-section">
        <button mat-icon-button routerLink="/persons" class="back-btn">
          <mat-icon>arrow_back</mat-icon>
        </button>
        <h1 class="page-title">
          <mat-icon class="title-icon">person_add</mat-icon>
          Yeni Kişi Ekle
        </h1>
      </div>

      <div class="form-container">
        <mat-card class="create-card custom-card">
          <mat-card-header>
            <div mat-card-avatar class="form-avatar">
              <mat-icon>person</mat-icon>
            </div>
            <mat-card-title>Kişi Bilgileri</mat-card-title>
            <mat-card-subtitle>Yeni kişi bilgilerini girin</mat-card-subtitle>
          </mat-card-header>

          <mat-card-content>
            <form [formGroup]="personForm" (ngSubmit)="onSubmit()" class="person-form">
              <div class="form-row">
                <mat-form-field appearance="outline" class="form-field">
                  <mat-label>Ad *</mat-label>
                  <input matInput formControlName="firstName">
                  <mat-icon matSuffix>person</mat-icon>
                  <mat-error *ngIf="personForm.get('firstName')?.hasError('required')">
                    Ad alanı zorunludur
                  </mat-error>
                  <mat-error *ngIf="personForm.get('firstName')?.hasError('minlength')">
                    Ad en az 2 karakter olmalıdır
                  </mat-error>
                </mat-form-field>

                <mat-form-field appearance="outline" class="form-field">
                  <mat-label>Soyad *</mat-label>
                  <input matInput formControlName="lastName">
                  <mat-icon matSuffix>person</mat-icon>
                  <mat-error *ngIf="personForm.get('lastName')?.hasError('required')">
                    Soyad alanı zorunludur
                  </mat-error>
                  <mat-error *ngIf="personForm.get('lastName')?.hasError('minlength')">
                    Soyad en az 2 karakter olmalıdır
                  </mat-error>
                </mat-form-field>
              </div>

              <mat-form-field appearance="outline" class="form-field full-width">
                <mat-label>Şirket</mat-label>
                <input matInput formControlName="company">
                <mat-icon matSuffix>business</mat-icon>
              </mat-form-field>

              <div class="form-actions">
                <button 
                  type="button" 
                  mat-button 
                  routerLink="/persons" 
                  class="cancel-btn"
                  [disabled]="loading">
                  <mat-icon>cancel</mat-icon>
                  İptal
                </button>
                
                <button 
                  type="submit" 
                  mat-raised-button 
                  color="primary" 
                  class="submit-btn"
                  [disabled]="personForm.invalid || loading">
                  <mat-spinner diameter="20" *ngIf="loading"></mat-spinner>
                  <mat-icon *ngIf="!loading">save</mat-icon>
                  {{ loading ? 'Kaydediliyor...' : 'Kaydet' }}
                </button>
              </div>
            </form>
          </mat-card-content>
        </mat-card>

        <!-- Help Card -->
        <mat-card class="help-card custom-card">
          <mat-card-header>
            <mat-icon mat-card-avatar class="help-icon">help_outline</mat-icon>
            <mat-card-title>Yardım</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="help-item">
              <mat-icon class="help-item-icon">info</mat-icon>
              <span>Kişi kaydettikten sonra iletişim bilgilerini ekleyebilirsiniz.</span>
            </div>
            <div class="help-item">
              <mat-icon class="help-item-icon">star</mat-icon>
              <span>Ad ve soyad alanları zorunludur.</span>
            </div>
            <div class="help-item">
              <mat-icon class="help-item-icon">business</mat-icon>
              <span>Şirket bilgisi opsiyoneldir.</span>
            </div>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .person-create-container {
      max-width: 800px;
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

    .page-title {
      display: flex;
      align-items: center;
      gap: 12px;
      margin: 0;
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

    .form-container {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 24px;
    }

    .create-card {
      height: fit-content;
    }

    .form-avatar {
      background: linear-gradient(45deg, var(--primary-color), #1976d2);
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .person-form {
      padding-top: 20px;
    }

    .form-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      margin-bottom: 16px;
    }

    .form-field {
      width: 100%;
    }

    .full-width {
      width: 100%;
      margin-bottom: 24px;
    }

    .form-actions {
      display: flex;
      gap: 16px;
      justify-content: flex-end;
      padding-top: 20px;
      border-top: 1px solid #e0e0e0;
    }

    .cancel-btn {
      display: flex;
      align-items: center;
      gap: 8px;
    }

    .submit-btn {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 12px 24px;
      border-radius: 8px;
      font-weight: 500;
      min-width: 140px;
    }

    .help-card {
      height: fit-content;
      background: linear-gradient(135deg, #f8f9ff 0%, #f0f4ff 100%);
      border: 1px solid #e3f2fd;
    }

    .help-icon {
      background: var(--primary-color);
      color: white;
    }

    .help-item {
      display: flex;
      align-items: center;
      gap: 12px;
      margin-bottom: 16px;
      color: var(--text-secondary);
      font-size: 0.9rem;
    }

    .help-item:last-child {
      margin-bottom: 0;
    }

    .help-item-icon {
      font-size: 1.2rem;
      width: 1.2rem;
      height: 1.2rem;
      color: var(--primary-color);
    }

    @media (max-width: 768px) {
      .person-create-container {
        padding: 15px;
      }

      .header-section {
        margin-bottom: 20px;
      }

      .page-title {
        font-size: 1.5rem;
      }

      .form-container {
        grid-template-columns: 1fr;
        gap: 20px;
      }

      .form-row {
        grid-template-columns: 1fr;
        gap: 12px;
      }

      .form-actions {
        flex-direction: column-reverse;
        gap: 12px;
      }

      .cancel-btn,
      .submit-btn {
        width: 100%;
        justify-content: center;
      }
    }
  `]
})
export class PersonCreateComponent {
  personForm: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private personService: PersonService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.personForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      company: ['']
    });
  }

  onSubmit(): void {
    if (this.personForm.valid && !this.loading) {
      this.loading = true;
      
      const request: CreatePersonRequest = {
        firstName: this.personForm.value.firstName.trim(),
        lastName: this.personForm.value.lastName.trim(),
        company: this.personForm.value.company?.trim() || undefined
      };

      this.personService.createPerson(request).subscribe({
        next: (person) => {
          this.snackBar.open('Kişi başarıyla oluşturuldu', 'Kapat', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.router.navigate(['/persons', person.id]);
        },
        error: (error) => {
          console.error('Error creating person:', error);
          this.snackBar.open('Kişi oluşturulurken hata oluştu', 'Kapat', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
          this.loading = false;
        }
      });
    }
  }
}