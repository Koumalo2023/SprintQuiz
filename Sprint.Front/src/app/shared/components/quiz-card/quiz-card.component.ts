// src/app/shared/components/quiz-card/quiz-card.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

export interface QuizStatus {
  type: 'completed' | 'in-progress' | 'not-started' | 'failed';
  label: string;
  icon?: string;
}

@Component({
  selector: 'app-quiz-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './quiz-card.component.html',
  styleUrl: './quiz-card.component.scss'
})
export class QuizCardComponent {
  @Input() title: string = 'Quiz';
  @Input() description: string = '';
  @Input() duration: number = 0; // en minutes
  @Input() totalQuestions: number = 0;
  @Input() score: number | null = null; // ex: 8
  @Input() maxScore: number = 10;
  @Input() progress: number = 0; // 0-100
  @Input() status: QuizStatus = { type: 'not-started', label: 'À faire' };
  @Input() disabled: boolean = false;
}
