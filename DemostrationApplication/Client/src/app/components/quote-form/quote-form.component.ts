import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { finalize } from 'rxjs';
import { quoteDetail } from 'src/app/Model/QuoteDetail';
import { quoteResponse } from 'src/app/Model/QuoteResponse';
import { QuoteApiService } from 'src/app/service/quote-api.service';
import { ageRangeValidator, toIsoDate } from 'src/app/validators/age-range.validator';

type QuoteField = 'insuranceType' | 'dateOfBirth' | 'make' | 'model';

@Component({
  selector: 'app-quote-form',
  templateUrl: './quote-form.component.html',
  standalone: false
})
export class QuoteFormComponent implements OnInit {

  readonly minAge = 17;
  readonly maxAge = 80;
  readonly today = toIsoDate(new Date());

  details = new quoteDetail();
  models: string[] = [];
  detailsFailed = false;
  submitting = false;
  submitFailed = false;
  result: quoteResponse | null = null;

  readonly form = new FormGroup({
    insuranceType: new FormControl('', { nonNullable: true, validators: Validators.required }),
    dateOfBirth: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, ageRangeValidator(this.minAge, this.maxAge)]
    }),
    make: new FormControl('', { nonNullable: true, validators: Validators.required }),
    model: new FormControl('', { nonNullable: true, validators: Validators.required }),
  });

  constructor(private quoteApi: QuoteApiService, private destroyRef: DestroyRef) { }

  ngOnInit(): void {
    this.loadDetails();

    this.form.controls.make.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(make => {
        this.models = this.modelsFor(make);
        this.form.controls.model.setValue('');
      });

    // A displayed quote must always match the details currently in the form.
    this.form.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        this.result = null;
        this.submitFailed = false;
      });
  }

  loadDetails(): void {
    this.detailsFailed = false;
    this.quoteApi.getDetails().subscribe({
      next: details => {
        this.details = details;
        this.models = this.modelsFor(this.form.controls.make.value);
      },
      error: () => this.detailsFailed = true
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting = true;
    this.submitFailed = false;
    this.quoteApi.getQuote(this.form.getRawValue())
      .pipe(finalize(() => this.submitting = false))
      .subscribe({
        next: response => this.result = response,
        error: () => this.submitFailed = true
      });
  }

  showError(field: QuoteField): boolean {
    const control = this.form.controls[field];
    return control.invalid && (control.touched || control.dirty);
  }

  get dateOfBirthError(): string {
    const errors = this.form.controls.dateOfBirth.errors;
    if (errors?.['required']) return 'Enter your date of birth.';
    if (errors?.['invalidDate']) return 'Enter a valid date of birth.';
    if (errors?.['futureDate']) return 'Your date of birth can\'t be in the future.';
    if (errors?.['ageRange']) return `We can only quote drivers aged ${this.minAge} to ${this.maxAge}.`;
    return '';
  }

  private modelsFor(make: string): string[] {
    return this.details.models.find(m => m.make === make)?.models ?? [];
  }
}
