import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { EnumService, NiveauDifficulte } from '../../../core/services/enum.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-quiz-question-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './quiz-question-form.component.html',
  styleUrls: ['./quiz-question-form.component.scss']
})
export class QuizQuestionFormComponent implements OnInit {
  @Input() question: any = null; // Doit être une question avec options
  @Input() questionIndex!: number;
  @Output() removeQuestion = new EventEmitter<number>();

  NiveauDifficulte = NiveauDifficulte;

  constructor(public enumService: EnumService) {}

  ngOnInit(): void {
    // Sécurisation : toujours initialiser options avec au moins 2 entrées
    if (!this.question) {
      this.question = {
        intitule: '',
        niveauDifficulte: NiveauDifficulte.Moyen,
        explication: '',
        options: []
      };
    }

    if (!Array.isArray(this.question.options)) {
      this.question.options = [];
    }

    // Si aucune option n'existe, créer 2 par défaut
    if (this.question.options.length === 0) {
      this.question.options.push(
        { texte: '', estCorrecte: false },
        { texte: '', estCorrecte: false }
      );
    }
  }

  // Ajouter une option
  addOption(): void {
    this.question.options.push({
      texte: '',
      estCorrecte: false
    });
  }

  // Supprimer une option
  removeOption(optionIndex: number): void {
    this.question.options.splice(optionIndex, 1);

    // Empêcher qu'il reste moins de 2 options
    if (this.question.options.length < 2) {
      while (this.question.options.length < 2) {
        this.addOption();
      }
    }
  }

  // Supprimer la question
  onRemove(): void {
    this.removeQuestion.emit(this.questionIndex);
  }
}