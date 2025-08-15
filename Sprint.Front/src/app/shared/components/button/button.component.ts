import { Component } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  template: `<button class="btn" [class.btn-primary]="variant === 'primary'">
    <ng-content />
  </button>`,
  styleUrls: ['./button.component.scss']
})
export class ButtonComponent {
  variant: 'primary' | 'secondary' = 'primary';
}
