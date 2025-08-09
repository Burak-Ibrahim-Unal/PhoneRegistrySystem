import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';

import { PersonService } from '../../../core/services/person.service';
import { PersonSummary } from '../../../core/models/person.model';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-person-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatPaginatorModule,
    MatInputModule,
    MatFormFieldModule,
    FormsModule
  ],
  template: `
    <div class="person-list-container fade-in">
      <div class="header-section">
        <h1 class="page-title">
          <mat-icon class="title-icon">people</mat-icon>
          Kişiler
        </h1>
        <button mat-raised-button color="primary" routerLink="/persons/create" class="create-btn">
          <mat-icon>person_add</mat-icon>
          Yeni Kişi Ekle
        </button>
      </div>

      <!-- Search Section -->
      <div class="search-section">
        <mat-form-field appearance="outline" class="search-field">
          <mat-label>Kişi Ara</mat-label>
          <input matInput [(ngModel)]="searchTerm" (input)="onSearchChange()" placeholder="Ad, soyad veya şirket...">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>
      </div>

      <!-- Loading State -->
      <div *ngIf="loading" class="loading-container">
        <mat-spinner diameter="50"></mat-spinner>
        <p>Kişiler yükleniyor...</p>
      </div>

      <!-- Empty State -->
      <div *ngIf="!loading && filteredPersons.length === 0" class="empty-state">
        <mat-icon class="empty-icon">people_outline</mat-icon>
        <h2>Henüz kişi bulunmuyor</h2>
        <p>İlk kişinizi eklemek için "Yeni Kişi Ekle" butonuna tıklayın.</p>
        <button mat-raised-button color="primary" routerLink="/persons/create">
          <mat-icon>person_add</mat-icon>
          Kişi Ekle
        </button>
      </div>

      <!-- Person Grid -->
      <div *ngIf="!loading && filteredPersons.length > 0" class="persons-grid">
        <mat-card *ngFor="let person of paginatedPersons" class="person-card custom-card">
          <mat-card-header>
            <div mat-card-avatar class="person-avatar">
              <mat-icon>person</mat-icon>
            </div>
            <mat-card-title>{{person.firstName}} {{person.lastName}}</mat-card-title>
            <mat-card-subtitle *ngIf="person.company">{{person.company}}</mat-card-subtitle>
          </mat-card-header>

          <mat-card-content>
            <div class="contact-info">
              <mat-icon class="info-icon">contact_phone</mat-icon>
              <span>{{person.contactInfoCount}} iletişim bilgisi</span>
            </div>
          </mat-card-content>

          <mat-card-actions align="end">
            <button mat-button color="primary" [routerLink]="['/persons', person.id]">
              <mat-icon>visibility</mat-icon>
              Görüntüle
            </button>
            <button mat-button color="warn" (click)="deletePerson(person)">
              <mat-icon>delete</mat-icon>
              Sil
            </button>
          </mat-card-actions>
        </mat-card>
      </div>

      <!-- Pagination -->
      <mat-paginator 
        *ngIf="!loading && filteredPersons.length > 0"
        [length]="filteredPersons.length"
        [pageSize]="pageSize"
        [pageSizeOptions]="[6, 12, 24, 48]"
        (page)="onPageChange($event)"
        showFirstLastButtons
        class="paginator">
      </mat-paginator>
    </div>
  `,
  styles: [`
    .person-list-container {
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

    .create-btn {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 12px 24px;
      border-radius: 8px;
      font-weight: 500;
    }

    .search-section {
      margin-bottom: 30px;
    }

    .search-field {
      width: 100%;
      max-width: 400px;
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

    .persons-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 24px;
      margin-bottom: 30px;
    }

    .person-card {
      transition: all 0.3s ease;
      cursor: pointer;
    }

    .person-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
    }

    .person-avatar {
      background: linear-gradient(45deg, var(--primary-color), #1976d2);
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
    }

    .contact-info {
      display: flex;
      align-items: center;
      gap: 8px;
      color: var(--text-secondary);
      margin-top: 10px;
    }

    .info-icon {
      font-size: 1rem;
      width: 1rem;
      height: 1rem;
    }

    .paginator {
      margin-top: 30px;
      background: var(--surface-color);
      border-radius: 8px;
    }

    @media (max-width: 768px) {
      .person-list-container {
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

      .persons-grid {
        grid-template-columns: 1fr;
        gap: 16px;
      }

      .create-btn {
        width: 100%;
        justify-content: center;
      }
    }

    @media (max-width: 480px) {
      .persons-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class PersonListComponent implements OnInit {
  persons: PersonSummary[] = [];
  filteredPersons: PersonSummary[] = [];
  paginatedPersons: PersonSummary[] = [];
  loading = false;
  searchTerm = '';
  
  // Pagination
  pageIndex = 0;
  pageSize = 6;

  constructor(
    private personService: PersonService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadPersons();
  }

  loadPersons(): void {
    this.loading = true;
    this.personService.getPersons(0, 1000).subscribe({
      next: (persons) => {
        this.persons = persons;
        this.applyFilter();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading persons:', error);
        this.snackBar.open('Kişiler yüklenirken hata oluştu', 'Kapat', {
          duration: 3000,
          panelClass: ['error-snackbar']
        });
        this.loading = false;
      }
    });
  }

  onSearchChange(): void {
    this.pageIndex = 0;
    this.applyFilter();
  }

  applyFilter(): void {
    if (!this.searchTerm.trim()) {
      this.filteredPersons = [...this.persons];
    } else {
      const term = this.searchTerm.toLowerCase().trim();
      this.filteredPersons = this.persons.filter(person =>
        person.firstName.toLowerCase().includes(term) ||
        person.lastName.toLowerCase().includes(term) ||
        (person.company && person.company.toLowerCase().includes(term))
      );
    }
    this.updatePaginatedPersons();
  }

  onPageChange(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.updatePaginatedPersons();
  }

  updatePaginatedPersons(): void {
    const startIndex = this.pageIndex * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.paginatedPersons = this.filteredPersons.slice(startIndex, endIndex);
  }

  deletePerson(person: PersonSummary): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Kişiyi Sil',
        message: `${person.firstName} ${person.lastName} adlı kişiyi silmek istediğinizden emin misiniz?`,
        confirmText: 'Sil',
        cancelText: 'İptal'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.personService.deletePerson(person.id).subscribe({
          next: () => {
            this.snackBar.open('Kişi başarıyla silindi', 'Kapat', {
              duration: 3000,
              panelClass: ['success-snackbar']
            });
            this.loadPersons();
          },
          error: (error) => {
            console.error('Error deleting person:', error);
            this.snackBar.open('Kişi silinirken hata oluştu', 'Kapat', {
              duration: 3000,
              panelClass: ['error-snackbar']
            });
          }
        });
      }
    });
  }
}