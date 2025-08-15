import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'uppercaseFirst',
  standalone: true
})
export class UppercaseFirstPipe implements PipeTransform {
  transform(value: string): string {
    return value ? value.charAt(0).toUpperCase() + value.slice(1) : value;
  }
}
