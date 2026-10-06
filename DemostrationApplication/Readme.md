## Demo Application
This is a demonstration application that can be used for practicing skills or trying out something new.  Some ideas are listed below for activities that could be attempted using the application.

- There is a runtime bug that prevents the BMW models from being displayed.  See if you can address this.
- The Quote system is quite basic.  Introduce the date of birth in to the calculation so that people under the age of 17 and over the age of 80 can not receive a quote.  As part of doing this consider refactoring the code to make it easier to extend in the future.  Also introduce some further unit testing to support the changed that have been made.
- The user interface is quite simple.  Make changes to the user interface to improve the user experience.  This could take the form of making the appearance more apealing or improving the validation code including adding  support for the date of birth changes that were carried out in the previous step.  The unit tests could also be reviewed and added to.
- Make changes to store quotes for later retrieval.

## Changes 
This section describes some of the changes made to the application and scope for future improvement
### BMW models
The dropdown was not populating with BMW models due to the make being stored twice with conflicting values("BMS") as a typo. the typo was corrected and then further refactoring was done to create `VehicleCatalogue` to store makes and models.
### Pricing
`PriceTable` was created so `PerformQuote` no longer uses nested if statements as that approach is tedious and not the best coding practice. `PriceTable` makes use of a dictionary to implement the same behaviour
### Age
Drivers under the age of 17 and over the age of 80 are now declined quotes. This is validated on both the frontend and the backend. While validation on frontend alone would be enough for the scope of this exercise, implenting checks on the backend would help with any future expansion of the app. In addition a status for quote being declined is returned instead of a quote of 0 so make the user aware
### User Interface
Bootstrap card layout was used to enhance the user interface and give a more tidy appearance. Seperate pages for viewing quote and retreving previously checked quotes are also implmented.
### Storing quotes
An accepted quote is saved with the premium that was quoted, the car, the date of birth, and the time it was created. It is given an 8-character reference (for example `N7J3FWCA`). Entity Framework Core and SQLite is used for the implementation.

### Possible future implementation

 - Anyone with a reference number can retrieve a saved quote, including the date of birth. References are hard to guess. A production system would also ask for the date of birth, or require a login.
- Quotes do not expire.
- The pricing of cars was left as it is in the initial application but has some inconsistencies which can be fixed such as a higher rate for Third party and theft insurance when compared to comprehensive, which is unrealistic in practice
- Requests for cars not in the catalogue should be declined however it is still defaulted to 300 as I did not make any changes to the pricing rules