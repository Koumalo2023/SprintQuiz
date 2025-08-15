import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { NotificationService, Notification } from '../../../core/services/notification.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notification',
  imports: [CommonModule],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss'
})
export class NotificationComponent implements OnInit, OnDestroy {
  notifications: Notification[] = [];
  private sub = new Subscription();

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.sub.add(
      this.notificationService.notifications$.subscribe(notifs => {
        this.notifications = notifs;
      })
    );
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  remove(id: string): void {
    this.notificationService.remove(id);
  }
}
