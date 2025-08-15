
// src/app/shared/components/breadcrumb/breadcrumb.component.ts
import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterModule } from '@angular/router';
import { EnumService, NiveauEnum } from '../../../core/services/enum.service';

export interface BreadcrumbItem {
  label: string;
  route?: string;
  niveau?: NiveauEnum; // 🔹 Ajout du niveau
}

@Component({
  selector: 'app-breadcrumb',
  imports:[CommonModule, RouterModule],
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss'],
  standalone: true
})
export class BreadcrumbComponent {
  @Input() items: BreadcrumbItem[] = [];

  constructor(private enumService: EnumService) {}

  getMetadata(item: BreadcrumbItem) {
    return item.niveau !== undefined
      ? this.enumService.getNiveauMetadata(item.niveau)
      : null;
  }
}