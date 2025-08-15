import { CommonModule } from '@angular/common';
import { AfterContentInit, Component, ContentChild, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.scss'
})
export class ModalComponent implements AfterContentInit {
  @Input() title: string = '';
  @Input() visible: boolean = false;
  @Input() closable: boolean = true;
  @Input() closeOnEscape: boolean = true;
  @Input() closeOnOverlayClick: boolean = true;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() onClose = new EventEmitter<void>();

  // Détecte si du contenu a été projeté dans le footer
  @ContentChild('modalFooter', { static: false }) modalFooter: any;

  // Flag mis à jour après le rendu du contenu
  hasFooter = false;

  ngAfterContentInit(): void {
    this.hasFooter = !!this.modalFooter;
  }

  handleEscape(event: KeyboardEvent): void {
    if (this.closeOnEscape && event.key === 'Escape' && this.visible) {
      this.close();
    }
  }

  close(): void {
    if (this.visible) {
      this.visible = false;
      this.visibleChange.emit(false);
      this.onClose.emit();
    }
  }

  onOverlayClick(event: MouseEvent): void {
    if (this.closeOnOverlayClick && (event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close();
    }
  }

  stopPropagation(event: MouseEvent): void {
    event.stopPropagation();
  }
}
