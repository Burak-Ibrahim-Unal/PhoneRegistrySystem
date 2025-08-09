import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private toastr: ToastrService) {}

  showSuccess(message: string, title?: string): void {
    console.log('NotificationService.showSuccess called with:', message);
    this.toastr.success(message, title || 'Başarılı', {
      timeOut: 3000,
      progressBar: true,
      closeButton: true,
      positionClass: 'toast-top-right'
    });
  }

  showError(message: string, title?: string): void {
    console.log('NotificationService.showError called with:', message);
    this.toastr.error(message, title || 'Hata', {
      timeOut: 5000,
      progressBar: true,
      closeButton: true,
      positionClass: 'toast-top-right'
    });
  }

  showInfo(message: string, title?: string): void {
    console.log('NotificationService.showInfo called with:', message);
    this.toastr.info(message, title || 'Bilgi', {
      timeOut: 3000,
      progressBar: true,
      closeButton: true,
      positionClass: 'toast-top-right'
    });
  }

  showWarning(message: string, title?: string): void {
    console.log('NotificationService.showWarning called with:', message);
    this.toastr.warning(message, title || 'Uyarı', {
      timeOut: 4000,
      progressBar: true,
      closeButton: true,
      positionClass: 'toast-top-right'
    });
  }
}
