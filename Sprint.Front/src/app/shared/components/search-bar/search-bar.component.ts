import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.scss'
})
export class SearchBarComponent {
  @Input() placeholder: string = 'Rechercher...';
  @Input() showFilters: boolean = false;
  @Output() search = new EventEmitter<string>();
  @Output() filterToggle = new EventEmitter<void>();

  searchTerm: string = '';

  onSearch() {
    this.search.emit(this.searchTerm);
  }

  onFilterClick() {
    this.filterToggle.emit();
  }
}
