import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-progress-ring',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './progress-ring.component.html',
  styleUrl: './progress-ring.component.scss'
})
export class ProgressRingComponent {
  @Input() percent: number = 0; // 0 à 100
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  @Input() showLabel: boolean = true;
}
