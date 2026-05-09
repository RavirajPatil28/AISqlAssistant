import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { map } from 'rxjs';

@Component({
  selector: 'app-query',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './query.html',
  styleUrls: ['./query.css'],
})
export class Query {

  question: string = '';
  response: any = null;
  loading: boolean = false;
  errorMessage: string = '';

  constructor(
    private http: HttpClient,
    private cd: ChangeDetectorRef
  ) {}

  ask() {
    if (!this.question.trim()) return;

    this.loading = true;
    this.response = null;
    this.errorMessage = '';

    this.http.post<any>('https://localhost:7271/api/query', {
      question: this.question
    })
    .subscribe({
      next: (res) => {
        this.response = res;
        this.loading = false;

        this.cd.detectChanges(); // ensure UI updates
      },
      error: (err) => {
        console.error('API Error:', err);

        this.errorMessage =
          err?.error?.message ||
          err?.error ||
          'Something went wrong';

        this.loading = false;

        this.cd.detectChanges(); // ensure UI updates
      }
    });
  }

  setExample(event: any) {
    this.question = event.target.value;
  }

  getColumns(data: any[]) {
    if (!data || data.length === 0) return [];
    return Object.keys(data[0]).map(col => col.toUpperCase());
  }
}