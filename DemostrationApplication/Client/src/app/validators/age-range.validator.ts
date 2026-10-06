import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const isoDatePattern = /^(\d{4})-(\d{2})-(\d{2})$/;

/** Formats a date as yyyy-MM-dd in local time, matching the value of an `<input type="date">`. */
export function toIsoDate(date: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
}

/** Age in whole years on `date` for someone born on `dateOfBirth` (yyyy-MM-dd). */
export function ageOn(dateOfBirth: string, date: Date): number {
  const [, year, month, day] = isoDatePattern.exec(dateOfBirth)!.map(Number);
  const currentMonth = date.getMonth() + 1;
  const hadBirthdayThisYear = currentMonth > month || (currentMonth === month && date.getDate() >= day);
  return date.getFullYear() - year - (hadBirthdayThisYear ? 0 : 1);
}

/**
 * Validates a yyyy-MM-dd date of birth. Empty values pass so that `Validators.required` reports them.
 * Errors: `invalidDate`, `futureDate`, or `ageRange: { min, max, actual }`.
 */
export function ageRangeValidator(minAge: number, maxAge: number, today: () => Date = () => new Date()): ValidatorFn {
  return (control: AbstractControl<string>): ValidationErrors | null => {
    const value = control.value;
    if (!value) {
      return null;
    }
    if (!isoDatePattern.test(value)) {
      return { invalidDate: true };
    }

    const now = today();
    if (value > toIsoDate(now)) {
      return { futureDate: true };
    }

    const age = ageOn(value, now);
    return age < minAge || age > maxAge
      ? { ageRange: { min: minAge, max: maxAge, actual: age } }
      : null;
  };
}
