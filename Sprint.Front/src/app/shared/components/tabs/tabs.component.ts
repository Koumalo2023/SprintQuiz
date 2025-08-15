// src/app/shared/components/tabs/tabs.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';

interface Tab {
  label: string;
  active?: boolean;
  disabled?: boolean;
}

@Component({
  selector: 'app-tabs',
  imports: [CommonModule],
  templateUrl: './tabs.component.html',
  styleUrl: './tabs.component.scss'
})
export class TabsComponent {
 @Input() tabs: Tab[] = [];
  @Input() activeTab: number = 0;
  @Output() tabChange = new EventEmitter<number>();

  onTabClick(index: number, tab: Tab): void {
    if (tab.disabled) return;
    this.activeTab = index;
    this.tabChange.emit(index);
  }
}
