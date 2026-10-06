export class quoteResponse{
    quoteRequestValid: boolean = false;
    quote: number = 0;
    declined: boolean = false;
    declineReason?: string;
    reference?: string;
}
