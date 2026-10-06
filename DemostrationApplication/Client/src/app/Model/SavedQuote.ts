import { InsuranceType } from "./InsuranceType";

export interface SavedQuote {
    reference: string;
    createdUtc: string;
    dateOfBirth: string;
    make: string;
    model: string;
    insuranceType: InsuranceType;
    insuranceTypeDescription: string;
    premium: number;
}
