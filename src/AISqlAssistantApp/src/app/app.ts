import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Query } from './components/query/query';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Query],
  templateUrl: './app.html',
  styleUrl: './app.css',
  template: `<app-query></app-query>`
})
export class App {
  protected readonly title = signal('AISqlAssistantApp');
}
