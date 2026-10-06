import { HttpErrorResponse } from '@angular/common/http';
import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, map, Observable, of, switchMap, tap } from 'rxjs';
import { SavedQuote } from 'src/app/Model/SavedQuote';
import { QuoteApiService } from 'src/app/service/quote-api.service';

type LookupState = 'idle' | 'loading' | 'found' | 'notFound' | 'failed';

@Component({
  selector: 'app-retrieve-quote',
  templateUrl: './retrieve-quote.component.html',
  standalone: false
})
export class RetrieveQuoteComponent implements OnInit {

  readonly referenceCtrl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.pattern(/^\s*[A-Za-z0-9]{8}\s*$/)]
  });

  readonly lookupForm = new FormGroup({ reference: this.referenceCtrl });

  state: LookupState = 'idle';
  quote: SavedQuote | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private quoteApi: QuoteApiService,
    private destroyRef: DestroyRef) { }

  ngOnInit(): void {
    this.route.paramMap.pipe(
      map(params => params.get('reference')),
      tap(reference => {
        this.quote = null;
        this.state = reference ? 'loading' : 'idle';
        if (reference) {
          this.referenceCtrl.setValue(reference);
        }
      }),
      switchMap(reference => reference ? this.lookUp(reference) : of(null)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe();
  }

  find(): void {
    if (this.referenceCtrl.invalid) {
      this.referenceCtrl.markAsTouched();
      return;
    }
    this.router.navigate(['/quotes', this.referenceCtrl.value.trim().toUpperCase()]);
  }

  get showReferenceError(): boolean {
    return this.referenceCtrl.invalid && this.referenceCtrl.touched;
  }

  private lookUp(reference: string): Observable<unknown> {
    return this.quoteApi.getSavedQuote(reference).pipe(
      tap(quote => {
        this.quote = quote;
        this.state = 'found';
      }),
      catchError((error: HttpErrorResponse) => {
        this.state = error.status === 404 ? 'notFound' : 'failed';
        return of(null);
      })
    );
  }
}
