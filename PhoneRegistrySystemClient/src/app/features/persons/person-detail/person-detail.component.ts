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
  templateUrl: './person-detail.component.html',
  styleUrls: ['./person-detail.component.css']
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
      content: [''],
      cityId: [null],
      phone: ['']
    });

    // Update validators based on contact type
    this.contactForm.get('type')?.valueChanges.subscribe(type => {
      const contentControl = this.contactForm.get('content');
      const cityControl = this.contactForm.get('cityId');
      // İçerik alanını sadece email için zorunlu yap
      if (type === ContactType.EmailAddress) {
        contentControl?.setValidators([Validators.required, Validators.email]);
      } else if (type === ContactType.Location) {
        contentControl?.clearValidators();
        contentControl?.setValue('');
      } else {
        contentControl?.setValidators([]);
      }
      // Şehir opsiyonel kalsın
      cityControl?.setValidators([]);
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
      
      const type = this.contactForm.value.type;
      const cityId = this.contactForm.value.cityId || null;
      const phone = (this.contactForm.value.phone || '').trim();
      const emailOrOther = (this.contactForm.value.content || '').trim();

      const mainRequest: AddContactInfoRequest = {
        type,
        content: type === ContactType.PhoneNumber ? phone : emailOrOther,
        cityId: type === ContactType.Location ? cityId : null
      };

      const main$ = this.personService.addContactInfo(this.person.id, mainRequest);

      if (phone && type !== ContactType.PhoneNumber) {
        main$.subscribe({
          next: () => {
            this.personService.addContactInfo(this.person!.id, { type: ContactType.PhoneNumber, content: phone }).subscribe({
              next: () => {
                this.notificationService.showSuccess('İletişim bilgisi eklendi');
                this.contactForm.reset({ type: ContactType.PhoneNumber, content: '', cityId: null, phone: '' });
                this.loadPerson(this.person!.id);
                this.addingContact = false;
              },
              error: (error) => {
                console.error('Error adding phone:', error);
                this.notificationService.showError('Telefon eklenirken hata oluştu');
                this.addingContact = false;
              }
            });
          },
          error: (error) => {
            console.error('Error adding contact info:', error);
            this.notificationService.showError('İletişim bilgisi eklenirken hata oluştu');
            this.addingContact = false;
          }
        });
      } else {
        main$.subscribe({
          next: () => {
            this.notificationService.showSuccess('İletişim bilgisi eklendi');
            this.contactForm.reset({ type: ContactType.PhoneNumber, content: '', cityId: null, phone: '' });
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