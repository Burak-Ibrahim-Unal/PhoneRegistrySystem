import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/persons', pathMatch: 'full' },
  { 
    path: 'persons', 
    loadComponent: () => import('./features/persons/person-list/person-list.component').then(m => m.PersonListComponent)
  },
  { 
    path: 'persons/create', 
    loadComponent: () => import('./features/persons/person-create/person-create.component').then(m => m.PersonCreateComponent)
  },
  { 
    path: 'persons/:id', 
    loadComponent: () => import('./features/persons/person-detail/person-detail.component').then(m => m.PersonDetailComponent)
  },
  { 
    path: 'reports', 
    loadComponent: () => import('./features/reports/report-list/report-list.component').then(m => m.ReportListComponent)
  },
  { 
    path: 'reports/:id', 
    loadComponent: () => import('./features/reports/report-detail/report-detail.component').then(m => m.ReportDetailComponent)
  },
  { path: '**', redirectTo: '/persons' }
];

