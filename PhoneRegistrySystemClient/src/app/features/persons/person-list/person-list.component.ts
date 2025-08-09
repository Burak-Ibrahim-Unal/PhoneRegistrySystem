import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';

import { PersonService } from '../../../core/services/person.service';
import { NotificationService } from '../../../shared/services/notification.service';
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
    MatDialogModule,
    MatPaginatorModule,
    MatInputModule,
    MatFormFieldModule,
    FormsModule
  ],
  templateUrl: './person-list.component.html',
  styleUrls: ['./person-list.component.scss']
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
    private notificationService: NotificationService,
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
        this.notificationService.showError('Kişiler yüklenirken hata oluştu');
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
            this.notificationService.showSuccess(
              `${person.firstName} ${person.lastName} başarıyla silindi!`,
              'Kişi Silindi'
            );
            this.loadPersons();
          },
          error: (error) => {
            console.error('Error deleting person:', error);
            this.notificationService.showError('Kişi silinirken hata oluştu');
          }
        });
      }
    });
  }
}