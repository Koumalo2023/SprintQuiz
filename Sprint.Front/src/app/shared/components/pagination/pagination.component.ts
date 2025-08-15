import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss'
})
export class PaginationComponent {
@Input() currentPage: number = 1;
  @Input() totalPages: number = 1;
  @Input() maxVisiblePages: number = 5;

  @Output() pageChange = new EventEmitter<number>();

  get pages(): number[] {
    const start = Math.max(1, this.currentPage - Math.floor(this.maxVisiblePages / 2));
    const end = Math.min(this.totalPages, start + this.maxVisiblePages - 1);

    const pages = [];
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }

    // Ajuster si on est trop près de la fin
    if (pages.length < this.maxVisiblePages && pages[0] > 1) {
      const newStart = Math.max(1, end - this.maxVisiblePages + 1);
      return Array.from({ length: this.maxVisiblePages }, (_, i) => newStart + i).filter(p => p <= this.totalPages);
    }

    return pages;
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.pageChange.emit(page);
    }
  }

  isFirstPage(): boolean {
    return this.currentPage === 1;
  }

  isLastPage(): boolean {
    return this.currentPage === this.totalPages;
  }
}
