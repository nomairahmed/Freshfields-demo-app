import { registerLocaleData } from '@angular/common';
import { provideHttpClient } from '@angular/common/http';
import localeEnGb from '@angular/common/locales/en-GB';
import { LOCALE_ID, NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { QuoteFormComponent } from './components/quote-form/quote-form.component';
import { RetrieveQuoteComponent } from './components/retrieve-quote/retrieve-quote.component';

registerLocaleData(localeEnGb);

@NgModule({
  declarations: [
    AppComponent,
    QuoteFormComponent,
    RetrieveQuoteComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule
  ],
  providers: [
    provideHttpClient(),
    { provide: LOCALE_ID, useValue: 'en-GB' }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
