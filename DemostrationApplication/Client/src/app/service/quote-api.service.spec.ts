import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from 'src/environments/env';
import { QuoteRequest } from 'src/app/Model/QuoteRequest';
import { QuoteApiService } from './quote-api.service';

describe('QuoteApiService', () => {
  const baseUrl = `${environment.apiBaseUrl}/Quote`;
  let service: QuoteApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(QuoteApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('loads the quote options', () => {
    const details = { makes: ['Ford'], models: [], insuranceTypes: [] };
    let received: unknown;

    service.getDetails().subscribe(d => received = d);
    const request = http.expectOne(baseUrl);
    request.flush(details);

    expect(request.request.method).toBe('GET');
    expect(received).toEqual(details);
  });

  it('posts the quote request', () => {
    const quoteRequest: QuoteRequest = {
      insuranceType: 'FullyComprehensive', dateOfBirth: '1990-01-01', make: 'Ford', model: 'Focus'
    };

    service.getQuote(quoteRequest).subscribe();
    const request = http.expectOne(baseUrl);

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(quoteRequest);
    request.flush({ quoteRequestValid: true, quote: 200, declined: false });
  });

  it('requests a saved quote using the trimmed, URL-encoded reference', () => {
    service.getSavedQuote('  AB/CD 12 ').subscribe();

    const request = http.expectOne(`${baseUrl}/AB%2FCD%2012`);
    expect(request.request.method).toBe('GET');
    request.flush({});
  });
});
