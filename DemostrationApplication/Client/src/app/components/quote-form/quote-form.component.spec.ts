import { HttpErrorResponse } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { of, throwError } from 'rxjs';
import { InsuranceType } from 'src/app/Model/InsuranceType';
import { quoteDetail } from 'src/app/Model/QuoteDetail';
import { QuoteRequest } from 'src/app/Model/QuoteRequest';
import { QuoteApiService } from 'src/app/service/quote-api.service';
import { toIsoDate } from 'src/app/validators/age-range.validator';
import { QuoteFormComponent } from './quote-form.component';

describe('QuoteFormComponent', () => {
  const details: quoteDetail = {
    makes: ['Ford', 'BMW'],
    models: [
      { make: 'Ford', models: ['Focus', 'Puma'] },
      { make: 'BMW', models: ['X5'] }
    ],
    insuranceTypes: [{ type: InsuranceType.FullyComprehensive, description: 'Fully Comprehensive' }]
  };

  const validRequest: QuoteRequest = {
    insuranceType: 'FullyComprehensive',
    dateOfBirth: '1990-01-01',
    make: 'Ford',
    model: 'Focus'
  };

  let fixture: ComponentFixture<QuoteFormComponent>;
  let component: QuoteFormComponent;
  let api: jasmine.SpyObj<QuoteApiService>;

  const page = () => fixture.nativeElement as HTMLElement;
  const submitButton = () => page().querySelector<HTMLButtonElement>('button[type=submit]')!;

  function submit(): void {
    submitButton().click();
    fixture.detectChanges();
  }

  beforeEach(async () => {
    api = jasmine.createSpyObj<QuoteApiService>('QuoteApiService', ['getDetails', 'getQuote']);
    api.getDetails.and.returnValue(of(details));

    await TestBed.configureTestingModule({
      declarations: [QuoteFormComponent],
      imports: [ReactiveFormsModule, RouterModule.forRoot([])],
      providers: [{ provide: QuoteApiService, useValue: api }]
    }).compileComponents();

    fixture = TestBed.createComponent(QuoteFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('shows the insurance types and makes from the API', () => {
    const insuranceOptions = page().querySelectorAll('#insuranceType option:not([disabled])');
    const makeOptions = page().querySelectorAll('#make option:not([disabled])');

    expect(Array.from(insuranceOptions).map(o => o.textContent?.trim())).toEqual(['Fully Comprehensive']);
    expect(Array.from(makeOptions).map(o => o.textContent?.trim())).toEqual(['Ford', 'BMW']);
  });

  it('lists only the models for the chosen make and clears a model from a previous make', () => {
    component.form.patchValue({ make: 'Ford', model: 'Focus' });
    component.form.controls.make.setValue('BMW');
    fixture.detectChanges();

    expect(component.models).toEqual(['X5']);
    expect(component.form.controls.model.value).toBe('');
  });

  it('does not request a quote and highlights every missing field when the form is incomplete', () => {
    submit();

    expect(api.getQuote).not.toHaveBeenCalled();
    expect(page().querySelectorAll('.is-invalid').length).toBe(4);
  });

  it('explains the age limit when the driver is too young', () => {
    const tenYearsAgo = new Date();
    tenYearsAgo.setFullYear(tenYearsAgo.getFullYear() - 10);

    component.form.controls.dateOfBirth.setValue(toIsoDate(tenYearsAgo));
    component.form.controls.dateOfBirth.markAsTouched();
    fixture.detectChanges();

    expect(page().querySelector('#dateOfBirthError')?.textContent).toContain('We can only quote drivers aged 17 to 80.');
  });

  it('shows the premium and quote reference for an accepted quote', () => {
    api.getQuote.and.returnValue(of({ quoteRequestValid: true, quote: 500, declined: false, reference: 'ABCD2345' }));

    component.form.patchValue(validRequest);
    submit();

    expect(api.getQuote).toHaveBeenCalledWith(validRequest);
    const quote = page().querySelector('[data-testid=quote]')!;
    expect(quote.textContent).toContain('£500.00');
    expect(quote.querySelector('[data-testid=reference]')?.textContent).toBe('ABCD2345');
    expect(quote.querySelector('a')?.getAttribute('href')).toBe('/quotes/ABCD2345');
  });

  it('shows the reason when a quote is declined', () => {
    api.getQuote.and.returnValue(of({
      quoteRequestValid: true, quote: 0, declined: true, declineReason: 'We can only provide quotes for drivers aged 17 to 80.'
    }));

    component.form.patchValue(validRequest);
    submit();

    expect(page().querySelector('[data-testid=declined]')?.textContent).toContain('drivers aged 17 to 80');
    expect(page().querySelector('[data-testid=quote]')).toBeNull();
  });

  it('shows an error and lets the user try again when the quote request fails', () => {
    api.getQuote.and.returnValue(throwError(() => new HttpErrorResponse({ status: 500 })));

    component.form.patchValue(validRequest);
    submit();

    expect(page().textContent).toContain('we couldn\'t get your quote');
    expect(submitButton().disabled).toBeFalse();
  });

  it('hides a quote once the details it was based on change', () => {
    api.getQuote.and.returnValue(of({ quoteRequestValid: true, quote: 500, declined: false, reference: 'ABCD2345' }));
    component.form.patchValue(validRequest);
    submit();

    component.form.controls.model.setValue('Puma');
    fixture.detectChanges();

    expect(page().querySelector('[data-testid=quote]')).toBeNull();
  });

  it('offers a retry when the quote options cannot be loaded', () => {
    api.getDetails.and.returnValue(throwError(() => new HttpErrorResponse({ status: 0 })));
    component.loadDetails();
    fixture.detectChanges();

    const retry = page().querySelector<HTMLButtonElement>('.alert-danger button')!;
    expect(retry.textContent).toContain('Try again');
    expect(submitButton().disabled).toBeTrue();

    api.getDetails.and.returnValue(of(details));
    retry.click();
    fixture.detectChanges();

    expect(page().querySelector('.alert-danger')).toBeNull();
    expect(submitButton().disabled).toBeFalse();
  });
});
