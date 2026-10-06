import { FormControl } from '@angular/forms';
import { ageOn, ageRangeValidator, toIsoDate } from './age-range.validator';

describe('ageOn', () => {
  const day = (year: number, month: number, date: number) => new Date(year, month - 1, date);

  it('counts a birthday that falls today', () => {
    expect(ageOn('2009-06-15', day(2026, 6, 15))).toBe(17);
  });

  it('does not count a birthday that is tomorrow', () => {
    expect(ageOn('2009-06-16', day(2026, 6, 15))).toBe(16);
  });

  it('treats a 29 February birthday as 1 March in non-leap years', () => {
    expect(ageOn('2008-02-29', day(2025, 2, 28))).toBe(16);
    expect(ageOn('2008-02-29', day(2025, 3, 1))).toBe(17);
  });

  it('handles birthdays at the end of the year', () => {
    expect(ageOn('2000-12-31', day(2026, 1, 1))).toBe(25);
  });
});

describe('toIsoDate', () => {
  it('formats a local date as yyyy-MM-dd', () => {
    expect(toIsoDate(new Date(2026, 0, 5))).toBe('2026-01-05');
  });
});

describe('ageRangeValidator', () => {
  const validate = (value: string) =>
    ageRangeValidator(17, 80, () => new Date(2026, 5, 15))(new FormControl(value));

  it('leaves empty values to the required validator', () => {
    expect(validate('')).toBeNull();
  });

  it('rejects values that are not dates', () => {
    expect(validate('15/06/2009')).toEqual({ invalidDate: true });
  });

  it('rejects dates in the future', () => {
    expect(validate('2026-06-16')).toEqual({ futureDate: true });
  });

  it('accepts drivers on their 17th birthday', () => {
    expect(validate('2009-06-15')).toBeNull();
  });

  it('rejects drivers the day before their 17th birthday', () => {
    expect(validate('2009-06-16')).toEqual({ ageRange: { min: 17, max: 80, actual: 16 } });
  });

  it('accepts drivers aged 80', () => {
    expect(validate('1945-06-16')).toBeNull();
  });

  it('rejects drivers on their 81st birthday', () => {
    expect(validate('1945-06-15')).toEqual({ ageRange: { min: 17, max: 80, actual: 81 } });
  });
});
