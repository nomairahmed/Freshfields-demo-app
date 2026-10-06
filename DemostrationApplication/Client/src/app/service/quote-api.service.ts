import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/env';
import { quoteDetail } from 'src/app/Model/QuoteDetail';
import { QuoteRequest } from 'src/app/Model/QuoteRequest';
import { quoteResponse } from 'src/app/Model/QuoteResponse';
import { SavedQuote } from 'src/app/Model/SavedQuote';

@Injectable({
  providedIn: 'root'
})
export class QuoteApiService {

  private readonly baseUrl = `${environment.apiBaseUrl}/Quote`;

  constructor(private httpClient: HttpClient) { }

  getDetails(): Observable<quoteDetail> {
    return this.httpClient.get<quoteDetail>(this.baseUrl);
  }

  getQuote(request: QuoteRequest): Observable<quoteResponse> {
    return this.httpClient.post<quoteResponse>(this.baseUrl, request);
  }

  getSavedQuote(reference: string): Observable<SavedQuote> {
    return this.httpClient.get<SavedQuote>(`${this.baseUrl}/${encodeURIComponent(reference.trim())}`);
  }
}
