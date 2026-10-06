import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { QuoteFormComponent } from './components/quote-form/quote-form.component';
import { RetrieveQuoteComponent } from './components/retrieve-quote/retrieve-quote.component';

const routes: Routes = [
  { path: '', component: QuoteFormComponent, title: 'Get a quote | Car Insurance Quotes' },
  { path: 'quotes', component: RetrieveQuoteComponent, title: 'Retrieve a quote | Car Insurance Quotes' },
  { path: 'quotes/:reference', component: RetrieveQuoteComponent, title: 'Your quote | Car Insurance Quotes' },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
