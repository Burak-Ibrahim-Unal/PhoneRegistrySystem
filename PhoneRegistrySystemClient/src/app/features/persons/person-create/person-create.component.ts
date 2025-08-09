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
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { PersonService } from '../../../core/services/person.service';
import { NotificationService } from '../../../shared/services/notification.service';
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
  templateUrl: './person-create.component.html',
  styleUrls: ['./person-create.component.scss']
})
export class PersonCreateComponent {
  personForm: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private personService: PersonService,
    private router: Router,
    private notificationService: NotificationService
  ) {
    this.personForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      company: ['']
    });
  }

  onSubmit(): void {
    if (this.personForm.valid && !this.isLoading) {
      this.isLoading = true;
      
      const request: CreatePersonRequest = {
        firstName: this.personForm.value.firstName.trim(),
        lastName: this.personForm.value.lastName.trim(),
        company: this.personForm.value.company?.trim() || undefined
      };

      this.personService.createPerson(request).subscribe({
        next: (person) => {
          this.notificationService.showSuccess(
            `${person.firstName} ${person.lastName} başarıyla eklendi!`,
            'Kişi Eklendi'
          );
          this.router.navigate(['/persons', person.id]);
        },
        error: (error) => {
          console.error('Error creating person:', error);
          this.notificationService.showError('Kişi oluşturulurken hata oluştu. Lütfen tekrar deneyin.');
          this.isLoading = false;
        }
      });
    }
  }

  onCancel(): void {
    this.router.navigate(['/persons']);
  }


}