import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

//src/app/core/services/notificationService.service.ts


export type NotificationType = 'success' | 'error' | 'info' | 'warning';

export interface Notification {
  id: string;
  message: string;
  type: NotificationType;
  duration?: number; // ms (0 = permanent)
  createdAt: Date;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  public notifications$ = this.notificationsSubject.asObservable();

  private readonly DEFAULT_DURATION = 5000; // 5s
  private idCounter = 0;

  show(
    message: string,
    type: NotificationType = 'info',
    duration: number = this.DEFAULT_DURATION
  ): void {
    const id = `notif-${this.idCounter++}`;
    const notification: Notification = {
      id,
      message,
      type,
      duration,
      createdAt: new Date()
    };

    const current = this.notificationsSubject.value;
    this.notificationsSubject.next([...current, notification]);

    if (duration > 0) {
      setTimeout(() => this.remove(id), duration);
    }
  }

  success(message: string, duration?: number): void {
    this.show(message, 'success', duration);
  }

  error(message: string, duration: number = 8000): void {
    this.show(message, 'error', duration);
  }

  info(message: string, duration?: number): void {
    this.show(message, 'info', duration);
  }

  warning(message: string, duration?: number): void {
    this.show(message, 'warning', duration);
  }

  remove(id: string): void {
    const current = this.notificationsSubject.value;
    const updated = current.filter(n => n.id !== id);
    this.notificationsSubject.next(updated);
  }

  clearAll(): void {
    this.notificationsSubject.next([]);
  }
}
