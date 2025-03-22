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
      grossPrice: [0, [Validators.required, Validators.min(0)]],
      tax: [0, [Validators.required, Validators.min(0)]],
      currency: ['PLN', Validators.required],
      description: [''],
      status: ['Planned', Validators.required],
      serviceId: ['', Validators.required],
      deviceId: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.expenseId = +this.route.snapshot.paramMap.get('id')!;

    this.apiService.getServices().subscribe(data =>{
     this.services = data});
    this.apiService.getDevice().subscribe(data =>{
     this.devices = data.items});

    if (this.expenseId) {
      this.apiService.getExpensesByYear(new Date().getFullYear())
        .subscribe(expenses => {
          const expense = expenses.find(e => e.id === this.expenseId);
          if (expense) this.form.patchValue(expense);
        });
    }
  }

  onSubmit() {
    if (this.expenseId) {
      this.apiService.updateExpense(this.expenseId, this.form.value)
        .subscribe(() => this.router.navigate(['/expense-planner-list']));
    } else {
      this.apiService.createExpense(this.form.value)
        .subscribe(() => this.router.navigate(['/expense-planner-list']));
    }
  }
}
