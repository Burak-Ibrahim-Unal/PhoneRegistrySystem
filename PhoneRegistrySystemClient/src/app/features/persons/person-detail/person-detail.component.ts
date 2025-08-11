import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';

import { PersonService } from '../../../core/services/person.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { Person, ContactInfo, ContactType, ContactTypeLabels, ContactTypeIcons, AddContactInfoRequest } from '../../../core/models/person.model';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-person-detail',
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
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatChipsModule,
    MatDividerModule,
    MatListModule
  ],
  template: `
    <div class="person-detail-container fade-in">
      <div class="header-section">
        <button mat-icon-button routerLink="/persons" class="back-btn">
          <mat-icon>arrow_back</mat-icon>
        </button>
        <div class="title-section" *ngIf="person">
          <h1 class="page-title">
            <mat-icon class="title-icon">person</mat-icon>
            {{person.firstName}} {{person.lastName}}
          </h1>
          <p class="subtitle" *ngIf="person.company">{{person.company}}</p>
        </div>
        <div class="actions">
          <button mat-button color="warn" (click)="deletePerson()" [disabled]="loading">
            <mat-icon>delete</mat-icon>
            Sil
          </button>
        </div>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading && !person" class="loading-container">
        <mat-spinner diameter="50"></mat-spinner>
        <p>Kişi bilgileri yükleniyor...</p>
      </div>

      <!-- Person Details -->
      <div *ngIf="person" class="content-grid">
        <!-- Person Info Card -->
        <mat-card class="person-info-card custom-card">
          <mat-card-header>
            <div mat-card-avatar class="person-avatar">
              <mat-icon>person</mat-icon>
            </div>
            <mat-card-title>Kişi Bilgileri</mat-card-title>
            <mat-card-subtitle>Temel bilgiler</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <div class="info-item">
              <mat-icon class="info-icon">badge</mat-icon>
              <div class="info-content">
                <span class="info-label">Ad Soyad</span>
                <span class="info-value">{{person.firstName}} {{person.lastName}}</span>
              </div>
            </div>
            <div class="info-item" *ngIf="person.company">
              <mat-icon class="info-icon">business</mat-icon>
              <div class="info-content">
                <span class="info-label">Şirket</span>
                <span class="info-value">{{person.company}}</span>
              </div>
            </div>
            <div class="info-item">
              <mat-icon class="info-icon">schedule</mat-icon>
              <div class="info-content">
                <span class="info-label">Oluşturma Tarihi</span>
                <span class="info-value">{{person.createdAt | date:'dd.MM.yyyy HH:mm'}}</span>
              </div>
            </div>
            <div class="info-item">
              <mat-icon class="info-icon">update</mat-icon>
              <div class="info-content">
                <span class="info-label">Son Güncelleme</span>
                <span class="info-value">{{person.updatedAt | date:'dd.MM.yyyy HH:mm'}}</span>
              </div>
            </div>
          </mat-card-content>
        </mat-card>

        <!-- Contact Info Card -->
        <mat-card class="contact-info-card custom-card">
          <mat-card-header>
            <div mat-card-avatar class="contact-avatar">
              <mat-icon>contact_phone</mat-icon>
            </div>
            <mat-card-title>İletişim Bilgileri</mat-card-title>
            <mat-card-subtitle>{{person.contactInfos.length}} iletişim bilgisi</mat-card-subtitle>
          </mat-card-header>
          <mat-card-content>
            <!-- Contact List -->
            <div *ngIf="person.contactInfos.length > 0" class="contact-list">
              <div *ngFor="let contact of person.contactInfos" class="contact-item">
                <div class="contact-main">
                  <mat-icon class="contact-type-icon">{{getContactTypeIcon(contact.type)}}</mat-icon>
                  <div class="contact-details">
                    <span class="contact-type">{{getContactTypeLabel(contact.type)}}</span>
                    <span class="contact-content">{{contact.content}}</span>
                  </div>
                </div>
                <button mat-icon-button color="warn" (click)="removeContactInfo(contact)" class="remove-btn">
                  <mat-icon>delete</mat-icon>
                </button>
              </div>
            </div>

            <!-- Empty State -->
            <div *ngIf="person.contactInfos.length === 0" class="empty-contacts">
              <mat-icon class="empty-icon">contact_phone</mat-icon>
              <p>Henüz iletişim bilgisi eklenmemiş</p>
            </div>

            <mat-divider class="divider"></mat-divider>

            <!-- Add Contact Form -->
            <div class="add-contact-section">
              <h3 class="section-title">Yeni İletişim Bilgisi Ekle</h3>
              <form [formGroup]="contactForm" (ngSubmit)="addContactInfo()" class="contact-form">
                <div class="form-row">
                  <mat-form-field appearance="fill" class="type-field">
                    <mat-label>Tür</mat-label>
                    <mat-select formControlName="type">
                      <mat-option [value]="ContactType.PhoneNumber">
                        <mat-icon>{{ContactTypeIcons[ContactType.PhoneNumber]}}</mat-icon>
                        {{ContactTypeLabels[ContactType.PhoneNumber]}}
                      </mat-option>
                      <mat-option [value]="ContactType.EmailAddress">
                        <mat-icon>{{ContactTypeIcons[ContactType.EmailAddress]}}</mat-icon>
                        {{ContactTypeLabels[ContactType.EmailAddress]}}
                      </mat-option>
                      <mat-option [value]="ContactType.Location">
                        <mat-icon>{{ContactTypeIcons[ContactType.Location]}}</mat-icon>
                        {{ContactTypeLabels[ContactType.Location]}}
                      </mat-option>
                    </mat-select>
                  </mat-form-field>

                  <mat-form-field appearance="fill" class="content-field" *ngIf="contactForm.get('type')?.value !== ContactType.Location">
                    <mat-label>İçerik</mat-label>
                    <input matInput formControlName="content" [placeholder]="getPlaceholder()">
                    <mat-error *ngIf="contactForm.get('content')?.hasError('required')">
                      Bu alan zorunludur
                    </mat-error>
                    <mat-error *ngIf="contactForm.get('content')?.hasError('email')">
                      Geçerli bir e-posta adresi girin
                    </mat-error>
                  </mat-form-field>

                  <mat-form-field appearance="fill" class="content-field" *ngIf="contactForm.get('type')?.value === ContactType.Location">
                    <mat-label>Şehir</mat-label>
                    <mat-select formControlName="cityId">
                      <mat-option *ngFor="let c of cities" [value]="c.id">{{c.name}}</mat-option>
                    </mat-select>
                  </mat-form-field>

                  <button 
                    type="submit" 
                    mat-raised-button 
                    color="primary" 
                    class="add-btn"
                    [disabled]="contactForm.invalid || addingContact">
                    <mat-spinner diameter="20" *ngIf="addingContact"></mat-spinner>
                    <mat-icon *ngIf="!addingContact">add</mat-icon>
                    {{ addingContact ? 'Ekleniyor...' : 'Ekle' }}
                  </button>
                </div>
              </form>
            </div>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .person-detail-container {
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
      margin: 0 0 4px 0;
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

    .subtitle {
      margin: 0;
      color: var(--text-secondary);
      font-size: 1rem;
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
      grid-template-columns: 1fr 2fr;
      gap: 24px;
    }

    .person-avatar {
      background: linear-gradient(45deg, var(--primary-color), #1976d2);
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .contact-avatar {
      background: linear-gradient(45deg, #4caf50, #388e3c);
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

    .contact-list {
      margin-bottom: 24px;
    }

    .contact-item {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 12px;
      border: 1px solid #e0e0e0;
      border-radius: 8px;
      margin-bottom: 8px;
      background: #fafafa;
    }

    .contact-main {
      display: flex;
      align-items: center;
      gap: 12px;
      flex: 1;
    }

    .contact-type-icon {
      color: var(--primary-color);
      font-size: 1.3rem;
      width: 1.3rem;
      height: 1.3rem;
    }

    .contact-details {
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .contact-type {
      font-size: 0.85rem;
      color: var(--text-secondary);
      font-weight: 500;
    }

    .contact-content {
      font-size: 0.95rem;
      color: var(--text-primary);
    }

    .remove-btn {
      opacity: 0.7;
      transition: opacity 0.2s ease;
    }

    .remove-btn:hover {
      opacity: 1;
    }

    .empty-contacts {
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

    .divider {
      margin: 24px 0;
    }

    .section-title {
      margin: 0 0 20px 0;
      font-size: 1.1rem;
      font-weight: 500;
      color: var(--text-primary);
    }

    .contact-form {
      margin-top: 16px;
    }

    .form-row {
      display: grid;
      grid-template-columns: 120px 1fr auto;
      gap: 12px;
      align-items: end;
    }

    .type-field,
    .content-field {
      width: 100%;
    }

    .add-btn {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 12px 16px;
      border-radius: 8px;
      font-weight: 500;
      min-width: 100px;
      height: 56px;
    }

    .ContactType {
      display: none;
    }

    @media (max-width: 768px) {
      .person-detail-container {
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

      .content-grid {
        grid-template-columns: 1fr;
        gap: 20px;
      }

      .form-row {
        grid-template-columns: 1fr;
        gap: 12px;
      }

      .add-btn {
        width: 100%;
        justify-content: center;
      }
    }
  `]
})
export class PersonDetailComponent implements OnInit {
  person: Person | null = null;
  loading = false;
  addingContact = false;
  contactForm: FormGroup;
  cities: { id: string, name: string }[] = [];
  
  // Expose enums and constants to template
  ContactType = ContactType;
  ContactTypeLabels = ContactTypeLabels;
  ContactTypeIcons = ContactTypeIcons;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private personService: PersonService,
    private notificationService: NotificationService,
    private dialog: MatDialog,
    private fb: FormBuilder
  ) {
    this.contactForm = this.fb.group({
      type: [ContactType.PhoneNumber, Validators.required],
      content: ['', Validators.required],
      cityId: [null]
    });

    // Update validators based on contact type
    this.contactForm.get('type')?.valueChanges.subscribe(type => {
      const contentControl = this.contactForm.get('content');
      const cityControl = this.contactForm.get('cityId');
      if (type === ContactType.EmailAddress) {
        contentControl?.setValidators([Validators.required, Validators.email]);
        cityControl?.clearValidators();
        cityControl?.setValue(null);
      } else if (type === ContactType.Location) {
        contentControl?.clearValidators();
        contentControl?.setValue('');
        cityControl?.setValidators([Validators.required]);
      } else {
        contentControl?.setValidators([Validators.required]);
        cityControl?.clearValidators();
        cityControl?.setValue(null);
      }
      contentControl?.updateValueAndValidity();
      cityControl?.updateValueAndValidity();
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadPerson(id);
      this.loadCities();
    } else {
      this.router.navigate(['/persons']);
    }
  }

  loadCities(): void {
    this.personService.getCities().subscribe({
      next: list => this.cities = list,
      error: _ => this.cities = []
    });
  }

  loadPerson(id: string): void {
    this.loading = true;
    this.personService.getPerson(id).subscribe({
      next: (person) => {
        this.person = person;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading person:', error);
        this.notificationService.showError('Kişi bilgileri yüklenirken hata oluştu');
        this.router.navigate(['/persons']);
      }
    });
  }

  addContactInfo(): void {
    if (this.contactForm.valid && this.person && !this.addingContact) {
      this.addingContact = true;
      
      const request: AddContactInfoRequest = {
        type: this.contactForm.value.type,
        content: (this.contactForm.value.content || '').trim(),
        cityId: this.contactForm.value.type === ContactType.Location ? this.contactForm.value.cityId : null
      };

      this.personService.addContactInfo(this.person.id, request).subscribe({
        next: () => {
          this.notificationService.showSuccess('İletişim bilgisi başarıyla eklendi!');
          this.contactForm.reset({ type: ContactType.PhoneNumber });
          this.loadPerson(this.person!.id);
          this.addingContact = false;
        },
        error: (error) => {
          console.error('Error adding contact info:', error);
          this.notificationService.showError('İletişim bilgisi eklenirken hata oluştu');
          this.addingContact = false;
        }
      });
    }
  }

  removeContactInfo(contact: ContactInfo): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'İletişim Bilgisini Sil',
        message: `${this.getContactTypeLabel(contact.type)} bilgisini silmek istediğinizden emin misiniz?`,
        confirmText: 'Sil',
        cancelText: 'İptal'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && this.person) {
        this.personService.removeContactInfo(this.person.id, contact.id).subscribe({
          next: () => {
            this.notificationService.showSuccess('İletişim bilgisi başarıyla silindi!');
            this.loadPerson(this.person!.id);
          },
          error: (error) => {
            console.error('Error removing contact info:', error);
            this.notificationService.showError('İletişim bilgisi silinirken hata oluştu');
          }
        });
      }
    });
  }

  deletePerson(): void {
    if (!this.person) return;

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Kişiyi Sil',
        message: `${this.person.firstName} ${this.person.lastName} adlı kişiyi silmek istediğinizden emin misiniz?`,
        confirmText: 'Sil',
        cancelText: 'İptal'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && this.person) {
        this.personService.deletePerson(this.person.id).subscribe({
          next: () => {
            this.notificationService.showSuccess('Kişi başarıyla silindi!');
            this.router.navigate(['/persons']);
          },
          error: (error) => {
            console.error('Error deleting person:', error);
            this.notificationService.showError('Kişi silinirken hata oluştu');
          }
        });
      }
    });
  }

  getContactTypeLabel(type: ContactType): string {
    return ContactTypeLabels[type] || 'Bilinmeyen';
  }

  getContactTypeIcon(type: ContactType): string {
    return ContactTypeIcons[type] || 'help';
  }

  getPlaceholder(): string {
    const type = this.contactForm.get('type')?.value;
    switch (type) {
      case ContactType.PhoneNumber:
        return 'Örn: +90 555 123 45 67';
      case ContactType.EmailAddress:
        return 'Örn: ornek@email.com';
      case ContactType.Location:
        return 'Örn: İstanbul, Türkiye';
      default:
        return 'İçerik girin';
    }
  }
}