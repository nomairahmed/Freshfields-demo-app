import { HttpErrorResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { provideRouter, Router, RouterModule } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { of, throwError } from 'rxjs';
import { InsuranceType } from 'src/app/Model/InsuranceType';
import { SavedQuote } from 'src/app/Model/SavedQuote';
import { QuoteApiService } from 'src/app/service/quote-api.service';
import { RetrieveQuoteComponent } from './retrieve-quote.component';

describe('RetrieveQuoteComponent', () => {
  const savedQuote: SavedQuote = {
    reference: 'ABCD2345',
    createdUtc: '2026-06-15T09:30:00Z',
    dateOfBirth: '1990-01-01',
    make: 'BMW',
    model: 'X5',
    insuranceType: InsuranceType.FullyComprehensive,
    insuranceTypeDescription: 'Fully Comprehensive',
    premium: 500
  };

  let api: jasmine.SpyObj<QuoteApiService>;
  let harness: RouterTestingHarness;

  const page = () => harness.routeNativeElement as HTMLElement;

  beforeEach(async () => {
    api = jasmine.createSpyObj<QuoteApiService>('QuoteApiService', ['getSavedQuote']);

    TestBed.configureTestingModule({
      declarations: [RetrieveQuoteComponent],
      imports: [ReactiveFormsModule, RouterModule],
      providers: [
        provideRouter([
          { path: 'quotes', component: RetrieveQuoteComponent },
          { path: 'quotes/:reference', component: RetrieveQuoteComponent }
        ]),
        { provide: QuoteApiService, useValue: api }
      ]
    });

    harness = await RouterTestingHarness.create();
  });

  it('shows just the lookup form when no reference is given', async () => {
    await harness.navigateByUrl('/quotes', RetrieveQuoteComponent);

    expect(api.getSavedQuote).not.toHaveBeenCalled();
    expect(page().querySelector('#reference')).not.toBeNull();
    expect(page().querySelector('[data-testid=saved-quote]')).toBeNull();
  });

  it('loads and shows the quote for the reference in the URL', async () => {
    api.getSavedQuote.and.returnValue(of(savedQuote));

    await harness.navigateByUrl('/quotes/ABCD2345', RetrieveQuoteComponent);
    harness.detectChanges();

    expect(api.getSavedQuote).toHaveBeenCalledWith('ABCD2345');
    const quote = page().querySelector('[data-testid=saved-quote]')!;
    expect(quote.textContent).toContain('Quote ABCD2345');
    expect(quote.textContent).toContain('£500.00');
    expect(quote.textContent).toContain('Fully Comprehensive');
    expect(quote.textContent).toContain('BMW X5');
    expect(quote.textContent).toContain('1 January 1990');
  });

  it('says so when no quote has that reference', async () => {
    api.getSavedQuote.and.returnValue(throwError(() => new HttpErrorResponse({ status: 404 })));

    await harness.navigateByUrl('/quotes/ZZZZZZZZ', RetrieveQuoteComponent);
    harness.detectChanges();

    expect(page().querySelector('[data-testid=not-found]')).not.toBeNull();
    expect(page().querySelector('[data-testid=saved-quote]')).toBeNull();
  });

  it('shows an error when the lookup fails for another reason', async () => {
    api.getSavedQuote.and.returnValue(throwError(() => new HttpErrorResponse({ status: 500 })));

    await harness.navigateByUrl('/quotes/ABCD2345', RetrieveQuoteComponent);
    harness.detectChanges();

    expect(page().textContent).toContain('we couldn\'t look up your quote');
    expect(page().querySelector('[data-testid=not-found]')).toBeNull();
  });

  it('navigates to the quote for an entered reference', async () => {
    api.getSavedQuote.and.returnValue(of(savedQuote));
    const component = await harness.navigateByUrl('/quotes', RetrieveQuoteComponent);

    component.referenceCtrl.setValue('  abcd2345 ');
    component.find();
    await harness.fixture.whenStable();
    harness.detectChanges();

    expect(TestBed.inject(Router).url).toBe('/quotes/ABCD2345');
    expect(api.getSavedQuote).toHaveBeenCalledWith('ABCD2345');
  });

  it('asks for a valid reference instead of searching for a badly formed one', async () => {
    const component = await harness.navigateByUrl('/quotes', RetrieveQuoteComponent);

    component.referenceCtrl.setValue('abc');
    component.find();
    harness.detectChanges();

    expect(TestBed.inject(Router).url).toBe('/quotes');
    expect(page().querySelector('#reference')?.classList).toContain('is-invalid');
  });
});
