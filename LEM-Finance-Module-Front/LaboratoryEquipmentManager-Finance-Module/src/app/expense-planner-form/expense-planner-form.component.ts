import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiServiceService, PagedAndSortedQueryOfDevicesList } from '../api-service.service';

@Component({
  selector: 'app-expense-planner-form',
  templateUrl: './expense-planner-form.component.html',
  styleUrls: ['./expense-planner-form.component.css']
})
export class ExpensePlannerFormComponent implements OnInit {
  form: FormGroup;
  expenseId?: number;
  services: any[] = [];
  devices: any[] = [];
  deviceQuery = new PagedAndSortedQueryOfDevicesList();
  constructor(
    private fb: FormBuilder,
    private apiService: ApiServiceService,
    private route: ActivatedRoute,
    public router: Router
  ) {
    this.form = this.fb.group({
      plannedDate: ['', Validators.required],
      storageLocationName: [''],
      netPrice: [0, [Validators.required, Validators.min(0)]],
      grossPrice: [{ value: 0, disabled: true }],
      tax: [23, [Validators.required, Validators.min(0)]],
      currency: ['PLN', Validators.required],
      description: [''],
      status: ['Planned', Validators.required],
      serviceId: ['', Validators.required],
      deviceId: ['', Validators.required]
    });
  }

  currentExchangeRate: number = 1;

  convertedValues = {
    netPricePLN: 0,
    grossPricePLN: 0,
    netPrice: 0,
    grossPrice: 0,
    taxPLN: 0
  };

  currencies = [
    { code: 'PLN', name: 'Polski złoty' },
    { code: 'EUR', name: 'Euro' },
    { code: 'USD', name: 'Dolar amerykański' },
    { code: 'GBP', name: 'Funt szterling' },
    { code: 'CHF', name: 'Frank szwajcarski' },
    { code: 'JPY', name: 'Jen japoński' },
    { code: 'CZK', name: 'Korona czeska' },
    { code: 'SEK', name: 'Korona szwedzka' },
    { code: 'NOK', name: 'Korona norweska' },
    { code: 'DKK', name: 'Korona duńska' }
  ];
  
  ngOnInit() {
    this.expenseId = +this.route.snapshot.paramMap.get('id')!;

    this.apiService.getServices().subscribe(data =>{
     this.services = data});
    this.apiService.getDevice().subscribe(data =>{
     this.devices = data.items});
    this.form.valueChanges.subscribe(() => {
     this.calculatePrices();
    });

    this.form.valueChanges.subscribe(() => {
      this.calculatePrices();
    });
    this.form.get('currency')!.valueChanges.subscribe(currency => {
      this.updateExchangeRate(currency);

    this.form.get('netPrice')!.valueChanges.subscribe(() => this.calculatePrices());
    this.form.get('tax')!.valueChanges.subscribe(() => this.calculatePrices());

    this.calculatePrices();

    if (this.expenseId) {
      this.apiService.getExpensesByYear(new Date().getFullYear())
        .subscribe(expenses => {
          const expense = expenses.find(e => e.id === this.expenseId);
          if (expense) {
            this.form.patchValue(expense, { emitEvent: false });
            this.updateExchangeRate(expense.currency);
          }
        });
      }
    });
  }

  calculatePrices() {
    const net = this.form.value.netPrice || 0;
    const vatRate = this.form.value.tax || 0;

    const gross = net * (1 + vatRate / 100);
    this.form.get('grossPrice')!.setValue(gross.toFixed(2), { emitEvent: false });

    this.convertedValues = {
      netPricePLN: net * this.currentExchangeRate,
      grossPricePLN: gross * this.currentExchangeRate,
      netPrice: net,
      grossPrice: gross,
      taxPLN: (gross - net) * this.currentExchangeRate
    };
  }

  updateExchangeRate(currency: string): void {
    if (currency && currency !== 'PLN') {
      this.apiService.getExchangeRate(currency).subscribe({
        next: (response: { currency: string, rate: number }) => {
          this.currentExchangeRate = response.rate;
          this.calculatePrices();
        },
        error: (error: any) => {
          console.error("Błąd pobierania kursu", error);
          this.currentExchangeRate = 1;
        }
      });
    } else {
      this.currentExchangeRate = 1;
      this.calculatePrices();
    }
  }

  onSubmit() {
    const formValues = this.form.value;

  const expense = {
    ...formValues,
    exchangeRate: this.currentExchangeRate,
    netPricePLN: this.convertedValues.netPricePLN,
    grossPricePLN: this.convertedValues.grossPricePLN,
    netPrice: this.convertedValues.netPrice,
    grossPrice: this.convertedValues.grossPrice,
    taxPLN: this.convertedValues.taxPLN
  };

    if (this.expenseId) {
      this.apiService.updateExpense(this.expenseId, expense)
        .subscribe(() => this.router.navigate(['/expense-planner-list']));
    } else {
      this.apiService.createExpense(expense)
        .subscribe(() => this.router.navigate(['/expense-planner-list']));
    }
  }
}
